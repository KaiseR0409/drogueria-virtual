<script>
  import jsPDF from "jspdf";
  import autoTable from "jspdf-autotable";

  // Recibe el objeto factura desde el backend
  export let factura;

  function generarPDF() {
    if (!factura) return;

    const doc = new jsPDF();
    const margenIzq = 14;
    let y = 20;

    // --- ENCABEZADO ---
    doc.setFontSize(16);
    doc.setFont("helvetica", "bold");
    doc.text("FACTURA ELECTRÓNICA", margenIzq, y);
    y += 10;

    doc.setFontSize(12);
    doc.setFont("helvetica", "normal");
    doc.text("Droguería Virtual - Sistema de Ventas", margenIzq, y);
    y += 10;
    doc.text(
      "Fecha Emisión: " +
        new Date(
          factura.fechaFactura ?? factura.fechaOrden,
        ).toLocaleDateString(),
      margenIzq,
      y,
    );
    y += 8;
    doc.text(
      "Número de Factura: " + (factura.numeroFactura || "SIN NUMERO"),
      margenIzq,
      y,
    );
    y += 8;
    doc.text("Número de Orden: " + factura.idOrden, margenIzq, y);
    y += 8;
    doc.text("Estado: " + (factura.estadoOrden || "Pagada"), margenIzq, y);
    y += 10;

    // --- DATOS DEL CLIENTE ---
    doc.setFont("helvetica", "bold");
    doc.text("Datos del Cliente", margenIzq, y);
    doc.setFont("helvetica", "normal");
    y += 6;
    doc.text(
      "Nombre: " + (factura.cliente?.nombreUsuario || "Cliente Final"),
      margenIzq,
      y,
    );
    y += 6;
    doc.text(
      "Correo: " + (factura.cliente?.correo || "No especificado"),
      margenIzq,
      y,
    );
    y += 6;
    doc.text(
      "Dirección de Envío: " +
        (factura.cliente?.direccionEnvioCompleta || "Sin dirección"),
      margenIzq,
      y,
    );
    y += 10;

    // --- DATOS DEL PROVEEDOR ---
    doc.setFont("helvetica", "bold");
    doc.text("Datos del Proveedor", margenIzq, y);
    doc.setFont("helvetica", "normal");
    y += 6;
    doc.text(
      "Nombre Comercial: " +
        (factura.proveedor?.nombreProveedor || "Proveedor Desconocido"),
      margenIzq,
      y,
    );
    y += 6;
    doc.text("RUT: " + (factura.proveedor?.rut || "N/D"), margenIzq, y);
    y += 6;
    doc.text(
      "Giro: " + (factura.proveedor?.giro || "Sin giro registrado"),
      margenIzq,
      y,
    );
    y += 6;
    doc.text(
      "Dirección: " +
        (factura.proveedor?.direccionComercial || "No registrada"),
      margenIzq,
      y,
    );
    y += 6;
    doc.text("Ciudad: " + (factura.proveedor?.ciudad || "N/D"), margenIzq, y);
    y += 10;

    // --- TABLA DE PRODUCTOS ---
    const rows = (factura.items ?? []).map((item) => [
      item.nombreProducto ?? "Producto",
      item.cantidad,
      "$" + item.precioUnitario.toFixed(0),
      "$" + (item.precioUnitario * item.cantidad).toFixed(0),
    ]);

    autoTable(doc, {
      startY: y,
      head: [["Producto", "Cantidad", "Precio Unitario", "Subtotal"]],
      body: rows,
      styles: { fontSize: 10 },
      headStyles: { fillColor: [40, 100, 200] },
    });

    const finalY = doc.lastAutoTable.finalY + 10;

    // --- TOTALES ---
    doc.setFontSize(12);
    doc.setFont("helvetica", "bold");
    doc.text(
      "Subtotal: $" +
        (factura.montoTotal - (factura.impuestos || 0)).toFixed(0),
      margenIzq,
      finalY,
    );
    doc.text(
      "Impuestos: $" + (factura.impuestos || 0).toFixed(0),
      margenIzq,
      finalY + 6,
    );
    doc.text(
      "Descuento: $" + (factura.descuento || 0).toFixed(0),
      margenIzq,
      finalY + 12,
    );
    doc.text(
      "Total: $" + factura.montoTotal.toFixed(0),
      margenIzq,
      finalY + 18,
    );
    doc.text(
      "Método de Pago: " + (factura.metodoPago || "No especificado"),
      margenIzq,
      finalY + 24,
    );

    // --- PIE ---
    doc.setFontSize(10);
    doc.setFont("helvetica", "italic");
    doc.text("Gracias por su compra.", margenIzq, finalY + 40);

    // Descargar PDF
    doc.save("Factura_" + (factura.numeroFactura || factura.idOrden) + ".pdf");
  }
</script>

<button on:click={generarPDF} class="btn btn-primary">
  💾 Descargar Factura PDF
</button>
