<script>
    import { onMount } from "svelte";

    let usuarios = [];
    
    // Variables de filtro
    let filtroNombre = "";
    let filtroTipo = "Todos"; // Filtro por Tipo de Usuario
    let filtroEstado = "Todos"; // Filtro por Estado
    
    let cargando = true;
    let error = null;

    // Opciones para los select de filtro
    const tiposUsuario = ["Todos", "CLIENTE", "PROVEEDOR", "ADMINISTRADOR"];
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

    // Función para formatear las fechas
    function formatearFecha(fecha) {
        if (!fecha) return 'N/A';
        try {
            // mostrar solo la fecha YYYY-MM-DD
            return fecha.substring(0, 10); 
        } catch (e) {
            return fecha;
        }
    }
    
    // Lógica de Filtro COMBINADA
    $: usuariosFiltrados = usuarios.filter((u) => {
        // filtrar por nombre
        const cumpleNombre = u.nombreUsuario?.toLowerCase().includes(filtroNombre.toLowerCase());
        
        // filtrar por tipo de usuario
        const cumpleTipo = filtroTipo === "Todos" || u.tipoUsuario === filtroTipo;

        // filtrar por estado
        const cumpleEstado = filtroEstado === "Todos" || u.estadoUsuario === filtroEstado;

        return cumpleNombre && cumpleTipo && cumpleEstado;
    });

    async function cambiarEstado(usuario) {
        const estados = ["Activo", "Inactivo", "Suspendido"];
        let id = estados.indexOf(usuario.estadoUsuario);
        if (id === -1) id = 0;
        usuario.estadoUsuario = estados[(id + 1) % estados.length];

        try {
            const res = await fetch(
                `http://localhost:5029/api/Usuario/${usuario.idUsuario}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(usuario),
                },
            );

            if (!res.ok) throw new Error("Error al actualizar usuario");

            // recargar la lista de usuarios para reflejar el cambio
            await cargarUsuarios();
        } catch (e) {
            console.error("Error en la solicitud PUT:", e);
            error = "No se pudo actualizar el estado del usuario.";
        }
    }
    
    async function eliminarUsuario(usuario) {
        if (!confirm(`¿Estás seguro de eliminar al usuario ${usuario.nombreUsuario}?`)) {
            return;
        }

        try {
            const res = await fetch(
                `http://localhost:5029/api/Usuario/${usuario.idUsuario}`,
                {
                    method: "DELETE",
                },
            );

            if (!res.ok) throw new Error("Error al eliminar usuario");

            // recargar la lista de usuarios para reflejar el cambio
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
        <a href="#/agregar-usuario" class="btn btn-principal">➕ Agregar Usuario</a>
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

            <select bind:value={filtroEstado} class="form-control control-filtro">
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
        <p class="empty-state">No se encontraron usuarios que coincidan con los filtros.</p>
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
                    {#each usuariosFiltrados as u (u.idUsuario)}
                        <tr>
                            <td class="nombre-usuario">{u.nombreUsuario}</td>
                            <td><span class="badge badge-tipo-{u.tipoUsuario}">{u.tipoUsuario}</span></td>
                            <td>{u.telefono}</td>
                            <td>{formatearFecha(u.fechaCreacion)}</td>
                            <td>{formatearFecha(u.fechaActualizacion)}</td>
                            <td>{u.tipoEstablecimiento}</td>
                            <td class="direccion-celda">
                                {u.direccion1 || 'Sin Dirección'} 
                                {#if u.direccion2} / {u.direccion2}{/if}
                                {#if u.direccion3} / {u.direccion3}{/if}
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
                                <button class="btn-warning btn-accion">Editar</button>
                                <button class="btn-danger btn-accion" on:click={() => eliminarUsuario(u)}>Eliminar</button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>