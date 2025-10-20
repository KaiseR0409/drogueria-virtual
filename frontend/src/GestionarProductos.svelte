<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import * as XLSX from 'xlsx';
    import ProductModal from "./ProductModal.svelte";
    import { checkAuth } from './auth.js';

    checkAuth({ rolRequerido: 'Proveedor' });

    // --- Variables de Estado ---
    let productos: any[] = [];
    let loading = true;
    let fetchError: string | null = null;

    // --- Variables de Carga Masiva ---
    let archivoSeleccionado: File;
    let mensajeCargaMasiva = '';

    // --- Estado del modal y producto seleccionado ---
    let isModalOpen = false;
    let selectedProduct: any = null;
    let _popstateHandler: (() => void) | null = null;

    // --- Variables de Filtro y Paginación ---
    let filtroNombre = "";
    let filtroID = "";
    let filtroStock = "";
    const itemsPerPage = 8;
    let currentPage = 1;
    let idProveedor; 

    // --- FUNCIONES ---


    // Función principal para obtener datos
    function fetchProductos() {
        loading = true;
        fetchError = null;
        
        idProveedor = localStorage.getItem("idUsuario");
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

    // --- Funciones de Carga Masiva ---
    function descargarPlantillaJSON() {
        const plantilla = [{
            "nombreProducto": "Ejemplo: Paracetamol 500mg",
            "principioActivo": "Paracetamol",
            "concentracion": "500mg",
            "formaFarmaceutica": "Comprimido",
            "presentacionComercial": "Caja x 20 comprimidos",
            "laboratorioFabricante": "Laboratorio Ejemplo S.A.",
            "registroSanitario": "INVIMA-12345",
            "fechaVencimiento": "2026-12-31T00:00:00",
            "condicionesAlmacenamiento": "Lugar fresco y seco, menor a 30°C",
            "imagenUrl": "https://ejemplo.com/imagen.png",
            "marca": "MarcaGenérica",
            "codigoBarras": "7701234567890",
            "precio": 5.99,
            "stock": 150
        }];
        const jsonString = JSON.stringify(plantilla, null, 4);
        const blob = new Blob([jsonString], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'plantilla_productos.json';
        a.click();
        URL.revokeObjectURL(url);
    }

    async function subirArchivo() {
        if (!archivoSeleccionado) {
            alert("Por favor, selecciona un archivo.");
            return;
        }
        mensajeCargaMasiva = 'Procesando archivo...';

        const procesarYEnviar = async (productosJSON: any[]) => {
            try {
                if (!Array.isArray(productosJSON) || productosJSON.length === 0) {
                    throw new Error("El archivo no contiene una lista de productos válida.");
                }
                const idProveedor = localStorage.getItem('idProveedor');
                const token = localStorage.getItem('token');
                if (!idProveedor || !token) throw new Error("Sesión de proveedor no válida.");

                const response = await fetch(`http://localhost:5029/api/proveedor/${idProveedor}/carga-masiva`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                    body: JSON.stringify(productosJSON)
                });
                const resultado = await response.json();
                if (!response.ok) throw new Error(resultado.mensaje || "Error en el servidor.");
                mensajeCargaMasiva = resultado.mensaje;
                fetchProductos();
            } catch (err) {
                mensajeCargaMasiva = `Error: ${(err as Error).message}`;
            }
        };

        const reader = new FileReader();
        if (archivoSeleccionado.name.endsWith('.json')) {
            reader.onload = (event) => procesarYEnviar(JSON.parse(event.target.result as string));
            reader.readAsText(archivoSeleccionado);
        } else if (archivoSeleccionado.name.endsWith('.xlsx') || archivoSeleccionado.name.endsWith('.xls')) {
            reader.onload = (event) => {
                try {
                    const data = new Uint8Array(event.target.result as ArrayBuffer);
                    const workbook = XLSX.read(data, { type: 'array' });
                    const sheetName = workbook.SheetNames[0];
                    const worksheet = workbook.Sheets[sheetName];
                    const productosJSON = XLSX.utils.sheet_to_json(worksheet, {
                        raw: false,
                        dateNF: 'yyyy-mm-dd'
                    });
                    procesarYEnviar(productosJSON);
                } catch (err) {
                     mensajeCargaMasiva = `Error al leer el archivo Excel: ${(err as Error).message}`;
                }
            };
            reader.readAsArrayBuffer(archivoSeleccionado);
        } else {
            mensajeCargaMasiva = "Error: Formato de archivo no soportado. Sube un .json o .xlsx.";
        }
    }
    
    // --- Manejadores del Modal y otras funciones de acción ---
    function openModal(product = null) {
        selectedProduct = product;
        isModalOpen = true;
        history.pushState({ productScreen: true }, "", "#producto");
        _popstateHandler = () => {
            handleClose(false);
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
        };
        window.addEventListener("popstate", _popstateHandler);
    }
    
    function handleClose(shouldGoBack = true) {
        isModalOpen = false;
        fetchProductos(); 
        if (_popstateHandler && shouldGoBack) {
            window.removeEventListener("popstate", _popstateHandler);
            _popstateHandler = null;
            history.back();
        }
    }

    function handleEdit(inv: any) {
        openModal(inv);
    }
    
    async function handleDelete(idProducto: number) {
        if (confirm("¿Está seguro de eliminar este producto?")) {
            const idProveedor = localStorage.getItem("idProveedor");
            const token = localStorage.getItem("token"); // Se necesita token para borrar
            try {
                const res = await fetch(
                    `http://localhost:5029/api/proveedor/${idProveedor}/inventario/${idProducto}`,
                    { 
                        method: "DELETE",
                        headers: { 'Authorization': `Bearer ${token}` }
                    },
                );
                if (!res.ok) throw new Error("Error al eliminar el producto");
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
    
    function goToPage(page: number) {
        if (page >= 1 && page <= totalPages) {
            currentPage = page;
        }
    }
    
    // --- Lógica de Filtro y Paginación Reactiva ---
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
        .sort((a, b) => (a.producto.nombreProducto || '').localeCompare(b.producto.nombreProducto || ''));
    
    $: totalPages = Math.ceil(productosFiltrados.length / itemsPerPage);
    $: startIndex = (currentPage - 1) * itemsPerPage;
    $: endIndex = startIndex + itemsPerPage;
    $: productosPaginados = productosFiltrados.slice(startIndex, endIndex);
    $: productosFiltrados, (currentPage = 1);

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
        <div>
             <button type="button" on:click={() => openModal(null)} class="btn btn-primary">
                + Agregar Producto Nuevo
            </button>
        </div>
    </div>

    <div class="card mt-4 p-3">
        <h4>Carga Masiva de Productos</h4>
        <p>Sube un archivo JSON o Excel. Las cabeceras del Excel deben coincidir con los campos de la plantilla.</p>
        
        <div class="btn-group mb-3">
            <button on:click={descargarPlantillaJSON} class="btn btn-secondary">
                📥 Descargar Plantilla JSON
            </button>
            <a href="/plantilla_productos.xlsx" download class="btn btn-success">
                📥 Descargar Plantilla Excel
            </a>
        </div>

        <div class="input-group">
            <input 
                type="file" 
                class="form-control" 
                id="file-upload"
                accept=".json, .xlsx, .xls"
                on:change={(e) => archivoSeleccionado = (e.target as HTMLInputElement).files[0]}
            >
            <button on:click={subirArchivo} class="btn btn-primary">
                🚀 Cargar desde Archivo
            </button>
        </div>
        
        {#if mensajeCargaMasiva}
            <p class="mt-2" style="color: {mensajeCargaMasiva.startsWith('Error') ? 'red' : 'green'};">
                {mensajeCargaMasiva}
            </p>
        {/if}
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
            <table class="tabla-usuarios"> 
                <thead>
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
                                    &#9998; 
                                </button>
                                <button
                                    on:click={() => handleDelete(inv.producto.idProducto)}
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