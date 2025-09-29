<script>
    import { onMount } from "svelte";

    let usuarios = [];
    let filtro = "";
    let cargando = true;
    let error = null;

    // Llamada a tu API
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

    // Cambiar estado de usuario y actualizar la lista
    async function cambiarEstado(usuario) {
        const estados = ["Activo", "Inactivo", "Suspendido"]; //array de estados
        let id = estados.indexOf(usuario.estadoUsuario); //busca el estado actual en el array con el estado de usuario y devuelve su indice
        if (id === -1) id = 0; //validacion 
        usuario.estadoUsuario = estados[(id + 1) % estados.length]; //suma +1 al indice y usa el modulo para volver al inicio del array si es necesario

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

            // Vuelve a cargar la lista de usuarios para reflejar el cambio
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

            // Vuelve a cargar la lista de usuarios para reflejar el cambio
            await cargarUsuarios();
        } catch (e) {
            console.error("Error en la solicitud DELETE:", e);
            error = "No se pudo eliminar el usuario.";
        }
    }

    // Filtro por nombre de usuario
    $: usuariosFiltrados = usuarios.filter((u) =>
        u.nombreUsuario?.toLowerCase().includes(filtro.toLowerCase()),
    );

    onMount(() => {
        cargarUsuarios();
    });
</script>

<h2>👤 Administración de Usuarios</h2>

<input
    type="text"
    placeholder="Filtrar por nombre..."
    bind:value={filtro}
    class="form-control"
/>
<a type="submit" class="btn btn-agregar-usuario" href="#/agregar-usuario">Agregar usuario</a>

{#if cargando}
    <p>Cargando usuarios...</p>
{:else if error}
    <p style="color:red">⚠️ {error}</p>
{:else}
    <table class="tabla-usuarios">
        <thead>
            <tr>
                <th>Nombre</th>
                <th>Tipo Usuario</th>
                <th>Fecha Creación</th>
                <th>Fecha Actualización</th>
                <th>Tipo Establecimiento</th>
                <th>Dirección 1</th>
                <th>Dirección 2</th>
                <th>Dirección 3</th>
                <th>Estado</th>
                <th>Acciones</th>
            </tr>
        </thead>
        <tbody>
            {#each usuariosFiltrados as u}
                <tr>
                    <td>{u.nombreUsuario}</td>
                    <td>{u.tipoUsuario}</td>
                    <td>{u.fechaCreacion}</td>
                    <td>{u.fechaActualizacion}</td>
                    <td>{u.tipoEstablecimiento}</td>
                    <td>{u.direccion1}</td>
                    <td>{u.direccion2}</td>
                    <td>{u.direccion3}</td>
                    <td>
                        <button
                            class={u.estadoUsuario === "Activo"
                                ? "btn btn-success"
                                : u.estadoUsuario === "Inactivo"
                                  ? "btn btn-secondary"
                                  : "btn btn-dark"}
                            on:click={() => cambiarEstado(u)}
                        >
                            {u.estadoUsuario}
                        </button>
                    </td>
                    <td>
                        <button class="btn btn-warning">Editar</button>
                        <button class="btn btn-danger " on:click={() => eliminarUsuario(u)}>Eliminar</button>
                    </td>
                </tr>
            {/each}
        </tbody>
    </table>
{/if}
