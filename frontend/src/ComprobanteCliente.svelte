<script lang="ts">
    import { createEventDispatcher } from "svelte";
    import jsPDF from "jspdf";
    import html2canvas from "html2canvas";

    export let ordenes = [];
    const dispatch = createEventDispatcher();

    let comprobanteElement: HTMLElement;

    function handleClose() {
        dispatch("close");
    }

    async function descargarPDF() {
        if (!comprobanteElement) return;
        const canvas = await html2canvas(comprobanteElement, { scale: 2 });
        const imgData = canvas.toDataURL('image/png');
        const pdf = new jsPDF({ orientation: 'p', unit: 'mm', format: 'a4' });
        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (canvas.height * pdfWidth) / canvas.width;
        
        pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
        pdf.save(`resumen-compra-${Date.now()}.pdf`);
    }

    const datosComprador = {
        nombre: localStorage.getItem('nombreUsuario') || 'Cliente Final',
    };
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="modal-overlay" on:click={handleClose}>
    <div class="modal-content-comprobante" on:click|stopPropagation>
        <div class="modal-header-elegant">
            <h3 class="modal-title">¡Compra Realizada con Éxito! 🎉</h3>
            <button class="close-btn" on:click={handleClose}>&times;</button>
        </div>

        <div class="modal-body-content">
            <p class="summary-text">
                Se generaron <strong class="count">{ordenes.length}</strong> órden{ordenes.length === 1 ? "" : "es"} de compra.
            </p>

            <div class="comprobante-scroll-container">
                <div bind:this={comprobanteElement}>
                    {#each ordenes as orden (orden.idOrden)}
                        <div class="pagina-pedido">
                            <header class="header">
                                <div class="info-vendedor">
                                    <h3>{orden.nombreProveedor || 'FARMACEUTICA REDFARMA LTDA.'}</h3>
                                    <p>GIRO: DISTRIBUCION DE PRODUCTOS FARMACEUTICOS</p>
                                </div>
                                <div class="info-documento">
                                    <h3>NOTA DE PEDIDO</h3>
                                    <p class="numero-pedido">{orden.idOrden}</p>
                                </div>
                            </header>

                            <section class="seccion-cliente">
                                <p><strong>RAZÓN SOCIAL:</strong> {datosComprador.nombre}</p>
                            </section>
                            
                            <section class="seccion-items">
                                <table>
                                    <thead>
                                        <tr>
                                            <th class="descripcion">DESCRIPCIÓN</th>
                                            <th>CANTIDAD</th>
                                            <th>PRECIO</th>
                                            <th>TOTAL</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {#if orden.items}
                                            {#each orden.items as item}
                                                <tr>
                                                    <td class="descripcion">{item.name || item.nombreProducto || 'Producto Desconocido'}</td>
                                                    <td>{item.quantity || item.cantidad}</td>
                                                    <td>${(item.price || item.precioUnitario).toFixed(0)}</td>
                                                    <td>${((item.quantity || item.cantidad) * (item.price || item.precioUnitario)).toFixed(0)}</td>
                                                </tr>
                                            {/each}
                                        {/if}
                                    </tbody>
                                </table>
                            </section>

                            <footer class="footer">
                                <div class="espacio-blanco"></div>
                                <div class="totales">
                                    <div class="fila total">
                                        <span>TOTAL:</span> 
                                        <span>${orden.montoTotal.toFixed(0)}</span>
                                    </div>
                                </div>
                            </footer>
                        </div>
                    {/each}
                </div>
            </div>

            <div class="action-buttons-group">
                <button on:click={descargarPDF} class="btn btn-download-elegant">
                    ⬇️ Descargar Comprobante PDF
                </button>
                <button on:click={handleClose} class="btn btn-close-elegant">
                    Cerrar
                </button>
            </div>
        </div>
    </div>
</div>