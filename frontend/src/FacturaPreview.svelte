<script>
  import jsPDF from "jspdf";
  import autoTable from "jspdf-autotable";

  export let factura;

  function generarPDF() {
    if (!factura) return;

    const doc = new jsPDF();

    // Encabezado
    doc.setFontSize(16);
    doc.text("Factura", 14, 20);
    doc.setFontSize(12);
    doc.text("Droguería Virtual", 14, 30);
    doc.text("Fecha Factura: " + new Date(factura.fechaFactura ?? factura.fechaOrden).toLocaleDateString(), 14, 38);

    // Datos principales
    doc.text("N° Factura: " + factura.numeroFactura, 14, 46);
    doc.text("ID Orden: " + factura.idOrden, 14, 52);
    doc.text("Estado: " + factura.estadoOrden, 14, 58);
    doc.text("Usuario (ID " + factura.idUsuario + ")", 14, 64);
    doc.text("Proveedor: " + factura.idProveedor, 14, 70);

    // Tabla productos
    const rows = (factura.items ?? []).map(item => [
      item.nombreProducto ?? "Producto",
      item.cantidad,
      "$" + (item.precioUnitario).toFixed(2),
      "$" + (item.precioUnitario * item.cantidad).toFixed(2)
    ]);

    autoTable(doc, {
      head: [["Producto", "Cantidad", "Precio Unitario", "Subtotal"]],
      body: rows,
      startY: 80,
      styles: { fontSize: 10 },
      headStyles: { fillColor: [200, 0, 0] } 
    });

    // Total
    let finalY = doc.lastAutoTable.finalY || 100;
    doc.setFontSize(14);
    doc.text("Total: $" + factura.montoTotal.toFixed(2), 14, finalY + 10);

    // Descargar
    doc.save("Factura_" + factura.numeroFactura + ".pdf");
  }
</script>

<button on:click={generarPDF} class="btn btn-primary">
  Descargar PDF
</button>
