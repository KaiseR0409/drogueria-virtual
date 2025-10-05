<script>
  import { createEventDispatcher } from "svelte";
  import jsPDF from "jspdf";
  import "jspdf-autotable";
  import { applyPlugin } from "jspdf-autotable";

  applyPlugin(jsPDF);

  export let ordenes = [];
  const dispatch = createEventDispatcher();

  // Función de cierre que despacha el evento 'close'
  function handleClose() {
    dispatch("close");
  }

  // Función de generación de PDF mejorada (sin cambios)
  function generarPDF() {
    const doc = new jsPDF();
    let finalY = 0;

    doc.setFontSize(22);
    doc.setTextColor(211, 47, 47);
    doc.text("Comprobante de Pago Electrónico", 105, 15, null, null, "center");

    doc.setFontSize(10);
    doc.setTextColor(150);
    doc.text(
      `Generado el: ${new Date().toLocaleString()}`,
      105,
      22,
      null,
      null,
      "center",
    );
    doc.line(10, 25, 200, 25);
    finalY = 30;

    ordenes.forEach((orden, idx) => {
      if (finalY + 10 > 280) {
        doc.addPage();
        finalY = 15;
      }

      doc.setFontSize(14);
      doc.setTextColor(51);
      doc.text(`Orden de Compra #${idx + 1}`, 14, finalY);
      finalY += 7;

      const metaData = [
        ["Proveedor", orden.nombreProveedor ?? `ID ${orden.idProveedor}`],
        ["N° Factura", orden.numeroFactura],
        ["Fecha Pedido", new Date(orden.fechaFactura).toLocaleDateString()],
        ["Método de Pago", orden.metodoPago],
        ["Monto Total", `$${orden.montoTotal.toFixed(2)}`],
      ];

      doc.autoTable({
        body: metaData,
        startY: finalY,
        theme: "plain",
        styles: { fontSize: 10, cellPadding: 2 },
        columnStyles: {
          0: { fontStyle: "bold", cellWidth: 35 },
          1: { cellWidth: 150 },
        },
        didDrawPage: (data) => {
          finalY = data.cursor.y + 5;
        },
      });

      finalY += 10;

      doc.setFontSize(12);
      doc.text(`Detalle de Items:`, 14, finalY);
      finalY += 5;

      const rows = (orden.items ?? []).map((it) => [
        it.idProducto,
        it.name ?? it.nombreProducto ?? "Producto Desconocido",
        it.cantidad,
        `$${it.precioUnitario.toFixed(2)}`,
        `$${(it.cantidad * it.precioUnitario).toFixed(2)}`,
      ]);

      doc.autoTable({
        head: [["ID", "Descripción", "Cant.", "P. Unitario", "Subtotal"]],
        body: rows,
        startY: finalY,
        theme: "striped",
        headStyles: { fillColor: [183, 28, 28] },
        styles: { fontSize: 10 },
        didDrawPage: (data) => {
          finalY = data.cursor.y + 10;
        },
      });

      finalY += 5;
    });

    doc.save(`comprobante_compra_${Date.now()}.pdf`);
  }
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_click_events_have_key_events -->
<div class="modal-overlay" on:click={handleClose}>
  <div class="modal-content-elegant" on:click|stopPropagation>
    <div class="modal-header-success">
      <h2 class="header-title">¡Compra realizada con éxito! 🎉</h2>
      <button class="close-btn" on:click={handleClose}>&times;</button>
    </div>

    <div class="modal-body-success">
      <p class="summary-text">
        Tus productos han sido procesados. Se generaron
        <strong class="count">{ordenes.length}</strong>
        órden{ordenes.length === 1 ? "" : "es"} de compra en total.
      </p>

      <div class="action-buttons-group">
        <button on:click={generarPDF} class="btn btn-download-elegant">
          ⬇️ Descargar Comprobante PDF
        </button>
        <button on:click={handleClose} class="btn btn-close-elegant">
          Cerrar
        </button>
      </div>
    </div>

    <div class="modal-footer-info">
      <p>El comprobante de envío para el transportista ya ha sido generado.</p>
    </div>
  </div>
</div>
