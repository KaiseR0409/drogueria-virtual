<script lang="ts">
    import Comprobante from './Comprobante.svelte';
    import { onMount } from "svelte";
    import { checkAuth } from './auth.js';
    checkAuth();

    let historial = [];
    let idUsuario = localStorage.getItem("idUsuario");


    let ordenSeleccionada = null; 
    let cargandoDetalle = false; 
    let filtroEstado = "Todos"; 
    let filtroFactura = "";
    let filtroOrden = "";
    let filtroFecha = "";
    let filtroMonto = "";
    let filtroProveedor = ""; 

    let cargando = true;
    let error = null;

    const estadosOrden = ["Todos", "Pagada", "Pendiente"];

    const itemsPerPage = 8;
    let currentPage = 1;
    
    function goToPage(page) {
        if (page >= 1 && page <= totalPages) {
            currentPage = page;
        }
    }
    
    $: historialFiltrado, (currentPage = 1);
    $: totalPages = Math.ceil(historialFiltrado.length / itemsPerPage);
    $: startIndex = (currentPage - 1) * itemsPerPage;
    $: endIndex = startIndex + itemsPerPage;
    $: historialPaginado = historialFiltrado.slice(startIndex, endIndex);

    async function cargarHistorial() {
        if (!idUsuario) {
            error = "No se pudo identificar al usuario. Por favor, inicie sesión.";
            cargando = false;
            return;
        }

        try {
            cargando = true;
            error = null;
            const res = await fetch(`http://localhost:5029/api/Orden/mi-historial/${idUsuario}`);
            
            if (!res.ok) {
                const errData = await res.json();
                error = errData.mensaje || `Error ${res.status} al cargar el historial.`;
                return;
            }
            
            historial = await res.json();
        } catch (err) {
            error = "No se pudo conectar con el servidor API.";
        } finally {
            cargando = false;
        }
    }
    async function verDetalle(idOrden: number) {
        cargandoDetalle = true;
        ordenSeleccionada = {}; // Abre el modal en modo "cargando"

        const token = localStorage.getItem('token');
        try {
            const res = await fetch(`http://localhost:5029/api/Orden/${idOrden}`, {
                 headers: { 'Authorization': `Bearer ${token}` }
            });
            if (!res.ok) throw new Error("No se pudo cargar el detalle de la orden.");

            const data = await res.json();
            ordenSeleccionada = data; 
        } catch (err) {
            alert((err as Error).message);
            ordenSeleccionada = null; 
        } finally {
            cargandoDetalle = false;
        }
    }

    function cerrarModal() {
        ordenSeleccionada = null;
    }

    function formatearFecha(fechaCompleta) {
        if (!fechaCompleta) return "N/A";
        return fechaCompleta.substring(0, 10);
    }

    // Lógica de Filtro combinada y adaptada
    $: historialFiltrado = historial
        .filter(o => {
            const fechaFmt = formatearFecha(o.fechaOrden);

            const cumpleEstado = filtroEstado === "Todos" || o.estadoOrden === filtroEstado;
            const cumpleFactura = o.numeroFactura?.toLowerCase().includes(filtroFactura.toLowerCase());
            const cumpleOrden = o.idOrden?.toString().includes(filtroOrden.toLowerCase());
            const cumpleProveedor = o.nombreProveedor?.toLowerCase().includes(filtroProveedor.toLowerCase()); // Lógica de filtro nueva
            const cumpleFecha = fechaFmt.includes(filtroFecha);
            const cumpleMonto = o.montoTotal?.toString().includes(filtroMonto.toLowerCase());

            return (
                cumpleEstado &&
                cumpleFactura &&
                cumpleOrden &&
                cumpleProveedor &&
                cumpleFecha &&
                cumpleMonto
            );
        })
        .sort((a, b) => new Date(b.fechaOrden) - new Date(a.fechaOrden));

    onMount(cargarHistorial);
    console.log("Orden cargada para detalle:", ordenSeleccionada);
</script>

{#if ordenSeleccionada}
    <!-- svelte-ignore a11y_click_events_have_key_events -->
    <!-- svelte-ignore a11y_no_static_element_interactions -->
    <div class="modal-overlay" on:click={cerrarModal}>
        <div class="modal-content" on:click|stopPropagation>
            <button class="modal-close" on:click={cerrarModal}>&times;</button>
            {#if cargandoDetalle}
                <p>Cargando comprobante...</p>
            {:else}
                <Comprobante orden={ordenSeleccionada} />
            {/if}
        </div>
    </div>
{/if}

<div class="dashboard-container">
    <div class="header">
        <h2>🛒 Mi Historial de Compras</h2>
    </div>

    <div class="filtros-card">
        <p class="filtros-titulo">Buscar y Filtrar Compras</p>
        <div class="controles-grid-facturas">
            <input type="text" placeholder="🔍 Factura (Número)" bind:value={filtroFactura} class="form-control control-filtro" />
            <input type="text" placeholder="🔍 Orden (ID)" bind:value={filtroOrden} class="form-control control-filtro" />
            <input type="text" placeholder="🔍 Proveedor" bind:value={filtroProveedor} class="form-control control-filtro" />
            <input type="text" placeholder="🔍 Fecha (YYYY-MM-DD)" bind:value={filtroFecha} class="form-control control-filtro" />
            <input type="text" placeholder="🔍 Monto" bind:value={filtroMonto} class="form-control control-filtro" />
            <select bind:value={filtroEstado} class="form-control control-filtro">
                {#each estadosOrden as estado}
                    <option value={estado}>{estado}</option>
                {/each}
            </select>
        </div>
    </div>

    {#if cargando}
        <p class="loading-state">Cargando historial...</p>
    {:else if error}
        <p class="error-message">⚠️ {error}</p>
    {:else if historialFiltrado.length === 0}
        <p class="empty-state">No se encontraron compras que coincidan con los filtros.</p>
    {:else}
        <div class="tabla-wrapper">
            <table class="tabla-usuarios">
                <thead>
                    <tr>
                        <th class="col-orden">ORDEN</th>
                        <th class="col-factura">FACTURA</th>
                        <th class="col-usuario">PROVEEDOR</th> <th class="col-fecha">FECHA</th>
                        <th class="col-monto">MONTO</th>
                        <th class="col-estado">ESTADO</th>
                        <th class="col-acciones">ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    {#each historialPaginado as orden (orden.idOrden)}
                        <tr>
                            <td>{orden.idOrden}</td>
                            <td class="factura-celda">{orden.numeroFactura || 'N/A'}</td>
                            <td>{orden.nombreProveedor}</td> <td>{formatearFecha(orden.fechaOrden)}</td>
                            <td class="monto-celda">${orden.montoTotal.toFixed(2)}</td>
                            <td>
                                <span class="badge badge-estado-{orden.estadoOrden.toLowerCase()}">
                                    {orden.estadoOrden}
                                </span>
                            </td>
                            <td class="acciones-celda">
                               <button class="btn btn-primary" on:click={() => verDetalle(orden.idOrden)}>
                                    Ver Detalle
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if totalPages > 1}
            <div class="pagination-controls">
                </div>
        {/if}
    {/if}
</div>