<script>
    import ComprobanteCliente from "./ComprobanteCliente.svelte";
    import { jsPDF } from "jspdf";
    import { get } from "svelte/store";
    // Asumiendo que estos stores están definidos correctamente
    import { cart, subtotal, totalItems } from "./stores.js"; 

    let ordenesConfirmadas = [];
    let mostrarComprobante = false;
    let confirmacionExitosa = false;
    let isProcessing = false; // Estado para deshabilitar el botón durante la compra
    
    // token e idUsuario desde localStorage
    const token = localStorage.getItem("token");
    const idUsuario = localStorage.getItem("idUsuario");

    // Función auxiliar para obtener el nombre del proveedor
    function getProveedorName(item) {
        return item.proveedor?.nombreProveedor ?? item.nombreProveedor ?? "Proveedor desconocido";
    }

    // Función auxiliar para calcular el precio total del item
    function getItemPrice(item) {
        const price = item.price ?? item.precioUnitario ?? 0;
        const quantity = item.quantity ?? item.cantidad ?? 1;
        return (price * quantity).toFixed(2);
    }

    // Función de la lógica de negocio (sin cambios importantes en la funcionalidad)
    async function finalizarCompra() {
        if (isProcessing) return; 
        
        const carrito = get(cart);

        if (!carrito || carrito.length === 0) {
            alert("El carrito está vacío.");
            return;
        }

        isProcessing = true;
        ordenesConfirmadas = [];
        const resultados = []; 

        // 1. Agrupar por idProveedor
        const grupos = carrito.reduce((acc, item) => {
            const prov =
                item.idProveedor ??
                item.proveedor?.idProveedor ??
                item.proveedor?.id ??
                null;

            if (prov == null) {
                console.warn("Item sin idProveedor detectado:", item);
                return acc;
            }

            const key = String(prov);
            if (!acc[key]) acc[key] = [];
            acc[key].push(item);
            return acc;
        }, {});

        // 2. Crear órdenes en la API
        for (const [idProveedor, itemsProveedor] of Object.entries(grupos)) {
            // ... (Lógica de construcción de ordenRequest y fetch POST a /api/orden) ...
            // (Tu lógica actual de creación de orden va aquí, solo por brevedad la omito en este bloque)
            
            // --- INICIO: Tu lógica de `ordenRequest` y `fetch` a la API ---
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
            // --- FIN: Tu lógica de creación de orden ---
        }

        // 3. Confirmación de Pago y Descuento de Stock
        const algunFallo = resultados.some((r) => !r.ok);
        confirmacionExitosa = true; // Asumir éxito inicial

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
                        console.error(`Fallo al confirmar pago/descontar stock para Orden ${orden.idOrden}`);
                        confirmacionExitosa = false;
                        // Opcional: Podrías romper el bucle o seguir intentando
                    }
                } catch (err) {
                    console.error(`Fetch error al confirmar pago para Orden ${orden.idOrden}:`, err);
                    confirmacionExitosa = false;
                }
            }
            
            // 4. Resultado Final
            if (confirmacionExitosa) {
              console.log("ordenes generadas: ", ordenesConfirmadas);
              
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
            on:click={finalizarCompra}
            class="btn btn-checkout"
            disabled={isProcessing}
        >
            {isProcessing ? 'Procesando Compra...' : 'Pagar y Finalizar Compra'}
        </button>
    {:else}
        <div class="cart-empty">
            <p>El carrito está vacío.</p>
        </div>
    {/if}

    {#if mostrarComprobante}
        <ComprobanteCliente
            ordenes={ordenesConfirmadas}
            onClose={() => (mostrarComprobante = false)}
        />
    {/if}
</div>

