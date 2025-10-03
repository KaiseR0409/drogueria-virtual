<script>
  import { onMount } from "svelte";
  import FacturaPreview from "./FacturaPreview.svelte";

  let facturas = [];
  let filtroEstado = "Todos";
  let idProveedor = localStorage.getItem("idProveedor");

  onMount(async () => {
    if (!idProveedor) return;

    try {
      const res = await fetch(`http://localhost:5029/api/Orden/mis-facturas/${idProveedor}`);
      if (!res.ok) {
        console.error("Error al cargar facturas:", res.status);
        return;
      }
      facturas = await res.json();
    } catch (err) {
      console.error("Error en fetch:", err);
    }
  });

  $: facturasFiltradas = facturas
    .filter(f => filtroEstado === "Todos" || f.estadoOrden === filtroEstado)
    .sort((a, b) => new Date(b.fechaOrden) - new Date(a.fechaOrden));
</script>

<select bind:value={filtroEstado}>
  <option>Todos</option>
  <option>Pagada</option>
  <option>Pendiente</option>
</select>

<table class="table table-mis-facturas table-striped table-hover">
  <thead style="background-color: red; color: white;">
    <tr>
      <th>Factura</th>
      <th>Orden</th>
      <th>Usuario</th>
      <th>Estado</th>
      <th>Fecha</th>
      <th>Monto</th>
      <th>Acciones</th>
    </tr>
  </thead>
  <tbody>
    {#each facturasFiltradas as f}
      <tr>
        <td>{f.numeroFactura}</td>
        <td>{f.idOrden}</td>
        <td>ID {f.idUsuario}</td>
        <td>{f.estadoOrden}</td>
        <td>{new Date(f.fechaOrden).toLocaleString()}</td>
        <td>${f.montoTotal}</td>
        <td>
          <FacturaPreview {f} factura={f} />
        </td>
      </tr>
    {/each}
  </tbody>
</table>
