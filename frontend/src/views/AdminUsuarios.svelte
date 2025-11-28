<script>
    import { onMount } from "svelte";
    import { checkAuth } from "../logic/auth.js";
    checkAuth({ rolRequerido: "Administrador" });

    let usuarios = [];

    // Variables de estado del Modal de Edición
    let mostrarModal = false;
    let usuarioAEditar = {}; // Objeto que contendrá los datos del usuario seleccionado

    let nuevaContrasena = "";

    // Variables de filtro
    let filtroNombre = "";
    let filtroTipo = "Todos";
    let filtroEstado = "Todos";

    let cargando = true;
    let error = null;
    let mensajeModal = { tipo: "", texto: "" }; // Mensajes de éxito/error dentro del modal

    // Opciones para los select de filtro y formulario
    const tiposUsuario = ["Todos", "Cliente", "Proveedor", "Administrador"];
    const estadosUsuario = ["Todos", "Activo", "Inactivo", "Suspendido"];

    async function cargarUsuarios() {
        try {
            cargando = true;
            const res = await fetch("http://localhost:5029/api/Usuario");
            if (!res.ok) throw new Error("Error al obtener usuarios");

            let lista = await res.json();

            for (const u of lista) {
                u.direcciones = await obtenerDirecciones(u.idUsuario);
            }

            usuarios = lista;
        } catch (e) {
            error = e.message;
        } finally {
            cargando = false;
        }
    }

    // Función para formatear las fechas (solo YYYY-MM-DD)
    function formatearFecha(fecha) {
        if (!fecha) return "N/A";
        try {
            return fecha.substring(0, 10);
        } catch (e) {
            return fecha;
        }
    }

    //paginacion
    const itemsPerPage = 8;
    let currentPage = 1;

    // Calcular el número total de páginas reactivamente
    $: totalPages = Math.ceil(usuariosFiltrados.length / itemsPerPage);

    // Calcular el índice de inicio y fin para el slice
    $: startIndex = (currentPage - 1) * itemsPerPage;
    $: endIndex = startIndex + itemsPerPage;

    // Obtener los usuarios de la página actual
    $: usuariosPaginados = usuariosFiltrados.slice(startIndex, endIndex);

    // Función para cambiar de página
    function goToPage(page) {
        if (page >= 1 && page <= totalPages) {
            currentPage = page;
        }
    }

    $: usuariosFiltrados, (currentPage = 1);

    $: usuariosFiltrados = usuarios.filter((u) => {
        // filtrar por nombre
        const cumpleNombre = u.nombreUsuario
            ?.toLowerCase()
            .includes(filtroNombre.toLowerCase());

        // filtrar por tipo de usuario
        const cumpleTipo =
            filtroTipo === "Todos" || u.tipoUsuario === filtroTipo;

        // filtrar por estado
        const cumpleEstado =
            filtroEstado === "Todos" || u.estadoUsuario === filtroEstado;

        return cumpleNombre && cumpleTipo && cumpleEstado;
    });

    function validarFormulario() {
        if (
            !usuarioAEditar.nombreUsuario ||
            usuarioAEditar.nombreUsuario.trim() === ""
        )
            return "El nombre no puede estar vacío.";

        if (!usuarioAEditar.usuario || usuarioAEditar.usuario.trim() === "")
            return "El usuario de inicio de sesión no puede estar vacío.";

        if (!usuarioAEditar.telefono || usuarioAEditar.telefono.trim() === "")
            return "El teléfono no puede estar vacío.";

        if (usuarioAEditar.telefono.length !== 12)
            return "El teléfono debe tener 12 caracteres (+569XXXXXXXX).";

        if (!usuarioAEditar.estadoUsuario)
            return "Debes seleccionar un estado.";

        if (
            !usuarioAEditar.tipoEstablecimiento ||
            usuarioAEditar.tipoEstablecimiento.trim() === ""
        )
            return "Debes ingresar un tipo de establecimiento.";

        // contraseña solo validar si se está cambiando
        if (nuevaContrasena.trim() !== "" && nuevaContrasena.length < 6)
            return "La contraseña debe tener al menos 6 caracteres.";

        return "";
    }

    function abrirModalEdicion(usuario) {
        usuarioAEditar = { ...usuario };
        nuevaContrasena = "";
        mostrarModal = true;
        mensajeModal = { tipo: "", texto: "" };
    }

    async function guardarEdicion() {
        const errorValidacion = validarFormulario();
        if (errorValidacion) {
            mensajeModal = { tipo: "error", texto: errorValidacion };
            return;
        }
        try {
            mensajeModal = { tipo: "cargando", texto: "Guardando cambios..." };

            const datosAEnviar = { ...usuarioAEditar };

            if (nuevaContrasena.trim() !== "") {
                datosAEnviar.password = nuevaContrasena.trim();
            } else {
                delete datosAEnviar.password;
            }

            datosAEnviar.fechaActualizacion = new Date().toISOString();

            const res = await fetch(
                `http://localhost:5029/api/Usuario/${datosAEnviar.idUsuario}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(datosAEnviar),
                },
            );

            if (!res.ok) throw new Error("Error al actualizar usuario");

            mensajeModal = {
                tipo: "exito",
                texto: " ¡Usuario actualizado correctamente!",
            };
            // Recargar la lista principal para reflejar los cambios
            setTimeout(() => {
                mostrarModal = false;
            }, 500);
            await cargarUsuarios();
        } catch (e) {
            console.error("Error en la solicitud PUT:", e);
            mensajeModal = {
                tipo: "error",
                texto: `❌ Error al actualizar: ${e.message}`,
            };
        }
    }

    // Lógica existente para el cambio de estado (mantener)
    async function cambiarEstado(usuario) {
        const estados = ["Activo", "Inactivo", "Suspendido"];
        let id = estados.indexOf(usuario.estadoUsuario);
        if (id === -1) id = 0;
        usuario.estadoUsuario = estados[(id + 1) % estados.length];

        usuarios = [...usuarios];
        try {
            await fetch(
                `http://localhost:5029/api/Usuario/${usuario.idUsuario}`,
                {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(usuario),
                },
            );
        } catch (e) {
            console.error("Error al cambiar estado:", e);
            error = "No se pudo actualizar el estado del usuario.";
        }
    }

    async function obtenerDirecciones(idUsuario) {
        try {
            const res = await fetch(
                `http://localhost:5029/api/Direcciones/usuario/${idUsuario}`,
            );
            if (!res.ok) throw new Error("Error al obtener direcciones");
            return await res.json();
        } catch (err) {
            console.error(err);
            return [];
        }
    }

    onMount(() => {
        cargarUsuarios();
    });
