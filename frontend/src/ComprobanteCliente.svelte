<script>
  import { createEventDispatcher } from 'svelte';
  import jsPDF from "jspdf";
  import "jspdf-autotable";
  import { applyPlugin } from 'jspdf-autotable'; // Importar applyPlugin

  applyPlugin(jsPDF); // Aplicar el plugin a jsPDF

  export let ordenes = [];
  const dispatch = createEventDispatcher();

  function generarPDF() {
    const doc = new jsPDF();

    doc.setFontSize(16);
    doc.text("Comprobante de Pago - Cliente", 14, 20);

    ordenes.forEach((orden, idx) => {
      doc.setFontSize(12);
      doc.text(`Orden #${idx + 1} - Proveedor: ${orden.nombreProveedor ?? orden.idProveedor}`, 14, 30 + (idx * 70));
      doc.text(`Factura: ${orden.numeroFactura}`, 14, 36 + (idx * 70));
      doc.text(`Fecha: ${new Date(orden.fechaFactura).toLocaleDateString()}`, 14, 42 + (idx * 70));
      doc.text(`Total: $${orden.montoTotal.toFixed(2)}`, 14, 48 + (idx * 70));

      const rows = (orden.items ?? []).map(it => [
        it.idProducto,
        it.cantidad,
        `$${it.precioUnitario.toFixed(2)}`,
        `$${(it.cantidad * it.precioUnitario).toFixed(2)}`
      ]);

      doc.autoTable({
        head: [["Producto", "Cantidad", "P. Unitario", "Subtotal"]],
        body: rows,
        startY: 54 + (idx * 70),
        theme: "grid",
        styles: { fontSize: 9 }
      });

      if (idx < ordenes.length - 1) doc.addPage();
    });

    doc.save("comprobante_cliente.pdf");
  }
</script>
<div class="modal-overlay">
  <div class="modal-content">
    <h2>Compra realizada con éxito</h2>
    <p>Se han generado {ordenes.length} órdenes.</p>

    <button on:click={generarPDF} class="btn btn-download">Descargar comprobante</button>
    <button on:click={() => dispatch('close')} class="btn btn-close">Cerrar</button>
  </div>
</div>

