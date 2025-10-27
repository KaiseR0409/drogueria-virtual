<script lang="ts">
  import jsPDF from "jspdf";
  import html2canvas from "html2canvas";

  export let factura;
  let cargando = false;

  async function generarPDF() {
    cargando = true;
    let facturaCompleta = factura;

    try {
      // Traer detalles completos si faltan
      if (!factura.items || !factura.proveedor?.direccionComercial) {
        const res = await fetch(`http://localhost:5029/api/Orden/${factura.idOrden}`);
        if (res.ok) facturaCompleta = await res.json();
      }

      // Crear contenedor invisible
      const temp = document.createElement("div");
      temp.style.position = "absolute";
      temp.style.left = "-9999px";
      temp.innerHTML = generarHTMLFactura(facturaCompleta);
      document.body.appendChild(temp);

      // Capturar en canvas
      const canvas = await html2canvas(temp, { scale: 2 });
      const imgData = canvas.toDataURL("image/png");

      const pdf = new jsPDF("p", "mm", "a4");
      const pdfWidth = pdf.internal.pageSize.getWidth();
      const pdfHeight = (canvas.height * pdfWidth) / canvas.width;

      pdf.addImage(imgData, "PNG", 0, 0, pdfWidth, pdfHeight);
      pdf.save(`FACTURA_${facturaCompleta.numeroFactura || facturaCompleta.idOrden}.pdf`);

      document.body.removeChild(temp);
    } catch (err) {
      console.error("Error generando factura PDF:", err);
    } finally {
      cargando = false;
    }
  }

  function generarHTMLFactura(f) {
    const subtotal = f.items?.reduce((acc, i) => acc + i.cantidad * i.precioUnitario, 0) || 0;
    const iva = subtotal * 0.19;
    const total = subtotal + iva;

    const fecha = new Date(f.fechaFactura ?? f.fechaOrden);
    const fechaFormateada = fecha.toLocaleDateString("es-CL");
    const horaFormateada = fecha.toLocaleTimeString("es-CL", { hour12: false });

    return `
      <div style="width:210mm;padding:20px;font-family:Arial,sans-serif;font-size:12px;color:#333;text-transform:uppercase;">
        <header style="display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:25px;">
          <div>
            <h1 style="color:#cc0000;margin:0 0 10px 0;font-size:48px;font-weight:bold;">Farmacias FIM </h1>
            <h2 style="margin:0 0 5px 0;font-size:16px;font-weight:bold;">${f.proveedor?.nombreProveedor || "PROVEEDOR"}</h2>
            <p style="margin:3px 0;font-size:11px;">GIRO: ${f.proveedor?.giro || "SIN GIRO"}</p>
            <p style="margin:3px 0;font-size:11px;">DIRECCIÓN: ${f.proveedor?.direccionComercial || "NO REGISTRADA"}</p>
            <p style="margin:3px 0;font-size:11px;">CIUDAD: ${f.proveedor?.ciudad || "N/D"}</p>
          </div>
          <div style="border:2px solid #cc0000;padding:10px;text-align:center;min-width:200px;">
            <p style="margin:0 0 8px 0;font-weight:bold;font-size:14px;">RUT: ${f.proveedor?.rut || "N/D"}</p>
            <h3 style="margin:0 0 8px 0;padding:5px;font-size:14px;border:1px solid #333;">FACTURA ELECTRÓNICA</h3>
            <p style="margin:0;font-size:18px;font-weight:bold;">${f.numeroFactura || f.idOrden}</p>
          </div>
        </header>

        <section style="border-top:2px solid #000;border-bottom:2px solid #000;padding:10px 0;margin-bottom:25px;">
          <table style="width:100%;border-collapse:collapse;font-size:12px;">
            <tbody>
              <tr>
                <td style="padding:3px 5px;width:140px;"><b>CLIENTE:</b></td>
                <td style="padding:3px 5px;"> ${f.cliente?.nombreUsuario || "CLIENTE"}</td>
                <td style="padding:3px 5px;width:140px;"><b>FECHA EMISIÓN:</b></td>
                <td style="padding:3px 5px;width:150px;">${fechaFormateada} ${horaFormateada}</td>
              </tr>
              <tr>
                <td style="padding:3px 5px;"><b>DIRECCIÓN:</b></td>
                <td style="padding:3px 5px;" colspan="3">${f.cliente?.direccionEnvioCompleta || "SIN DIRECCIÓN"}</td>
              </tr>
            </tbody>
          </table>
        </section>

        <section>
          <table style="width:100%;border-collapse:collapse;">
            <thead style="background-color:#eee;">
              <tr>
                <th style="padding:8px;border:1px solid #000;text-align:left;">DESCRIPCIÓN</th>
                <th style="padding:8px;border:1px solid #000;">CANTIDAD</th>
                <th style="padding:8px;border:1px solid #000;">PRECIO UNIT.</th>
                <th style="padding:8px;border:1px solid #000;">TOTAL</th>
              </tr>
            </thead>
            <tbody>
              ${(f.items || [])
                .map(
                  (item) => `
                  <tr style="border-bottom:1px solid #ccc;">
                    <td style="padding:8px;border:1px solid #000;">${item.nombreProducto}</td>
                    <td style="padding:8px;border:1px solid #000;text-align:center;">${item.cantidad}</td>
                    <td style="padding:8px;border:1px solid #000;text-align:right;">$${item.precioUnitario.toLocaleString("es-CL")}</td>
                    <td style="padding:8px;border:1px solid #000;text-align:right;">$${(item.cantidad * item.precioUnitario).toLocaleString("es-CL")}</td>
                  </tr>`
                )
                .join("")}
            </tbody>
          </table>
        </section>

        <footer style="margin-top:25px;display:flex;justify-content:space-between;align-items:flex-start;">
          <div>
            <p><b>MÉTODO DE PAGO:</b> ${f.metodoPago || "NO ESPECIFICADO"}</p>
          </div>
          <table style="width:280px;border-collapse:collapse;border:1px solid #000;font-size:13px;">
            <tr>
              <td style="padding:6px;font-weight:bold;text-align:right;">SUBTOTAL:</td>
              <td style="padding:6px;text-align:right;">$${subtotal.toLocaleString("es-CL")}</td>
            </tr>
            <tr>
              <td style="padding:6px;font-weight:bold;text-align:right;">IVA (19%):</td>
              <td style="padding:6px;text-align:right;">$${iva.toLocaleString("es-CL")}</td>
            </tr>
            <tr>
              <td style="padding:6px;font-weight:bold;background-color:#eee;text-align:right;">TOTAL:</td>
              <td style="padding:6px;font-weight:bold;background-color:#eee;text-align:right;">$${total.toLocaleString("es-CL")}</td>
            </tr>
          </table>
        </footer>

      </div>
    `;
  }
</script>

<button on:click={generarPDF} class="btn-descargar" disabled={cargando}>
  {#if cargando}⏳ GENERANDO...{/if}
  {#if !cargando}📥 DESCARGAR FACTURA PDF{/if}
</button>
