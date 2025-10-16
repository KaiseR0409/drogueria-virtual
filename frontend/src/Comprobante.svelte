<script lang="ts">
    import jsPDF from 'jspdf';
    import html2canvas from 'html2canvas';

    // Este componente espera recibir el objeto 'orden' completo
    export let orden: any;

    let comprobanteElement: HTMLElement; // Elemento HTML que se convertirá a PDF

    // Función para descargar el comprobante como PDF
    async function descargarPDF() {
        if (!comprobanteElement) return;

        const canvas = await html2canvas(comprobanteElement, { scale: 2 });
        const imgData = canvas.toDataURL('image/png');
        
        const pdf = new jsPDF({
            orientation: 'p',
            unit: 'mm',
            format: 'a4'
        });

        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (canvas.height * pdfWidth) / canvas.width;
        
        pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
        pdf.save(`nota-pedido-${orden.idOrden}.pdf`);
    }

    $: console.log('Orden recibida en Comprobante.svelte:', orden);
    $: console.log('Items de la orden:', orden?.items);
    $: subtotal = orden?.items.reduce((acc: number, item: any) => acc + (item.cantidad * item.precioUnitario), 0) || 0; // Cálculo del subtotal 
    $: iva = (orden?.montoTotal || 0) - subtotal;
</script>

{#if orden && orden.items}
    <div class="actions-bar">
        <button class="btn-descargar" on:click={descargarPDF}>📥 Descargar como PDF</button>
    </div>

    <div class="comprobante" bind:this={comprobanteElement}>
        <header class="header">
            <div class="info-vendedor">
                <h3>{orden.proveedor?.nombreProveedor || 'Nombre del Vendedor'}</h3>
                <p>GIRO: DISTRIBUCION DE PRODUCTOS FARMACEUTICOS</p>
                <p>DIRECCION: AVENIDA COLON N 9765 B-10, HUALPEN, CHILE</p>
            </div>
            <div class="info-documento">
                <p class="rut">RUT: {orden.proveedor?.rutProveedor || 'RUT del proveedor'}</p>
                <h3>NOTA DE PEDIDO</h3>
                <p class="numero-pedido">{orden.idOrden}</p>
            </div>
        </header>

        <section class="seccion-cliente">
            <table>
                <tbody>
                    <tr>
                        <td><strong>RAZÓN SOCIAL</strong></td>
                        <td>{orden.usuario?.nombreUsuario || 'Nombre del Cliente'}</td>
                    </tr>
                </tbody>
            </table>
        </section>

        <section class="seccion-pedido">
            <table>
                <thead>
                    <tr>
                        <th>PEDIDO</th>
                        <th>CLIENTE</th>
                        <th>FECHA EMISIÓN</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{orden.idOrden}</td>
                        <td>{orden.usuario?.nombreUsuario}</td>
                        <td>{new Date(orden.fechaOrden).toLocaleDateString()}</td>
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
                        <th>PRECIO</th>
                        <th>TOTAL</th>
                    </tr>
                </thead>
                <tbody>
                    {#each orden.items as item}
                        <tr>
                            <td class="descripcion">{item.nombreProducto}</td>
                            <td>{item.cantidad}</td>
                            <td>${item.precioUnitario?.toFixed(2) || '0.00'}</td>
                            <td>${(item.cantidad * item.precioUnitario)?.toFixed(2) || '0.00'}</td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </section>

        <footer class="footer">
            <div class="espacio-blanco"></div>
            <div class="totales">
                <div class="fila"><span>SUBTOTAL:</span> <span>${subtotal?.toFixed(2) || '0.00'}</span></div>
                <div class="fila"><span>I.V.A.:</span> <span>${iva?.toFixed(2) || '0.00'}</span></div>
                <div class="fila total"><span>TOTAL:</span> <span>${orden.montoTotal?.toFixed(2) || '0.00'}</span></div>
            </div>
        </footer>
    </div>
{:else}
    <p>Cargando datos del comprobante...</p>
{/if}
