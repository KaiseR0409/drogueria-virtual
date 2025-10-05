<script>
    import { onMount } from "svelte";
    import FacturaPreview from "./FacturaPreview.svelte";

    let facturas = [];
    let idProveedor = localStorage.getItem("idProveedor");

    // Variables de Filtro
    let filtroEstado = "Todos"; 
    let filtroFactura = "";
    let filtroOrden = "";
    let filtroFecha = "";
    let filtroMonto = "";
    let filtroIDUsuario = ""; 

    let cargando = true;
    let error = null;

    // Opciones de estado
    const estadosFactura = ["Todos", "Pagada", "Pendiente"];

    // Lógica de Paginación
    const itemsPerPage = 8; // Define cuántas filas por página
    let currentPage = 1;
    
    // Función para cambiar de página
    function goToPage(page) {
        if (page >= 1 && page <= totalPages) {
            currentPage = page;
        }
    }
    
    // Resetear la página a 1 cuando los filtros cambian (para evitar páginas vacías)
    $: facturasFiltradas, (currentPage = 1);
    
    // Calcular el número total de páginas reactivamente
    $: totalPages = Math.ceil(facturasFiltradas.length / itemsPerPage);

    // Calcular el índice de inicio y fin para el slice
    $: startIndex = (currentPage - 1) * itemsPerPage;
    $: endIndex = startIndex + itemsPerPage;

    // Obtener las facturas de la página actual
    $: facturasPaginadas = facturasFiltradas.slice(startIndex, endIndex);
    // ----------------------------

    async function cargarFacturas() {
        if (!idProveedor) return;

        try {
            cargando = true;
            error = null;
            // Endpoint de la API
            const res = await fetch(`http://localhost:5029/api/Orden/mis-facturas/${idProveedor}`);
            
            if (!res.ok) {
                console.error("Error al cargar facturas:", res.status);
                error = `Error ${res.status} al cargar las facturas.`;
                return;
            }
            
            facturas = await res.json();
        } catch (err) {
            console.error("Error en fetch:", err);
            error = "No se pudo conectar con el servidor API.";
        } finally {
            cargando = false;
        }
    }

    // Función para formatear fechas
    function formatearFecha(fechaCompleta) {
        if (!fechaCompleta) return "N/A";
        try {
            // Obtener solo la parte de la fecha
            return fechaCompleta.substring(0, 10);
        } catch (e) {
            return "Fecha inválida";
        }
    }

    // Lógica de Filtro combinada y ordenamiento
    $: facturasFiltradas = facturas
        .filter(f => {
            const fechaFmt = formatearFecha(f.fechaOrden);

            // Filtrar por Estado
            const cumpleEstado = filtroEstado === "Todos" || f.estadoOrden === filtroEstado;

            // Filtrar por Número de Factura
            const cumpleFactura = f.numeroFactura
                ?.toLowerCase()
                .includes(filtroFactura.toLowerCase());

            // Filtrar por ID de Orden
            const cumpleOrden = f.idOrden
                ?.toString()
                .includes(filtroOrden.toLowerCase());

            // Filtrar por ID de Usuario
            const cumpleIDUsuario = f.idUsuario
                ?.toString()
                .includes(filtroIDUsuario.toLowerCase());
            
            // Filtrar por Fecha
            const cumpleFecha = fechaFmt.includes(filtroFecha);

            // Filtrar por Monto Total
            const cumpleMonto = f.montoTotal
                ?.toString()
                .includes(filtroMonto.toLowerCase());

            return (
                cumpleEstado &&
                cumpleFactura &&
                cumpleOrden &&
                cumpleIDUsuario &&
                cumpleFecha &&
                cumpleMonto
            );
        })
        .sort((a, b) => new Date(b.fechaOrden) - new Date(a.fechaOrden)); // Ordenar por fecha descendente

    onMount(() => {
        cargarFacturas();
    });
</script>
<div class="dashboard-container">
    <div class="header">
        <h2>🧾 Mis Facturas (Proveedor: {idProveedor})</h2>
    </div>

    <div class="filtros-card">
        <p class="filtros-titulo">Buscar y Filtrar Facturas</p>
        <div class="controles-grid-facturas">
            <input
                type="text"
                placeholder="🔍 Factura (Número)"
                bind:value={filtroFactura}
                class="form-control control-filtro"
            />
            <input
                type="text"
                placeholder="🔍 Orden (ID)"
                bind:value={filtroOrden}
                class="form-control control-filtro"
            />
            <input
                type="text"
                placeholder="🔍 ID Usuario"
                bind:value={filtroIDUsuario}
                class="form-control control-filtro"
            />
            
            <input
                type="text"
                placeholder="🔍 Fecha (YYYY-MM-DD)"
                bind:value={filtroFecha}
                class="form-control control-filtro"
            />
            <input
                type="text"
                placeholder="🔍 Monto"
                bind:value={filtroMonto}
                class="form-control control-filtro"
            />
            <select bind:value={filtroEstado} class="form-control control-filtro">
                {#each estadosFactura as estado}
                    <option value={estado}>{estado}</option>
                {/each}
            </select>
        </div>
    </div>

    {#if cargando}
        <p class="loading-state">Cargando facturas...</p>
    {:else if error}
        <p class="error-message">⚠️ {error}</p>
    {:else if facturasFiltradas.length === 0}
        <p class="empty-state">
            No se encontraron facturas que coincidan con los filtros.
        </p>
    {:else}
        <div class="tabla-wrapper">
            <table class="tabla-usuarios"> 
                <thead>
                    <tr>
                        <th class="col-factura">FACTURA</th>
                        <th class="col-orden">ORDEN</th>
                        <th class="col-usuario">USUARIO (ID)</th>
                        <th class="col-fecha">FECHA</th>
                        <th class="col-monto">MONTO</th>
                        <th class="col-estado">ESTADO</th>
                        <th class="col-acciones">ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    {#each facturasPaginadas as f (f.idOrden)} 
                        <tr>
                            <td class="factura-celda">{f.numeroFactura}</td>
                            <td>{f.idOrden}</td>
                            <td>ID {f.idUsuario}</td>
                            <td>{formatearFecha(f.fechaOrden)}</td>
                            <td class="monto-celda">${f.montoTotal.toFixed(2)}</td> 
                            <td>
                                <span class="badge badge-estado-{f.estadoOrden.toLowerCase()}">
                                    {f.estadoOrden}
                                </span>
                            </td>
                            <td class="acciones-celda">
                                <FacturaPreview {f} factura={f} />
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