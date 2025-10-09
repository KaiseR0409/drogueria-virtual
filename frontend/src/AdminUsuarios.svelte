<script>
    import { onMount } from "svelte";

    let usuarios = [];

    // Variables de estado del Modal de Edición
    let mostrarModal = false;
    let usuarioAEditar = {}; // Objeto que contendrá los datos del usuario seleccionado

    //NUEVA VARIABLE PARA LA CONTRASEÑA
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
            usuarios = await res.json();
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

    // Resetear la página a 1 cuando los filtros cambian (para evitar páginas vacías)
    $: usuariosFiltrados, (currentPage = 1);

    // Lógica de Filtro COMBINADA
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

    // Abre el modal y rellena el formulario con los datos del usuario
    function abrirModalEdicion(usuario) {
        // Clonar el objeto para no modificar los datos originales antes de Guardar
        usuarioAEditar = { ...usuario };
        nuevaContrasena = ""; // IMPORTANTE: Limpiar la contraseña al abrir
        mostrarModal = true;
        mensajeModal = { tipo: "", texto: "" }; // Limpiar mensajes
    }

    // Envía la solicitud PUT para actualizar el usuario
    async function guardarEdicion() {
        try {
            mensajeModal = { tipo: "cargando", texto: "Guardando cambios..." };

            // Clonar el objeto a enviar para modificarlo temporalmente
            const datosAEnviar = { ...usuarioAEditar };

            // Lógica de Contraseña: Solo agregarla si se ingresó un valor
            if (nuevaContrasena.trim() !== "") {
                // Aquí se envía la nueva contraseña para que el backend la hashee
                datosAEnviar.password = nuevaContrasena.trim();
            } else {
                delete datosAEnviar.password;
            }

            // Asegurarse de que la fecha de actualización sea la actual
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
            console.log("datos del usuario", datosAEnviar);
            // Recargar la lista principal para reflejar los cambios
            await cargarUsuarios();

            // Cerrar el modal después de un breve tiempo
            setTimeout(() => {
                mostrarModal = false;
            }, 1500);
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

        // Lógica de PUT simplificada para el estado
        try {
            await fetch(
                `http://localhost:5029/api/Usuario/${usuario.idUsuario}`,
                {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(usuario),
                },
            );
            await cargarUsuarios();
        } catch (e) {
            console.error("Error al cambiar estado:", e);
            error = "No se pudo actualizar el estado del usuario.";
        }
    }

    // Lógica existente para eliminar (mantener)
    async function eliminarUsuario(usuario) {
        if (
            !confirm(
                `¿Estás seguro de eliminar al usuario ${usuario.nombreUsuario}?`,
            )
        ) {
            return;
        }
        try {
            const res = await fetch(
                `http://localhost:5029/api/Usuario/${usuario.idUsuario}`,
                { method: "DELETE" },
            );
            if (!res.ok) throw new Error("Error al eliminar usuario");
            await cargarUsuarios();
        } catch (e) {
            console.error("Error en la solicitud DELETE:", e);
            error = "No se pudo eliminar el usuario.";
        }
    }

    onMount(() => {
        cargarUsuarios();
    });
</script>

<div class="dashboard-container">
    <div class="header">
        <h2>👤 Administración de Usuarios</h2>
        <a href="#/agregar-usuario" class="btn btn-principal"
            >➕ Agregar Usuario</a
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
        <p class="error-message">⚠️ {error}</p>
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
                            <td class="direccion-celda">
                                {u.direccion1 || "Sin Dirección"}
                                {#if u.direccion2}
                                    / {u.direccion2}{/if}
                                {#if u.direccion3}
                                    / {u.direccion3}{/if}
                            </td>
                            <td>
                                <button
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
                                <button
                                    class="btn-danger btn-accion"
                                    on:click={() => eliminarUsuario(u)}
                                >
                                    Eliminar
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
    <div class="modal-overlay" on:click|self={() => (mostrarModal = false)}>
        <div class="modal modal-default">
            <div class="modal-header">
                <h3 class="modal-title">
                    🖊️ Editar Usuario: {usuarioAEditar.nombreUsuario}
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
                        <label for="tipoUsuario">Tipo de Usuario</label>
                        <select
                            id="tipoUsuario"
                            class="form-control"
                            bind:value={usuarioAEditar.tipoUsuario}
                            required
                            style="border: 1px solid gray;"
                        >
                            {#each tiposUsuario.filter((t) => t !== "Todos") as tipo}
                                <option value={tipo}>{tipo}</option>
                            {/each}
                        </select>
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

                    <div class="grid-2-cols mb-3">
                        <div>
                            <label for="direccion1">Dirección 1</label>
                            <input
                                id="direccion1"
                                type="text"
                                class="form-control"
                                bind:value={usuarioAEditar.direccion1}
                            />
                        </div>
                        <div>
                            <label for="direccion2">Dirección 2</label>
                            <input
                                id="direccion2"
                                type="text"
                                class="form-control"
                                bind:value={usuarioAEditar.direccion2}
                            />
                        </div>
                        <div class="full-width">
                            <label for="direccion3">Dirección 3</label>
                            <input
                                id="direccion3"
                                type="text"
                                class="form-control"
                                bind:value={usuarioAEditar.direccion3}
                            />
                        </div>
                    </div>

                    {#if mensajeModal.texto}
                        <p class="alert alert-{mensajeModal.tipo} alert-modal">
                            {#if mensajeModal.tipo === "cargando"}
                                <span>🔄 {mensajeModal.texto}</span>
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
