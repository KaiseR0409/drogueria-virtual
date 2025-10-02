<script>
	import ComprobanteCliente from './ComprobanteCliente.svelte';
  import { jsPDF } from "jspdf";
  import { get } from "svelte/store";
  import { cart, subtotal, totalItems } from "./stores.js";

  let ordenesConfirmadas = []; // orden creada y pasada a ComprobanteCliente
  let mostrarComprobante = false;
  const resultados = [];
  

  // token e idUsuario desde localStorage
  const token = localStorage.getItem("token");
  const idUsuario = localStorage.getItem("idUsuario");

  function generarNumeroOrdenFactura() {
    return "TEMP-" + Math.floor(Math.random() * 100000);
  }
  // Agrupa items por proveedor y crea una orden por proveedor en la API
  async function finalizarCompra() {
    const carrito = get(cart);

    if (!carrito || carrito.length === 0) {
      alert("El carrito está vacío.");
      return;
    }

    // Agrupar por idProveedor (compatible con diferentes formas de la estructura)
    const grupos = carrito.reduce((acc, item) => {
      const prov =
        item.idProveedor ??
        item.proveedor?.idProveedor ??
        item.proveedor?.id ??
        null;

      if (prov == null) {
        console.warn("Item sin idProveedor detectado:", item);
        return acc;
      }

      const key = String(prov);
      if (!acc[key]) acc[key] = [];
      acc[key].push(item);
      return acc;
    }, {});

    

    // Para cada proveedor, enviar POST /api/orden
    for (const [idProveedor, itemsProveedor] of Object.entries(grupos)) {
      // Construir items en el formato que espera la API
      const items = itemsProveedor.map((i) => ({
        idProducto: i.idProducto ?? i.id ?? 0,
        cantidad: Number(i.quantity ?? i.cantidad ?? 1),
        precioUnitario: Number(i.price ?? i.precioUnitario ?? 0),
        impuesto: Number(i.impuesto ?? 0),
        descuento: Number(i.descuento ?? 0),
      }));

      const montoTotal = items.reduce(
        (s, it) => s + it.precioUnitario * it.cantidad,
        0,
      );

      const ordenRequest = {
        idUsuario: Number(idUsuario) || 0,
        idProveedor: Number(idProveedor),
        montoTotal,
        numeroFactura: "TEMP-" + Date.now(), // la pasarela/PUT podrá rellenar estos más tarde
        tipoComprobante: "Boleta", //o factura
        fechaFactura: new Date().toISOString(),
        metodoPago: "Pendiente",
        moneda: "CLP",
        impuestos: 0,
        descuento: 0,
        items,
      };

      try {
        const res = await fetch("http://localhost:5029/api/orden", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          body: JSON.stringify(ordenRequest),
        });

        if (!res.ok) {
          // leer mensaje de error devuelto por la API para debugging
          const text = await res.text();
          console.error(
            "Error al crear orden (prov " + idProveedor + "):",
            res.status,
            text,
          );
          resultados.push({
            idProveedor,
            ok: false,
            status: res.status,
            message: text,
          });
        } else {

          const data = await res.json();
          console.log("Orden creada para proveedor", idProveedor, data);
          resultados.push({ idProveedor, ok: true, data });
          ordenesConfirmadas.push(data);
          
          
        }
      } catch (err) {
        console.error(
          "Fetch error creando orden (prov " + idProveedor + "):",
          err,
        );
        resultados.push({ idProveedor, ok: false, error: err.message });
      }
    }

    // Resultado global
    console.log("Resultados de creación de órdenes:", resultados);

    // Si todo OK, limpiar carrito; si hubo fallos, mantener y notificar
    const algunFallo = resultados.some((r) => !r.ok);
    if (!algunFallo) {
      cart.set([]); // vacía el carrito global
      mostrarComprobante = true;
    } else {
      alert(
        "Se crearon algunas órdenes, pero hubo errores. Revisa la consola para más detalles.",
      );
    }
  }
</script>

<div class="cart-container">
  <h4>Tu carrito ({$totalItems})</h4>

  {#if $cart.length > 0}
    <ul>
      {#each $cart as item}
        <li>
          {item.name ?? item.nombreProducto ?? "Producto"} —
          {item.proveedor?.nombreProveedor ??
            item.nombreProveedor ??
            "Proveedor desconocido"}
          <strong> x{item.quantity ?? item.cantidad}</strong>
          → ${(
            (item.price ?? item.precioUnitario ?? 0) *
            (item.quantity ?? item.cantidad ?? 1)
          ).toFixed(2)}
        </li>
      {/each}
    </ul>

    <p><strong>Total: ${$subtotal.toFixed(2)}</strong></p>
    <button
      on:click={(finalizarcompra) => finalizarCompra()}
      class="btn btn-checkout"
    >
      Finalizar compra
    </button>
  {:else}
    <p>El carrito está vacío.</p>
  {/if}



  {#if mostrarComprobante}
    <ComprobanteCliente
      ordenes={ordenesConfirmadas} onClose={() => (mostrarComprobante = false)}
    />
  {/if}
</div>
