<script>
    import { onMount } from 'svelte';
    import { get } from "svelte/store";
    import ComprobanteCliente from "./ComprobanteCliente.svelte";
    import { cart, subtotal, totalItems } from "./stores.js"; 
    

    let ordenesConfirmadas = [];
    let mostrarComprobante = false;
    let isProcessing = false; 


    let mostrarModalDireccion = false;
    let selectedAddressKey = ''; // Clave de la dirección seleccionada (e.g., 'Direccion1')
    

    let userAddresses = []; 
    let isLoadingAddresses = true; 
    let addressFetchError = false; 
    
    // token e idUsuario desde localStorage
    const token = localStorage.getItem("token");
    const idUsuario = localStorage.getItem("idUsuario");
    
    // --- Funciones Auxiliares ---

    // Función auxiliar para obtener el nombre del proveedor
    function getProveedorName(item) {
        return item.proveedor?.nombreProveedor ?? item.nombreProducto ?? "Proveedor desconocido";
    }


    function getItemPrice(item) {
        const price = item.price ?? item.precioUnitario ?? 0;
        const quantity = item.quantity ?? item.cantidad ?? 1;
        return (price * quantity).toFixed(2);
    }
    
    
    onMount(async () => {
        if (!idUsuario) {
            isLoadingAddresses = false;
            addressFetchError = true;
            return;
        }

        try {

            const res = await fetch(`http://localhost:5029/api/usuario/${idUsuario}`, {
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { Authorization: `Bearer ${token}` } : {}),
                },
            });

            if (res.ok) {
                const data = await res.json();
                const loadedAddresses = [];
                
                if (data.direccion1 && data.direccion1.trim() !== "") {
                    loadedAddresses.push({ key: 'Direccion1', label: data.direccion1, value: data.direccion1});
                }
                if (data.direccion2 && data.direccion2.trim() !== "") {
                    loadedAddresses.push({ key: 'Direccion2', label:  data.direccion2, value: data.direccion2 });
                }
                if (data.direccion3 && data.direccion3.trim() !== "") {
                    loadedAddresses.push({ key: 'Direccion3', label: data.direccion3, value: data.direccion3});
                }

                userAddresses = loadedAddresses;
                
                // Establecer la primera como predeterminada
                if (userAddresses.length > 0) {
                    selectedAddressKey = userAddresses[0].key;
                }
            } else {
                addressFetchError = true;
            }
        } catch (error) {
            addressFetchError = true;
        } finally {
            isLoadingAddresses = false;
        }
    });

    // --- Funciones de Flujo de Compra ---
    
    // Paso 1: Abrir el modal de selección y validación
    function iniciarCompra() {
        if (isProcessing || get(cart).length === 0) return;

        if (isLoadingAddresses) {
            alert("Aún estamos cargando sus direcciones. Por favor, espere.");
            return;
        }
        
        if (addressFetchError || userAddresses.length === 0) {
            alert("No tiene direcciones de envío válidas. Añada al menos una a su perfil.");
            return;
        }
        
        mostrarModalDireccion = true; 
    }
    

    async function finalizarCompra() {
        if (isProcessing) return; 

        const selectedAddress = userAddresses.find(addr => addr.key === selectedAddressKey);
        if (!selectedAddress) {
            alert("Debe seleccionar una dirección válida.");
            return;
        }
        const direccionSeleccionada = selectedAddress.label;
        
        const carrito = get(cart);

        if (!carrito || carrito.length === 0) return;

        isProcessing = true;
        ordenesConfirmadas = [];
        let confirmacionGeneralExitosa = false;
        const resultados = []; 

        const grupos = carrito.reduce((acc, item) => {
            const prov =
                item.idProveedor ??
                item.proveedor?.idProveedor ??
                item.proveedor?.id ??
                null;

            if (prov == null) return acc;

            const key = String(prov);
            if (!acc[key]) acc[key] = [];
            acc[key].push(item);
            return acc;
        }, {});

        for (const [idProveedor, itemsProveedor] of Object.entries(grupos)) {
            const items = itemsProveedor.map((i) => ({
                idProducto: i.idProducto ?? i.id ?? 0,
                cantidad: Number(i.quantity ?? i.cantidad ?? 1),
                precioUnitario: Number(i.price ?? i.precioUnitario ?? 0),
                impuesto: Number(i.impuesto ?? 0),
                descuento: Number(i.descuento ?? 0),
            }));

            const montoTotal = items.reduce(
                (s, it) => s + it.precioUnitario * it.cantidad,
                0,
            );

            const ordenRequest = {
                idUsuario: Number(idUsuario) || 0,
                idProveedor: Number(idProveedor),
                montoTotal,
                numeroFactura: "TEMP-" + Date.now(), 
                tipoComprobante: "Boleta", 
                fechaFactura: new Date().toISOString(),
                metodoPago: "Pendiente",
                moneda: "CLP",
                impuestos: 0,
                descuento: 0,
                items,
                DireccionEnvioCompleta: direccionSeleccionada
            };

            try {
                const res = await fetch("http://localhost:5029/api/orden", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        ...(token ? { Authorization: `Bearer ${token}` } : {}),
                    },
                    body: JSON.stringify(ordenRequest),
                });
                

                if (!res.ok) {
                    const text = await res.text();
                    resultados.push({ idProveedor, ok: false, status: res.status, message: text });
                } else {
                    const data = await res.json();
                    resultados.push({ idProveedor, ok: true, data });
                    ordenesConfirmadas.push(data);
                }
            } catch (err) {
                resultados.push({ idProveedor, ok: false, error: err.message });
            }
        }

        const algunFallo = resultados.some((r) => !r.ok);
        confirmacionGeneralExitosa = true; 

        if (!algunFallo) {
            for (const orden of ordenesConfirmadas) {
                const confirmPagoUrl = `http://localhost:5029/api/orden/${orden.idOrden}/confirmar-pago`;

                const pagoRequest = {
                    NumeroFactura: orden.numeroFactura,
                    TipoComprobante: orden.tipoComprobante,
                    MetodoPago: "Tarjeta", 
                    Moneda: "CLP",
                    Impuestos: orden.impuestos,
                    Descuento: orden.descuento,
                };
                
                try {
                    const resPago = await fetch(confirmPagoUrl, {
                        method: "PUT",
                        headers: {
                            "Content-Type": "application/json",
                            ...(token ? { Authorization: `Bearer ${token}` } : {}),
                        },
                        body: JSON.stringify(pagoRequest),
                    });
                    
                    if (!resPago.ok) {
                        confirmacionGeneralExitosa = false;
                    }
                } catch (err) {
                    confirmacionGeneralExitosa = false;
                }
            }
            
            if (confirmacionGeneralExitosa) {
                
                cart.set([]); // limpiar carrito solo si todo OK
                mostrarComprobante = true; // mostrar comprobante
            } else {
                alert("Las órdenes se crearon, pero hubo errores al confirmar el pago o descontar stock. Revisa la consola.");
            }
        } else {
            alert("Hubo errores al crear algunas órdenes. Revisa la consola para más detalles.");
        }
        
        isProcessing = false;
    }
