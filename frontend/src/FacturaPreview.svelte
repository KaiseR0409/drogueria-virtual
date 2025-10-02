<script>
  import jsPDF from "jspdf";
  import autoTable from "jspdf-autotable";
  export let orden = null; // la orden viene desde el Carrito
  export let onClose; // callback para cerrar el modal
  let loading = false;

  function generarFacturaPDF() {
    const doc = new jsPDF();

    // Encabezado
    doc.setFontSize(16);
    doc.text("Factura Provisional", 14, 20);
    doc.setFontSize(12);
    doc.text("Droguería Virtual", 14, 30);
    doc.text("Fecha: " + new Date().toLocaleDateString(), 14, 38);

    // Tabla con productos
    const rows = orden.items.map(item => [
      item.nombreProducto ?? item.name ?? "Producto",
      item.cantidad ?? item.quantity,
      "$" + (item.precioUnitario ?? item.price).toFixed(2),
      "$" + ((item.precioUnitario ?? item.price) * (item.cantidad ?? item.quantity)).toFixed(2)
    ]);

    autoTable(doc, {
      head: [["Producto", "Cantidad", "Precio Unitario", "Subtotal"]],
      body: rows,
      startY: 50
    });

    // Total
    let finalY = doc.lastAutoTable.finalY || 80;
    doc.setFontSize(14);
    doc.text("Total: $" + orden.montoTotal.toFixed(2), 14, finalY + 10);

    // Guardar
    doc.save("Factura_" + orden.numeroFactura + ".pdf");
  }

  //esto va para crear la factura para el proveedor
  /*
  let facturaGenerada = null; // orden creada y pasada a FacturaPreview
// luego en Carrito.svelte, después de crear la orden exitosamente:
  facturaGenerada = {
            numeroFactura: "TEMP-12345",
            idProveedor: 16,
            montoTotal: 50000,
            items: get(cart) // o los items reales de la respuesta
          };
  */
</script>

{#if orden}
<div class="modal-backdrop">
  <div class="modal">
    <h2>Factura provisional lista</h2>
    <p><strong>N° Factura:</strong> {orden.numeroFactura}</p>
    <p><strong>Proveedor:</strong> {orden.idProveedor}</p>
    <p><strong>Total:</strong> ${orden.montoTotal.toFixed(2)}</p>

    <div class="acciones">
      <button on:click={generarFacturaPDF} class="btn btn-primary">Generar PDF</button>
      <button on:click={onClose} class="btn btn-secondary">Cerrar</button>
    </div>
  </div>
</div>
{/if}

