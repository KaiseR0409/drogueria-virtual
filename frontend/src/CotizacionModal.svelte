<script>
    import { onMount } from 'svelte';
    import SimilarProductCard from './SimilarProductCard.svelte';
    import { createEventDispatcher } from 'svelte';

    export let productoBase; 
    export let onAddToCart; 
    
    const dispatch = createEventDispatcher();
    const close = () => dispatch('close');

    let productosSimilares = [];
    let isLoading = true;
    let fetchError = false;

    // Función para obtener productos similares de la API
    async function fetchProductosSimilares(baseProduct) {
        isLoading = true;
        fetchError = false;
        productosSimilares = [];

        try {
            const params = new URLSearchParams();
            params.append("idProductoExcluir", baseProduct.idProducto);

            // Prioriza el Principio Activo, sino usa el Nombre del Producto.
            if (baseProduct.principioActivo) {
                params.append("principioActivo", baseProduct.principioActivo);
            } else if (baseProduct.nombreProducto) {
                params.append("nombreProducto", baseProduct.nombreProducto);
            } else {
                isLoading = false;
                return;
            }
            
            const apiUrl = `http://localhost:5029/api/proveedor/BuscarSimilares?${params.toString()}`;

            const res = await fetch(apiUrl);
            if (!res.ok) {
                throw new Error(`Error ${res.status}: No se pudieron obtener los productos similares.`);
            }
            
            const data = await res.json();
            productosSimilares = Array.isArray(data) ? data : []; 

        } catch (error) {
            console.error("Error al cargar productos similares:", error);
            fetchError = true;
        } finally {
            isLoading = false;
        }
    }
    
    onMount(() => {
        fetchProductosSimilares(productoBase);
    });
    
    // Función que llama al handler del padre (función `cotizar` en Productos.svelte)
    function handleAddToCart(item) {
        onAddToCart(item); 
    }
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="cotizacion-modal-overlay" on:click={close}>
    <div class="cotizacion-modal-content" on:click|stopPropagation>
        
        <header class="modal-header">
            <h2 class="modal-title">
                Cotizaciones y Alternativas: 
                <span class="product-base-name">{productoBase.nombreProducto}</span>
            </h2>
            <button class="close-button" on:click={close}>&times;</button>
        </header>

        <div class="modal-body">
            {#if isLoading}
                <p class="text-center loading-message">Cargando productos similares...</p>
            {:else if fetchError}
                <p class="text-center error-message">Error al cargar las alternativas. Intente más tarde.</p>
            {:else if productosSimilares.length > 0}
                <p class="suggestions-count">Se encontraron {productosSimilares.length} alternativas de diferentes proveedores y presentaciones:</p>
                
                <div class="similar-products-grid">
                    {#each productosSimilares as product (product.idProducto + '-' + product.idProveedor)}
                        <SimilarProductCard 
                            product={product} 
                            on:addtocart={(e) => handleAddToCart(e.detail)}
                        />
                    {/each}
                </div>
            {:else}
                <p class="no-suggestions">No se encontraron cotizaciones ni productos similares con stock.</p>
            {/if}
        </div>

        <footer class="modal-footer">
            <button class="btn btn-close-footer" on:click={close}>Cerrar Ventana</button>
        </footer>
    </div>
</div>