</script>

<div class="cart-container-card">
    <div class="cart-header">
        <h4 class="cart-title">Tu Carrito ({$totalItems})</h4>
        <span class="cart-icon">🛒</span> 
    </div>

    {#if $cart.length > 0}
        <div class="cart-items-list">
            {#each $cart as item}
                <div class="cart-item">
                    <div class="item-details">
                        <div class="item-name">
                            {item.name ?? item.nombreProducto ?? "Producto"}
                            <span class="item-quantity"> x{item.quantity ?? item.cantidad}</span>
                        </div>
                        <div class="item-provider">
                            de {getProveedorName(item)}
                        </div>
                    </div>
                    <div class="item-price">
                        ${getItemPrice(item)}
                    </div>
                </div>
            {/each}
        </div>

        <div class="cart-summary">
            <div class="summary-line">
                <span class="summary-label">Subtotal:</span>
                <span class="summary-value">${$subtotal.toFixed(2)}</span>
            </div>
            <div class="summary-line total-line">
                <span class="summary-label">Total a Pagar:</span>
                <span class="summary-value total-value">${$subtotal.toFixed(2)}</span>
            </div>
        </div>

        <button
            on:click={iniciarCompra} class="btn btn-checkout"
            disabled={isProcessing || $totalItems === 0 || isLoadingAddresses || addressFetchError}
        >
            {#if isLoadingAddresses}
                Cargando Direcciones...
            {:else if isProcessing}
                Procesando Compra...
            {:else if addressFetchError}
                Error al Cargar Direcciones
            {:else}
                Pagar y Finalizar Compra
            {/if}
        </button>
    {:else}
        <div class="cart-empty">
            <p>El carrito está vacío.</p>
        </div>
    {/if}

    <!-- svelte-ignore a11y_no_static_element_interactions -->
     <!-- svelte-ignore a11y_click_events_have_key_events -->
    {#if mostrarModalDireccion}
        
        <div class="modal-overlay" on:click={() => (mostrarModalDireccion = false)}>
            <div class="address-modal-content" on:click|stopPropagation>
                
                <div class="modal-header-elegant">
                    <h3 class="modal-title">Selección de Dirección de Envío</h3>
                    <button class="close-btn" on:click={() => (mostrarModalDireccion = false)}>&times;</button>
                </div>

                <div class="modal-body-content">
                    {#if userAddresses.length > 0}
                        <h4>Direcciones disponibles ({userAddresses.length})</h4>
                        <label for="address-select">Selecciona dónde quieres recibir tu pedido:</label>
                        
                        <select id="address-select" bind:value={selectedAddressKey} class="address-select-box">
                            {#each userAddresses as addr}
                                <option value={addr.key}>{addr.label}</option>
                            {/each}
                        </select>
                    {:else}
                        <p class="no-address"> No tienes direcciones de envío configuradas.</p>
                    {/if}
                </div>

                <div class="modal-footer-action">
                    <button class="btn btn-cancel" on:click={() => (mostrarModalDireccion = false)} disabled={isProcessing}>Cancelar</button>
                    <button 
                        class="btn btn-primary-confirm" 
                        on:click={() => {
                            mostrarModalDireccion = false; // Cierra el modal
                            finalizarCompra(); // Ejecuta la compra con la dirección seleccionada
                        }} 
                        disabled={isProcessing || userAddresses.length === 0}
                    >
                        {#if isProcessing}
                            Procesando Pago...
                        {:else}
                            Continuar y Pagar
                        {/if}
                    </button>
                </div>
            </div>
        </div>
    {/if}

    {#if mostrarComprobante}
        <ComprobanteCliente
            ordenes={ordenesConfirmadas}
            onClose={() => (mostrarComprobante = false)}
        />
    {/if}
</div>


