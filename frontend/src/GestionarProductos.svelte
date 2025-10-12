<script>
    import { onMount, onDestroy } from "svelte";
    import ProductModal from "./ProductModal.svelte"; // Componente ProductModal
    import { checkAuth } from './auth.js';

    checkAuth({ rolRequerido: 'Proveedor' });

    // Estado reactivo de los productos
    let productos = [];
    let loading = true;
    let fetchError = null;

    // Estado del modal y producto seleccionado
    let isModalOpen = false;
    let selectedProduct = null; // Producto seleccionado para editar/crear
    let _popstateHandler = null; // Manejador de historial para cerrar modal

    // Variables de Filtro
    let filtroNombre = "";
    let filtroID = "";
    let filtroStock = "";
    
    //  Lógica de Paginación
    const itemsPerPage = 8; // Filas por página
    let currentPage = 1;

    // --- Funciones de Lógica ---

    // Función principal para obtener datos
    function fetchProductos() {
        loading = true;
        fetchError = null;
        const idProveedor = localStorage.getItem("idProveedor");

        if (!idProveedor) {
            fetchError = "No se encontró al proveedor. Por favor, inicie sesión.";
            loading = false;
            return;
        }
        
        // Llamada a la API
        fetch(`http://localhost:5029/api/proveedor/${idProveedor}/productos`)
            .then((res) => {
                if (!res.ok) throw new Error(`Error al cargar: ${res.status}`);
                return res.json();
            })
            .then((data) => {
                productos = data;
                loading = false;
            })
            .catch((err) => {
                console.error("Error al cargar productos:", err);
                fetchError = "Error al obtener productos del servidor.";
                loading = false;
            });
    }

    // Manejadores del Modal y navegación
    function openModal(product = null) {
        selectedProduct = product; // null para crear, objeto para editar
        isModalOpen = true;
        // Lógica de historial para navegación tipo pantalla completa
        history.pushState({ productScreen: true }, "", "#producto");
        _popstateHandler = () => {
            handleClose(false); // No refrescar si es solo el popstate
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
        };
        window.addEventListener("popstate", _popstateHandler);
    }
    
    function handleClose(shouldGoBack = true) {
        isModalOpen = false;
        // Refresca la lista tras cerrar el modal
        fetchProductos(); 
        
        // Si no se cerró con el botón 'atrás' del navegador
        if (_popstateHandler && shouldGoBack) {
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
            history.back(); // Vuelve atrás para limpiar el #producto de la URL
        }
    }

    // Funciones de acción
    function handleEdit(inv) {
        openModal(inv);
    }
    
    async function handleDelete(idProducto) {
        if (confirm("¿Está seguro de eliminar este producto?")) {
            const idProveedor = localStorage.getItem("idProveedor");

            try {
                const res = await fetch(
                    `http://localhost:5029/api/proveedor/${idProveedor}/inventario/${idProducto}`,
                    { method: "DELETE" },
                );

                if (!res.ok) throw new Error("Error al eliminar el producto");

                // Quitar del array en memoria
                productos = productos.filter(
                    (inv) => inv.producto.idProducto !== idProducto,
                );

                alert("Producto eliminado exitosamente.");
            } catch (err) {
                console.error(err);
                alert("No se pudo eliminar el producto.");
            }
        }
    }
    
    // --- Lógica de Filtro y Paginación ---

    // Filtrado Reactivo
    $: productosFiltrados = productos
        .filter(p => {
            const nombre = p.producto?.nombreProducto?.toLowerCase() || '';
            const id = p.producto?.idProducto?.toString() || '';
            const stock = p.stock?.toString() || '';

            const cumpleNombre = nombre.includes(filtroNombre.toLowerCase());
            const cumpleID = id.includes(filtroID);
            const cumpleStock = stock.includes(filtroStock);

            return cumpleNombre && cumpleID && cumpleStock;
        })
        .sort((a, b) => (a.producto.nombreProducto || '').localeCompare(b.producto.nombreProducto || '')); // Ordenar alfabéticamente por nombre
    
    // Paginación Reactiva
    $: totalPages = Math.ceil(productosFiltrados.length / itemsPerPage);
    $: startIndex = (currentPage - 1) * itemsPerPage;
    $: endIndex = startIndex + itemsPerPage;
    $: productosPaginados = productosFiltrados.slice(startIndex, endIndex);

    // Resetear la página al filtrar
    $: productosFiltrados, (currentPage = 1);

    function goToPage(page) {
        if (page >= 1 && page <= totalPages) {
            currentPage = page;
        }
    }
    
    // --- Ciclo de Vida ---
    onMount(() => {
        fetchProductos();
    });

    onDestroy(() => {
        if (_popstateHandler) {
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
        }
    });
