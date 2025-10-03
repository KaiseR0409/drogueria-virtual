<script>
    import { onMount, onDestroy } from "svelte";
    import ProductModal from "./ProductModal.svelte";
    // Estado reactivo de los productos
    let productos = [];
    let loading = true;
    let isModalOpen = false;
    let selectedProduct = null; // Producto seleccionado para editar
    let _popstateHandler = null;
    let fetchError = null;

    function fetchProductos() {
        loading = true;
        fetchError = null;
        const idProveedor = localStorage.getItem("idProveedor");

        if (!idProveedor) {
            alert("No se encontró al proveedor, por favor inicie sesion.");
            loading = false;
            return;
        }
        fetch(`http://localhost:5029/api/proveedor/${idProveedor}/productos`)
            .then((res) => res.json())
            .then((data) => {
                productos = data;
                loading = false;
            })
            .catch((err) => {
                console.error("Error al cargar productos:", err);
                loading = false;
            });
    }

    function obtenerClaseStock(stock) {
        if (stock <= 30) {
            return "stock-bajo"; // Rojo: Menor o igual a 30
        } else if (stock >= 31 && stock <= 80) {
            return "stock-medio"; // Naranja/Amarillo: 31 a 80
        } else {
            return "stock-alto"; // Verde: Mayor o igual a 81
        }
    }

    function agregarProducto() {
        console.log("agregarProducto llamado"); // debug
        selectedProduct = null; // Modo Creación
        isModalOpen = true;
        console.log("isModalOpen ->", isModalOpen); // debug
        // Abrir como pantalla completa y añadir entrada al historial
        isModalOpen = true;
        history.pushState({ productScreen: true }, "", "#producto");
        _popstateHandler = () => {
            isModalOpen = false;
            selectedProduct = null;
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
        };
        window.addEventListener("popstate", _popstateHandler);
        console.log("isModalOpen ->", isModalOpen); // debug
    }
    function handleClose() {
        isModalOpen = false;
        fetchProductos(); // Refresca la lista tras cerrar el modal
        // Si abrimos vía pushState, retrocedemos para activar popstate y restaurar URL
        if (_popstateHandler) {
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
            history.back();
        } else {
            isModalOpen = false;
        }
        fetchProductos(); // Refresca la lista tras cerrar la pantalla
    }

    onDestroy(() => {
        if (_popstateHandler) {
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
        }
    });

    function handleCreate() {
        selectedProduct = null;
        isModalOpen = true;
    }
    //borrar productos
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
    function handleEdit(e) {
        selectedProduct = e;
        isModalOpen = true;
        console.log("Editing product:", selectedProduct);
    }
    function handleSuccess() {
        isModalOpen = false;
        fetchProductos(); // Refresca la lista tras cerrar el modal
    }

    onMount(() => {
        fetchProductos();
    });
</script>

{#if isModalOpen}
    <div class="modal-overlay">
        <div
            class="modal"
            role="dialog"
            aria-modal="true"
            aria-label="Formulario de producto"
        >
            <div class="modal-header">
                <h3 class="modal-title">
                    {selectedProduct
                        ? "Editar Producto"
                        : "Publicar Nuevo Producto"}
                </h3>
                <button
                    class="modal-close"
                    type="button"
                    on:click={handleClose}
                    aria-label="Cerrar">×</button
                >
            </div>

            <div class="modal-body">
                <ProductModal
                    product={selectedProduct}
                    sucess={handleSuccess}
                    close={handleClose}
                />
            </div>

            <div class="modal-actions">
                <button
                    class="btn btn-cancel"
                    type="button"
                    on:click={handleClose}>Cancelar</button
                >
            </div>
        </div>
    </div>
{/if}

<div class="dashboard-container">
    <div class="header">
        <h1>Inventario de Productos Farmacéuticos</h1>
        <button
            type="button"
            on:click={agregarProducto}
            class="btn btn-primary"
        >
            + Agregar Producto Nuevo
        </button>
    </div>

    {#if loading}
        <div class="spinner-border" role="status">
            <span class="sr-only"></span>
        </div>
        <p class="loading-state">Cargando inventario...</p>
    {:else if productos.length === 0}
        <p class="empty-state">
            No hay productos en su inventario. ¡Agregue el primero!
        </p>
    {:else}
        <div class="table-wrapper">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nombre</th>
                        <th>Descripción</th>
                        <th>Precio</th>
                        <th>Stock</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    {#each productos as inv (inv.idProducto)}
                        <tr>
                            <td>{inv.producto.idProducto}</td>
                            <td class="product-name"
                                >{inv.producto.nombreProducto}</td
                            >
                            <td class="product-desc"
                                >{inv.producto.principioActivo} - {inv.producto
                                    .presentacionComercial}</td
                            >
                            <td>${inv.precio.toFixed(2)}</td>
                            <td>
                                <span 
                                    class:stock-bajo={inv.stock <= 30}
                                    class:stock-medio={inv.stock >= 31 && inv.stock <= 80}
                                    class:stock-alto={inv.stock >= 81}
                                >
                                    {inv.stock}
                                </span>
                            </td>
                            <td class="actions-cell">
                                <button
                                    on:click={() => handleEdit(inv)}
                                    class="btn btn-icon btn-edit"
                                >
                                    &#9998;
                                </button>
                                <button
                                    on:click={() =>
                                        handleDelete(inv.producto.idProducto)}
                                    class="btn btn-icon btn-delete"
                                >
                                    &times;
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>
