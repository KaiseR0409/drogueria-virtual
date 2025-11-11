<script lang="ts">
    import jsPDF from 'jspdf';
    import html2canvas from 'html2canvas';

    export let orden: any;
    let comprobanteElement: HTMLElement;

    async function descargarPDF() {
        if (!comprobanteElement) return;

        const canvas = await html2canvas(comprobanteElement, { scale: 2 });
        const imgData = canvas.toDataURL('image/png');
        const pdf = new jsPDF({ orientation: 'p', unit: 'mm', format: 'a4' });

        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (canvas.height * pdfWidth) / canvas.width;

        pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
        pdf.save(`comprobante-compra-${orden.idOrden}.pdf`);
    }

    // Cálculos base
    $: subtotal = orden?.items?.reduce(
        (acc: number, item: any) => acc + (parseFloat(item.precioUnitario) * item.cantidad),
        0
    ) || parseFloat(orden.subtotal) || 0;

    $: iva = parseFloat(orden.iva) || subtotal * 0.19;
    $: total = parseFloat(orden.total) || subtotal + iva;
</script>

{#if orden && orden.items}
    <div class="actions-bar">
        <button class="btn-descargar" on:click={descargarPDF}>
            📥 Descargar como PDF
        </button>
    </div>

    <div class="comprobante" bind:this={comprobanteElement}>
        <header class="header">
            <div class="info-vendedor">
                <h3>{orden.proveedor?.nombreProveedor || 'Nombre del Proveedor'}</h3>
                <p>GIRO: {orden.proveedor?.giro || 'Distribución de Productos Farmacéuticos'}</p>
                <p>DIRECCIÓN: {orden.proveedor?.direccionComercial || 'No registrada'}</p>
                <p>CIUDAD: {orden.proveedor?.ciudad || 'No registrada'}</p>
            </div>
            <div class="info-documento">
                <p class="rut">RUT: {orden.proveedor?.rut || 'RUT del proveedor'}</p>
                <h3>NOTA DE PEDIDO</h3>
                <p class="numero-pedido">{orden.idOrden}</p>
            </div>
        </header>

        <section class="seccion-cliente">
            <table>
                <tbody>
                    <tr>
                        <td><strong>RAZÓN SOCIAL:</strong></td>
                        <td>{orden.cliente?.nombreUsuario || 'N/A'}</td>
                    </tr>
                    <tr>
                        <td><strong>EMAIL:</strong></td>
                        <td>{orden.cliente?.correo || orden.usuario?.correo || 'No especificado'}</td>
                    </tr>
                    <tr>
                        <td><strong>DIRECCIÓN DE ENVÍO:</strong></td>
                        <td>{orden.direccionEnvioCompleta || orden.cliente?.direccionEnvioCompleta || 'No especificada'}</td>
                    </tr>
                </tbody>
            </table>
        </section>

        <section class="seccion-pedido">
            <table>
                <thead>
                    <tr>
                        <th>PEDIDO</th>
                        <th>FECHA EMISIÓN</th>
                        <th>MÉTODO DE PAGO</th>
                        <th>MONEDA</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{orden.idOrden}</td>
                        <td>{new Date(orden.fechaOrden).toLocaleDateString()}</td>
                        <td>{orden.metodoPago || 'Tarjeta'}</td>
                        <td>{orden.moneda || 'CLP'}</td>
                    </tr>
                </tbody>
            </table>
        </section>

        <section class="seccion-items">
            <table>
                <thead>
                    <tr>
                        <th class="descripcion">DESCRIPCIÓN</th>
                        <th>CANTIDAD</th>
                        <th>PRECIO UNIT.</th>
                        <th>TOTAL</th>
                    </tr>
                </thead>
                <tbody>
                    {#each orden.items as item}
                        <tr>
                            <td class="descripcion">{item.nombreProducto}</td>
                            <td>{item.cantidad}</td>
                            <td>${parseFloat(item.precioUnitario).toFixed(2)}</td>
                            <td>${(parseFloat(item.precioUnitario) * item.cantidad).toFixed(2)}</td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </section>

        <footer class="footer">
            <div class="espacio-blanco"></div>
            <div class="totales">
                <div class="fila">
                    <span>SUBTOTAL:</span>
                    <span>${subtotal.toFixed(2)}</span>
                </div>
                <div class="fila">
                    <span>I.V.A. (19%):</span>
                    <span>${iva.toFixed(2)}</span>
                </div>
                <div class="fila total">
                    <span>TOTAL (I.V.A INCLUIDO):</span>
                    <span>${total.toFixed(2)}</span>
                </div>
            </div>
        </footer>
    </div>
{:else}
    <p>Cargando datos del comprobante...</p>
{/if}
