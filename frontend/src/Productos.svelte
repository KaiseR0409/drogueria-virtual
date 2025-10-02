<script>
    import { onMount } from "svelte";
    // import Filtros from "./Filtros.svelte";
    import { cart, filters } from "./stores.js";
    let products = [];
    let errMessage = "";

    // conjunto de ids de tarjetas expandidas
    let expanded = new Set();

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

    //funcion traer productos
    async function fetchProducts() {
        try {
            // 🎯 SIMPRE USAMOS EL ENDPOINT GLOBAL PARA EL CATÁLOGO 🎯
            let apiUrl = `http://localhost:5029/api/ProveedorProducto/InventarioCompletoConFiltros`;

            // Adjuntamos los filtros
            const params = new URLSearchParams();
            $filters.laboratoriosSeleccionados.forEach((lab) =>
                params.append("laboratoriosSeleccionados", lab),
            );
            if ($filters.busquedaNombre)
                params.append("nombreProducto", $filters.busquedaNombre);
            if ($filters.busquedaPrincipioActivo)
                params.append(
                    "principioActivo",
                    $filters.busquedaPrincipioActivo,
                );
            if ($filters.busquedaLaboratorio)
                params.append(
                    "laboratorioFabricante",
                    $filters.busquedaLaboratorio,
                );
            if ($filters.busquedaFormaFarmaceutica)
                params.append(
                    "formaFarmaceutica",
                    $filters.busquedaFormaFarmaceutica,
                );

            apiUrl += `?${params.toString()}`;

            const res = await fetch(apiUrl);
            if (!res.ok)
                throw new Error(
                    `Error en la solicitud: ${res.status} ${res.statusText}`,
                );
            const data = await res.json();
            console.log("Datos recibidos de la API (Catálogo Global):", data);

            // === LÓGICA DE AGRUPACIÓN POR PRODUCTO (Múltiples Proveedores) ===

            if (Array.isArray(data) && data.length && data[0].producto) {
                const grouped = data.reduce((acc, item) => {
                    const prodId = item.idProducto;
                    if (!acc[prodId]) {
                        // Inicializamos el objeto del producto (datos farmacéuticos)
                        const p = item.producto || {};
                        acc[prodId] = {
                            idProducto: item.idProducto,
                            nombreProducto: p.nombreProducto ?? "",
                            principioActivo: p.principioActivo ?? "",
                            concentracion: p.concentracion ?? "",
                            formaFarmaceutica: p.formaFarmaceutica ?? "",
                            presentacionComercial:
                                p.presentacionComercial ?? "",
                            laboratorioFabricante:
                                p.laboratorioFabricante ?? "",
                            registroSanitario: p.registroSanitario ?? "",
                            fechaVencimiento: p.fechaVencimiento ?? null,
                            condicionesAlmacenamiento:
                                p.condicionesAlmacenamiento ?? "",
                            imagenUrl: p.imagenUrl ?? "",
                            // Lista de proveedores de este producto
                            proveedores: [],
                        };
                    }
                    // Añadimos la información del proveedor específico (precio, stock, nombre)
                    acc[prodId].proveedores.push({
                        idProveedor: item.idProveedor,
                        nombreProveedor:
                            item.proveedor?.nombreProveedor ||
                            `Proveedor #${item.idProveedor}`,
                        precio: item.precio ?? 0,
                        stock: item.stock ?? 0,
                        __raw: item,
                    });
                    return acc;
                }, {});

                // El array final es la lista de productos únicos con sus opciones de proveedor anidadas.
                products = Object.values(grouped);
            } else {
                // Si la API no retorna datos anidados o está vacía
                products = [];
            }

            console.log("Catálogo Global Cargado:", products);
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
    function cotizar(producto){
        cart.update(items => {
            const existing = items.find(p=> p.idProducto === producto.idProducto && p.idProveedor === producto.idProveedor);
            if(existing){
                return items.map(p => p.idProducto === producto.idProducto && p.idProveedor === producto.idProveedor ? {...p, quantity: p.quantity + 1} : p);

            }else{
                return [...items, {...producto, quantity: 1}];
            }
        });
    

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
    <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
        {#each products as product}
            <div class="col">
                <div class="card border border-primary shadow-0 h-100">
                    <div class="bg-image hover-overlay ripple" data-mdb-ripple-color="light">
                        <img 
                            src={product.imagenUrl || 'https://via.placeholder.com/300x180?text=Producto'} 
                            class="img-fluid" 
                            alt={product.nombreProducto} 
                        />
                        <a href="#!">
                            <div class="mask" style="background-color: rgba(251, 251, 251, 0.15)"></div>
                        </a>
                    </div>

                    <div class="card-header text-truncate" title={product.nombreProducto}>
                        {product.nombreProducto}
                    </div>

                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title text-dark">{product.principioActivo}</h5>
                        
                        <p class="card-text mb-2 text-muted small">
                            {product.presentacionComercial}<br>
                            Vencimiento: {product.fechaVencimiento ? new Date(product.fechaVencimiento).toLocaleDateString() : 'N/A'}
                        </p>

                        {#if product.proveedores?.length > 0}
                            <h6 class="mb-2 fw-bold text-primary">Opciones de Proveedor:</h6>
                            <div class="supplier-list flex-grow-1 overflow-auto mb-3">
                                {#each product.proveedores as prov (prov.idProveedor)}
                                    <div class="supplier-option d-flex justify-content-between align-items-center py-1 border-bottom">
                                        <div class="supplier-details">
                                            <p class="mb-0 small fw-semibold text-dark">{prov.nombreProveedor}</p>
                                            <p class="mb-0 tiny text-muted">Stock: {prov.stock}</p>
                                        </div>
                                        <div class="supplier-actions d-flex align-items-center">
                                            <p class="mb-0 fw-bold fs-6 me-2 text-success">
                                                ${prov.precio?.toFixed ? prov.precio.toFixed(2) : prov.precio}
                                            </p>
                                            <button 
                                                type="button" 
                                                class="btn btn-checkout btn-sm" 
                                                on:click={() => cotizar({ 
                                                    idProducto: product.idProducto, 
                                                    name: product.nombreProducto, 
                                                    price: prov.precio, 
                                                    proveedor: { idProveedor: prov.idProveedor, nombreProveedor: prov.nombreProveedor } 
                                                })}
                                            >
                                                Cotizar
                                            </button>
                                        </div>
                                    </div>
                                {/each}
                            </div>
                        {:else}
                            <p class="text-danger my-2 fw-semibold mt-auto">Sin proveedores o stock disponible.</p>
                        {/if}

                        <div class="mt-auto d-flex gap-2 justify-content-between align-items-center pt-2">
                            <button
                                type="button"
                                class="btn btn-outline-primary btn-sm flex-grow-1"
                                on:click={() => toggleExpand(product)}
                                aria-expanded={expanded.has(getId(product))}
                            >
                                {#if expanded.has(getId(product))}Ver menos{:else}Ver más{/if}
                            </button>
                            </div>

                        {#if expanded.has(getId(product))}
                            <div class="card-details mt-2 border-top pt-2">
                                <p class="mb-1"><strong>Concentración:</strong> {product.concentracion}</p>
                                <p class="mb-1"><strong>Forma Farmacéutica:</strong> {product.formaFarmaceutica}</p>
                                <p class="mb-1"><strong>Registro Sanitario:</strong> {product.registroSanitario}</p>
                                <p class="mb-1"><strong>Condiciones Almacenamiento:</strong> {product.condicionesAlmacenamiento}</p>
                                </div>
                        {/if}
                    </div>
                </div>
            </div>
        {/each}
    </div>
{/if}
