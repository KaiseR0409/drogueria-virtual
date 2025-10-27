<script lang="ts">
    import jsPDF from "jspdf";
    import autoTable from "jspdf-autotable";

    export let ordenes = [];

    //Traer detalles completos antes de generar PDF
    async function obtenerDetallesOrdenes() {
        const completas = [];

        for (const orden of ordenes) {
            try {
                const res = await fetch(`http://localhost:5029/api/Orden/detalle/${orden.idOrden}`);
                if (res.ok) {
                    const data = await res.json();
                    console.log("Detalles orden obtenidos:", data);
                    completas.push(data);
                } else {
                    completas.push(orden); // fallback por si falla
                }
            } catch {
                completas.push(orden);
            }
        }
        return completas;

    }

    async function descargarPDF() {
        const ordenesCompletas = await obtenerDetallesOrdenes();

        const pdf = new jsPDF({ orientation: 'p', unit: 'mm', format: 'a4' });

        ordenesCompletas.forEach((orden, index) => {
            if (index > 0) pdf.addPage();

            const fecha = new Date(orden.fechaOrden).toLocaleString();
            const clienteNombre = orden.cliente?.nombre || "Cliente Final";
            const clienteEmail = orden.cliente?.correo || "-";
            const direccionEnvio = orden.cliente?.direccionEnvioCompleta || "No especificada";
            const proveedorNombre = orden.proveedor?.nombreProveedor || "Proveedor Desconocido";
            const rutProveedor = orden.proveedor?.rut || "N/A";
            const numeroFactura = orden.numeroFactura || `TEMP-${orden.idOrden}`;

            //  Título
            pdf.setFontSize(26);
            pdf.setTextColor(200, 0, 0);
            pdf.text("FARMACIAS FIM", 20, 20);

            //  Proveedor
            pdf.setFontSize(14);
            pdf.setTextColor(0, 0, 0);
            pdf.text(proveedorNombre, 20, 30);
            pdf.setFontSize(11);
            pdf.text(`Giro: ${orden.proveedor?.giro || "DISTRIBUCION DE PRODUCTOS FARMACEUTICOS"}`, 20, 38);
            pdf.text(`Dirección: ${orden.proveedor?.direccionComercial || "No especificada"}`, 20, 44);
            pdf.text(`Ciudad: ${orden.proveedor?.ciudad || "No especificada"}`, 20, 50);

            //  RUT + documento
            pdf.setFontSize(12);
            pdf.text(`RUT: ${rutProveedor}`, 150, 20);
            pdf.setFontSize(12);
            pdf.text("FACTURA ELECTRÓNICA", 150, 28);
            pdf.setFontSize(14);
            pdf.text(numeroFactura, 150, 38);

            //  Info general
            pdf.setFontSize(11);
            pdf.text(`Fecha emisión: ${fecha}`, 20, 65);
            pdf.text(`N° Pedido: ${orden.idOrden}`, 20, 72);

            //  Cliente
            pdf.setFontSize(13);
            pdf.setTextColor(200, 0, 0);
            pdf.text("Datos del Cliente", 20, 90);
            pdf.setFontSize(11);
            pdf.setTextColor(0, 0, 0);
            pdf.text(`Nombre: ${clienteNombre}`, 20, 98);
            pdf.text(`Email: ${clienteEmail}`, 20, 104);
            pdf.text(`Dirección de envío: ${direccionEnvio}`, 20, 110);

            //  Productos
            const items = (orden.items || []).map(item => [
                item.nombreProducto || "Producto",
                item.cantidad || 0,
                `$${(Number(item.precioUnitario) || 0).toLocaleString("es-CL")}`,
                `$${(Number(item.subtotal) || (item.cantidad * item.precioUnitario) || 0).toLocaleString("es-CL")}`
            ]);

            autoTable(pdf, {
                startY: 125,
                head: [["Descripción", "Cantidad", "Precio Unit.", "Total"]],
                body: items,
                theme: "grid",
                headStyles: {
                    fillColor: [200, 0, 0],
                    textColor: [255, 255, 255],
                    fontStyle: "bold",
                    halign: "center",
                },
                styles: {
                    fontSize: 10,
                    halign: "center",
                    cellPadding: 3,
                }
            });

            // Pago y totales
            let finalY = (pdf as any).lastAutoTable.finalY + 15;
            pdf.setFontSize(11);
            pdf.setTextColor(0, 0, 0);
            pdf.text(`Método de pago: ${orden.metodoPago || "Tarjeta"}`, 20, finalY);

            const subtotal = Number(orden.subtotal || 0);
            const iva = Number(orden.iva || subtotal * 0.19);
            const total = Number(orden.total || subtotal + iva);

            pdf.setFontSize(12);
            pdf.text(`Subtotal: $${subtotal.toLocaleString("es-CL")}`, 140, finalY);
            pdf.text(`IVA (19%): $${iva.toLocaleString("es-CL")}`, 140, finalY + 7);
            pdf.setFontSize(14);
            pdf.setTextColor(200, 0, 0);
            pdf.text(`TOTAL: $${total.toLocaleString("es-CL")}`, 140, finalY + 16);
        });

        pdf.save(`comprobante-compra-${Date.now()}.pdf`);
    }
</script>

<div class="comprobante-container">
    <h3>✅ Compra realizada con éxito</h3>
    <p>Se generaron {ordenes.length} órdenes de compra.</p>

    <button class="btn-descargar" on:click={descargarPDF}>
        📥 Descargar Comprobante PDF
    </button>
</div>