</script>


{#if isModalOpen}
    <div class="modal-overlay">
        <div class="modal" role="dialog" aria-modal="true" aria-label="Formulario de producto">
            <div class="modal-header">
                <h3 class="modal-title">
                    {selectedProduct ? "Editar Producto" : "Publicar Nuevo Producto"}
                </h3>
                <button
                    class="modal-close"
                    type="button"
                    on:click={() => handleClose(true)}
                    aria-label="Cerrar"
                >&times;</button>
            </div>

            <div class="modal-body">
                <ProductModal
                    product={selectedProduct}
                    sucess={fetchProductos}
                    close={() => handleClose(true)}
                />
            </div>
        </div>
    </div>
{/if}

<div class="dashboard-container">
    <div class="header">
        <h1>Inventario de Productos Farmacéuticos</h1>
        <button
            type="button"
            on:click={() => openModal(null)}
            class="btn btn-primary"
        >
            + Agregar Producto Nuevo
        </button>
    </div>
    
    <div class="filtros-card">
        <p class="filtros-titulo">Buscar y Filtrar Productos</p>
        <div class="controles-grid-productos">
            <input
                type="text"
                placeholder="🔍 Nombre del Producto"
                bind:value={filtroNombre}
                class="form-control control-filtro"
            />
            <input
                type="text"
                placeholder="🔍 ID"
                bind:value={filtroID}
                class="form-control control-filtro"
            />
            <input
                type="text"
                placeholder="🔍 Stock"
                bind:value={filtroStock}
                class="form-control control-filtro"
            />
        </div>
    </div>

    {#if loading}
        <p class="loading-state">Cargando inventario...</p>
    {:else if fetchError}
        <p class="error-message">⚠️ {fetchError}</p>
    {:else if productosFiltrados.length === 0}
        <p class="empty-state">
            No hay productos que coincidan con los filtros.
        </p>
    {:else}
        <div class="tabla-wrapper">
            <table class="tabla-usuarios"> <thead>
                    <tr>
                        <th style="width: 5%;">ID</th>
                        <th style="width: 25%;">NOMBRE</th>
                        <th style="width: 30%;">DESCRIPCIÓN</th>
                        <th style="width: 10%;">PRECIO</th>
                        <th style="width: 10%;">STOCK</th>
                        <th style="width: 10%;">ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    {#each productosPaginados as inv (inv.producto.idProducto)}
                        <tr>
                            <td>{inv.producto.idProducto}</td>
                            <td class="product-name">{inv.producto.nombreProducto}</td>
                            <td class="product-desc">
                                {inv.producto.principioActivo} - {inv.producto.presentacionComercial}
                            </td>
                            <td>${inv.precio.toFixed(2)}</td>
                            <td>
                                <span class="badge" 
                                    class:stock-bajo={inv.stock <= 30}
                                    class:stock-medio={inv.stock >= 31 && inv.stock <= 80}
                                    class:stock-alto={inv.stock > 80}
                                >
                                    {inv.stock}
                                </span>
                            </td>
                            <td class="actions-cell">
                                <button
                                    on:click={() => handleEdit(inv)}
                                    class="btn btn-icon btn-edit"
                                >
                                    &#9998; </button>
                                <button
                                    on:click={() => handleDelete(inv.producto.idProducto)}
                                    class="btn btn-icon btn-delete"
                                >
                                    &times; </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if totalPages > 1}
            <div class="pagination-controls">
                <button
                    class="btn btn-secondary btn-pagination"
                    on:click={() => goToPage(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    &laquo; Anterior
                </button>

                <span class="page-info">
                    Página {currentPage} de {totalPages}
                </span>

                <button
                    class="btn btn-secondary btn-pagination"
                    on:click={() => goToPage(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    Siguiente &raquo;
                </button>
            </div>
        {/if}
    {/if}
</div>