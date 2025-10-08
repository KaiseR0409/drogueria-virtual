<script>
    import CotizacionModal from './CotizacionModal.svelte';
    import { onMount } from "svelte";
    import { cart, filters } from "./stores.js"; 

    let products = [];
    let errMessage = "";

    // conjunto de ids de tarjetas expandidas
    let expanded = new Set();

    // estados del modal 
    let mostrarCotizacionModal = false;
    let productoBaseParaCotizar = null; 

    // helper para obtener id consistente
    const getId = (p) => p.idProducto ?? p.id ?? p.IdProducto ?? p.idProducto;

    function toggleExpand(p) {
        const id = getId(p);
        if (!id && id !== 0) return;
        if (expanded.has(id)) {
            expanded.delete(id);
        } else {
            expanded.add(id);
        }
        // re-asignar para activar reactividad
        expanded = new Set(expanded);
    }

    // función traer productos 
    async function fetchProducts() {
        try {
            let apiUrl = `http://localhost:5029/api/ProveedorProducto/InventarioCompletoConFiltros`;
            const params = new URLSearchParams();
            
            $filters.laboratoriosSeleccionados.forEach((lab) =>
                params.append("laboratoriosSeleccionados", lab),
            );
            
            if ($filters.busquedaNombre)
                params.append("nombreProducto", $filters.busquedaNombre);
                
            if ($filters.busquedaPrincipioActivo)
                params.append("principioActivo", $filters.busquedaPrincipioActivo);
                
            if ($filters.busquedaLaboratorio)
                params.append("laboratorioFabricante", $filters.busquedaLaboratorio);
                
            if ($filters.busquedaFormaFarmaceutica)
                params.append("formaFarmaceutica", $filters.busquedaFormaFarmaceutica);

            apiUrl += `?${params.toString()}`;

            const res = await fetch(apiUrl);
            if (!res.ok)
                throw new Error(
                    `Error en la solicitud: ${res.status} ${res.statusText}`,
                );
            const data = await res.json();

            if (Array.isArray(data) && data.length && data[0].producto) {
                const grouped = data.reduce((acc, item) => {
                    const prodId = item.idProducto;
                    if (!acc[prodId]) {
                        const p = item.producto || {};
                        acc[prodId] = {
                            idProducto: item.idProducto,
                            nombreProducto: p.nombreProducto ?? "",
                            principioActivo: p.principioActivo ?? "",
                            concentracion: p.concentracion ?? "",
                            formaFarmaceutica: p.formaFarmaceutica ?? "",
                            presentacionComercial: p.presentacionComercial ?? "",
                            laboratorioFabricante: p.laboratorioFabricante ?? "",
                            registroSanitario: p.registroSanitario ?? "",
                            fechaVencimiento: p.fechaVencimiento ?? null,
                            condicionesAlmacenamiento: p.condicionesAlmacenamiento ?? "",
                            imagenUrl: p.imagenUrl ?? "",
                            proveedores: [],
                        };
                    }
                    acc[prodId].proveedores.push({
                        idProveedor: item.idProveedor,
                        nombreProveedor: item.proveedor?.nombreProveedor || `Proveedor #${item.idProveedor}`,
                        precio: item.precio ?? 0,
                        stock: item.stock ?? 0,
                        __raw: item,
                    });
                    return acc;
                }, {});
                
                products = Object.values(grouped);
                // Opcional: Ordenar proveedores por precio (más barato primero)
                products.forEach(p => {
                    p.proveedores.sort((a, b) => a.precio - b.precio);
                });

            } else {
                products = [];
            }
            errMessage = "";
        } catch (error) {
            console.error(
                "No se pudo obtener la información de medicamentos:",
                error,
            );
            errMessage =
                "No se pudieron cargar los productos. Por favor, intente de nuevo más tarde.";
            products = [];
        }
    }


    let debounceTimer;
    $: $filters,
        (() => {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                fetchProducts();
            }, 500);
        })();

    onMount(() => {
        fetchProducts();
    });
    
    // para recibir el quantity del modal o usar 1 por defecto
    function cotizar(producto){
        cart.update(items => {
            const quantityToAdd = producto.quantity ?? 1; // Usar la cantidad si existe, sino 1
            const existing = items.find(p => 
                p.idProducto === producto.idProducto && 
                p.idProveedor === producto.proveedor.idProveedor
            );
            
            if(existing){
                return items.map(p => 
                    (p.idProducto === producto.idProducto && p.idProveedor === producto.proveedor.idProveedor) 
                    ? {...p, quantity: p.quantity + quantityToAdd} 
                    : p
                );
            } else {
                return [...items, {
                    ...producto, 
                    idProveedor: producto.proveedor.idProveedor,
                    nombreProveedor: producto.proveedor.nombreProveedor,
                    quantity: quantityToAdd
                }];
            }
        });
    }

    // función para abrir el modal de cotización
    function openCotizacionModal(product) {
        productoBaseParaCotizar = {
            idProducto: product.idProducto,
            nombreProducto: product.nombreProducto,
            principioActivo: product.principioActivo 
            // solo se pasan los datos necesarios para buscar similares
        };
        mostrarCotizacionModal = true;
    }
