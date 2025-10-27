<script lang="ts">
  import { onMount } from "svelte";

  let direcciones = [];
  let filtro = "";
  let cargando = true;
  let error = null;
  let direccionEditando = null;
  let mostrarModal = false;
  let esNueva = false;

  const idUsuario = localStorage.getItem("idUsuario");
  const token = localStorage.getItem("token");

  async function cargarDirecciones() {
    cargando = true;
    try {
      const res = await fetch(`http://localhost:5029/api/Direcciones/usuario/${idUsuario}`, {
        headers: {
          "Content-Type": "application/json",
          ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
      });
      if (!res.ok) throw new Error("Error al obtener direcciones");
      direcciones = await res.json();
    } catch (err) {
      error = err.message;
    } finally {
      cargando = false;
    }
  }

  function abrirModal(d = null) {
    direccionEditando = d
      ? { ...d }
      : {
          etiqueta: "",
          region: "",
          comuna: "",
          calle: "",
          numeroCalle: "",
          complemento: "",
          esPrincipal: false
        };
    esNueva = !d;
    mostrarModal = true;
  }

  async function guardarEdicion() {
    const url = esNueva
      ? `http://localhost:5029/api/Direcciones/usuario/${idUsuario}`
      : `http://localhost:5029/api/Direcciones/${direccionEditando.idDireccion}`;
    const method = esNueva ? "POST" : "PUT";

    await fetch(url, {
      method,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(direccionEditando)
    });
    mostrarModal = false;
    cargarDirecciones();
  }

  async function eliminarDireccion(id) {
    if (!confirm("¿Eliminar esta dirección?")) return;
    await fetch(`http://localhost:5029/api/Direcciones/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` }
    });
    cargarDirecciones();
  }

  async function marcarPrincipal(id) {
    await fetch(`http://localhost:5029/api/Direcciones/${id}/principal`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` }
    });
    cargarDirecciones();
  }

  onMount(cargarDirecciones);

  $: direccionesFiltradas = direcciones.filter(d =>
    [d.etiqueta, d.region, d.comuna, d.calle]
      .some(v => v?.toLowerCase().includes(filtro.toLowerCase()))
  );
</script>

<div class="dashboard-container">
  <div class="header d-flex justify-content-between align-items-center">
    <h1>Direcciones de Envío</h1>
    <button class="btn btn-primary" on:click={() => abrirModal()}>
      + Agregar Dirección
    </button>
  </div>

  <div class="filtros-card">
    <h4 class="filtros-titulo">Buscar Dirección</h4>
    <input type="text" class="form-control" placeholder="🔍 Buscar por comuna o región" bind:value={filtro} />
  </div>

  {#if cargando}
    <p class="loading-state">Cargando direcciones...</p>
  {:else if error}
    <p class="error-message">{error}</p>
  {:else if direccionesFiltradas.length === 0}
    <p class="empty-state">No hay direcciones registradas.</p>
  {:else}
    <div class="tabla-wrapper">
      <table class="tabla-usuarios">
        <thead>
          <tr>
            <th>Etiqueta</th>
            <th>Dirección</th>
            <th>Comuna</th>
            <th>Región</th>
            <th>Principal</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {#each direccionesFiltradas as d}
            <tr>
              <td>{d.etiqueta}</td>
              <td>{d.calle} {d.numeroCalle}</td>
              <td>{d.comuna}</td>
              <td>{d.region}</td>
              <td>
                {#if d.esPrincipal}
                  <span class="badge badge-tipo-ADMINISTRADOR">Principal</span>
                {:else}
                  <button class="btn btn-warning" on:click={() => marcarPrincipal(d.idDireccion)}>Marcar</button>
                {/if}
              </td>
              <td>
                <button class="btn-accion btn-warning" on:click={() => abrirModal(d)}>Editar</button>
                <button class="btn-accion btn-danger" on:click={() => eliminarDireccion(d.idDireccion)}>Eliminar</button>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    </div>
  {/if}

  {#if mostrarModal}
    <div class="modal-overlay direcciones-modal">
      <div class="modal-content">
        <h3>{esNueva ? "Agregar Dirección" : "Editar Dirección"}</h3>
        <div class="grid-2-cols">
          <div>
            <label>Etiqueta</label>
            <input bind:value={direccionEditando.etiqueta} placeholder="Casa, Trabajo, Sucursal 1, etc." />
          </div>
          <div>
            <label>Región</label>
            <input bind:value={direccionEditando.region} placeholder="Región Metropolitana, Región del Maule, Región del Bio Bio, etc." />
          </div>
          <div>
            <label>Comuna</label>
            <input bind:value={direccionEditando.comuna} placeholder="Concepción, San Pedro de la Paz, Lota, Coronel, etc."/>
          </div>
          <div>
            <label>Calle</label>
            <input bind:value={direccionEditando.calle} placeholder="Avenida las Condes " />
          </div>
          <div>
            <label>Número</label>
            <input bind:value={direccionEditando.numeroCalle}  placeholder="Número de calle, Avenida las Condes 85"/>
          </div>
          <div>
            <label>Complemento</label>
            <input bind:value={direccionEditando.complemento} placeholder=" Casa con reja de madera, Sala 2B piso 1, etc." />
          </div>
          <div class="form-check">
            <label for="esPrincipal" class="form-check-label" style="color: black;">Marcar como principal</label>
            <input type="checkbox" id="esPrincipal"  bind:checked={direccionEditando.esPrincipal} class="form-check-input" />
          </div>
        </div>
        <div class="modal-actions mt-3">
          <button class="btn btn-cancel" on:click={() => (mostrarModal = false)}>Cancelar</button>
          <button class="btn btn-primary-confirm" on:click={guardarEdicion}>
            {esNueva ? "Agregar" : "Guardar"}
          </button>
        </div>
      </div>
    </div>
  {/if}
</div>