</script>

<div class="dashboard-container">
    <div class="header">
        <h2>Administración de Usuarios</h2>
        <a href="#/agregar-usuario" class="btn btn-principal">
            Agregar Usuario</a
        >
    </div>

    <div class="filtros-card">
        <p class="filtros-titulo">Buscar y Filtrar</p>
        <div class="controles-grid">
            <input
                type="text"
                placeholder="🔍 Filtrar por Nombre..."
                bind:value={filtroNombre}
                class="form-control control-filtro"
            />
            <select bind:value={filtroTipo} class="form-control control-filtro">
                {#each tiposUsuario as tipo}
                    <option value={tipo}>{tipo}</option>
                {/each}
            </select>
            <select
                bind:value={filtroEstado}
                class="form-control control-filtro"
            >
                {#each estadosUsuario as estado}
                    <option value={estado}>{estado}</option>
                {/each}
            </select>
        </div>
    </div>

    {#if cargando}
        <p class="loading-state">Cargando usuarios...</p>
    {:else if error}
        <p class="error-message">{error}</p>
    {:else if usuariosFiltrados.length === 0}
        <p class="empty-state">
            No se encontraron usuarios que coincidan con los filtros.
        </p>
    {:else}
        <div class="tabla-wrapper">
            <table class="tabla-usuarios">
                <thead>
                    <tr>
                        <th class="col-nombre">Nombre</th>
                        <th>Tipo</th>
                        <th>Teléfono</th>
                        <th class="col-fecha">Creación</th>
                        <th class="col-fecha">Actualización</th>
                        <th>Establecimiento</th>
                        <th>Dirección</th>
                        <th>Estado</th>
                        <th class="col-acciones">Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    {#each usuariosPaginados as u (u.idUsuario)}
                        <tr>
                            <td class="nombre-usuario">{u.nombreUsuario}</td>
                            <td>
                                <span class="badge badge-tipo-{u.tipoUsuario}"
                                    >{u.tipoUsuario}</span
                                >
                            </td>
                            <td>{u.telefono}</td>
                            <td>{formatearFecha(u.fechaCreacion)}</td>
                            <td>{formatearFecha(u.fechaActualizacion)}</td>
                            <td>{u.tipoEstablecimiento}</td>
                            <td>
                                {#if u.direcciones.length > 0}
                                    {#each u.direcciones as d}
                                        {#if d.esPrincipal}
                                            {d.calle} {d.numero}, {d.comuna}
                                        {/if}
                                    {/each}
                                {:else}
                                    Sin Dirección
                                {/if}
                            </td>
                            <td>
                                <button
                                    type="button"
                                    class="btn-estado btn-estado-{u.estadoUsuario}"
                                    on:click={() => cambiarEstado(u)}
                                >
                                    {u.estadoUsuario}
                                </button>
                            </td>
                            <td class="acciones-celda">
                                <button
                                    class="btn-warning btn-accion"
                                    on:click={() => abrirModalEdicion(u)}
                                >
                                    Editar
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if totalPages > 1}
            <div class="pagination-controls">
                <button
                    class="btn btn-secondary btn-pagination"
                    on:click={() => goToPage(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    &laquo; Anterior
                </button>

                <span class="page-info">
                    Página {currentPage} de {totalPages}
                </span>

                <button
                    class="btn btn-secondary btn-pagination"
                    on:click={() => goToPage(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    Siguiente &raquo;
                </button>
            </div>
        {/if}
    {/if}
</div>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_click_events_have_key_events -->
{#if mostrarModal}
    <!-- svelte-ignore a11y_no_static_element_interactions -->
    <div class="modal-overlay">
        <div class="modal modal-default" style="max-width: 992px">
            <div class="modal-header">
                <h3 class="modal-title">
                    Editar Usuario: {usuarioAEditar.nombreUsuario}
                </h3>
                <button
                    class="modal-close"
                    on:click={() => (mostrarModal = false)}>&times;</button
                >
            </div>

            <div class="modal-body">
                <form on:submit|preventDefault={guardarEdicion}>
                    <div class="form-group mb-3">
                        <label for="idUsuario">ID de Usuario</label>
                        <input
                            id="idUsuario"
                            type="text"
                            class="form-control"
                            value={usuarioAEditar.idUsuario}
                            disabled
                        />
                    </div>

                    <div class="form-group mb-3">
                        <label for="nombreUsuario">Nombre de Usuario</label>
                        <input
                            id="nombreUsuario"
                            type="text"
                            class="form-control"
                            bind:value={usuarioAEditar.nombreUsuario}
                            required
                        />
                    </div>

                    <div class="form-group mb-3">
                        <label for="usuarioLogin"
                            >Usuario de inicio de sesión</label
                        >
                        <input
                            id="usuarioLogin"
                            type="text"
                            class="form-control"
                            bind:value={usuarioAEditar.usuario}
                            required
                            style="border: 1px solid gray;"
                            placeholder="Ej: jprieto89"
                        />
                    </div>

                    <div class="form-group mb-3">
                        <label for="nuevaContrasena"
                            >Nueva Contraseña (Dejar vacío para no cambiar)</label
                        >
                        <input
                            id="nuevaContrasena"
                            type="password"
                            class="form-control"
                            bind:value={nuevaContrasena}
                            placeholder="Ingrese nueva contraseña aquí"
                            style="border: 1px solid gray;"
                        />
                    </div>

                    <div class="form-group mb-3">
                        <label for="telefono">Teléfono</label>
                        <input
                            id="telefono"
                            type="text"
                            class="form-control"
                            maxlength="12"
                            bind:value={usuarioAEditar.telefono}
                            style="border: 1px solid gray;"
                        />
                    </div>

                    <div class="form-group mb-3">
                        <label for="estadoUsuario">Estado</label>
                        <select
                            id="estadoUsuario"
                            class="form-control"
                            bind:value={usuarioAEditar.estadoUsuario}
                            required
                            style="border: 1px solid gray;"
                        >
                            {#each estadosUsuario.filter((e) => e !== "Todos") as estado}
                                <option value={estado}>{estado}</option>
                            {/each}
                        </select>
                    </div>

                    <div class="form-group mb-3">
                        <label for="tipoEstablecimiento"
                            >Tipo de Establecimiento</label
                        >
                        <input
                            id="tipoEstablecimiento"
                            type="text"
                            class="form-control"
                            bind:value={usuarioAEditar.tipoEstablecimiento}
                        />
                    </div>

                    {#if mensajeModal.texto}
                        <p class="alert alert-{mensajeModal.tipo} alert-modal">
                            {#if mensajeModal.tipo === "cargando"}
                                <span> {mensajeModal.texto}</span>
                            {:else if mensajeModal.tipo === "exito"}
                                <span>{mensajeModal.texto}</span>
                            {:else}
                                <span>{mensajeModal.texto}</span>
                            {/if}
                        </p>
                    {/if}

                    <div class="modal-actions">
                        <button
                            type="button"
                            class="btn btn-secondary btn-cancel"
                            on:click={() => (mostrarModal = false)}
                        >
                            Cancelar
                        </button>
                        <button
                            type="submit"
                            class="btn btn-submit"
                            disabled={mensajeModal.tipo === "cargando"}
                        >
                            Guardar Cambios
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
{/if}