</script>

{#if errMessage}
    <div class="alert alert-danger" role="alert">
        {errMessage}
    </div>
{:else if products.length === 0}
    <div class="alert alert-info" role="alert">
        No se encontraron productos que coincidan con los criterios de búsqueda.
    </div>

{:else}
    <div class="product-grid">
        {#each products as product}
            {#if product.proveedores?.some(prov => prov.stock > 0)}
                <div class="product-card">
                    <div class="card-image-wrapper">
                        <img 
                            src={product.imagenUrl || 'https://via.placeholder.com/300x200?text=Sin+Imagen'} 
                            alt={product.nombreProducto} 
                            class="product-image" 
                        />
                    </div>

                    <div class="card-content">
                        <h3 class="product-name" title={product.nombreProducto}>
                            {product.nombreProducto}
                        </h3>
                        <p class="product-principio">{product.principioActivo}</p>

                        <div class="product-meta">
                            <span class="meta-label">Laboratorio:</span>
                            <span class="meta-value">{product.laboratorioFabricante}</span>
                        </div>
                        <div class="product-meta">
                            <span class="meta-label">Presentación:</span>
                            <span class="meta-value">{product.presentacionComercial}</span>
                        </div>
                        <div class="product-meta">
                            <span class="meta-label">Vencimiento:</span>
                            <span class="meta-value">
                                {product.fechaVencimiento ? new Date(product.fechaVencimiento).toLocaleDateString() : 'N/A'}
                            </span>
                        </div>
                        
                        {#if product.proveedores?.length > 0}
                            <div class="provider-options-block">
                                <h4 class="provider-title">Opciones de Compra:</h4>
                                <div class="supplier-list">
                                    {#each product.proveedores.filter(prov => prov.stock > 0) as prov (prov.idProveedor)}
                                        <div class="supplier-option">
                                            <div class="supplier-info">
                                                <p class="supplier-name">{prov.nombreProveedor}</p>
                                                <p class="supplier-stock">Stock: <span class="stock-value">{prov.stock}</span></p>
                                            </div>
                                            <div class="supplier-action">
                                                <span class="supplier-price">
                                                    ${prov.precio?.toFixed ? prov.precio.toFixed(2) : prov.precio}
                                                </span>
                                                <button 
                                                    class="btn-add-to-cart" 
                                                    on:click={() => cotizar({ 
                                                        idProducto: product.idProducto, 
                                                        name: product.nombreProducto, 
                                                        price: prov.precio, 
                                                        proveedor: { idProveedor: prov.idProveedor, nombreProveedor: prov.nombreProveedor } 
                                                    })}
                                                >
                                                    + Añadir
                                                </button>
                                            </div>
                                        </div>
                                    {/each}
                                </div>
                            </div>
                        {:else}
                            <p class="text-danger my-2 fw-semibold mt-auto">Sin stock disponible.</p>
                        {/if}

                        <div class="card-footer-action">
                             <button
                                type="button"
                                class="btn-cotizar-similares"
                                on:click={() => openCotizacionModal(product)} 
                            >
                                🔍 Cotizar Similares
                            </button>
                            <button
                                type="button"
                                class="btn-toggle-details"
                                on:click={() => toggleExpand(product)}
                                aria-expanded={expanded.has(getId(product))}
                            >
                                {#if expanded.has(getId(product))}
                                    <span class="icon">▲</span> Ver menos detalles
                                {:else}
                                    <span class="icon">▼</span> Ver más detalles
                                {/if}
                            </button>
                        </div>
                        
                        {#if expanded.has(getId(product))}
                            <div class="expanded-details">
                                <p class="detail-item"><strong>Concentración:</strong> {product.concentracion}</p>
                                <p class="detail-item"><strong>Forma Farmacéutica:</strong> {product.formaFarmaceutica}</p>
                                <p class="detail-item"><strong>Registro Sanitario:</strong> {product.registroSanitario}</p>
                                <p class="detail-item"><strong>Almacenamiento:</strong> {product.condicionesAlmacenamiento}</p>
                            </div>
                        {/if}
                    </div>
                </div>
            {/if}
        {/each}
    </div>
{/if}

{#if mostrarCotizacionModal && productoBaseParaCotizar}
    <CotizacionModal 
        productoBase={productoBaseParaCotizar}
        onAddToCart={cotizar}
        on:close={() => mostrarCotizacionModal = false}
    />
{/if}
