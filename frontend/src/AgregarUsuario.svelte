<script>
    import { onMount } from "svelte";

    let nombreUsuario = "";
    let password = "";
    let tipoUsuario = "Cliente"; // Puede ser "Administrador" o "Cliente"
    let mensaje = "";
    let error = "";

    async function agregarUsuario() {
        mensaje = "";
        error = "";

        // Validar campos
        if (!nombreUsuario || !password) {
            error = "Completa todos los campos";
            return;
        }

        try {
            const res = await fetch("http://localhost:5029/api/Usuario", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    NombreUsuario: nombreUsuario,
                    Usuario: nombreUsuario,
                    Password: password,
                    TipoUsuario: tipoUsuario,
                    TipoEstablecimiento: "Cliente",
                    direccion1: "Sin dirección",
                    EstadoUsuario: "Activo",
                }),
            });

            if (!res.ok) {
                const data = await res.json();
                throw new Error(data.mensaje || "Error al agregar usuario");
            }

            const data = await res.json();
            console.log(data);

            // si es proveedor, crearlo también en la tabla proveedores
            if (tipoUsuario === "Proveedor") {
                const proveedorRes = await fetch(
                    `http://localhost:5029/api/Proveedor/registrar/${data.idUsuario}`,
                    {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                            nombreProveedor: data.nombreUsuario,
                        }),
                    },
                );

                

                if (!proveedorRes.ok) {
                    error = proveedorRes.statusText;
                    console.log(error);
                    throw new Error("Error al registrar proveedor");
                }
            }

            mensaje = `Usuario ${data.nombreUsuario} agregado correctamente!`;
            // Limpiar campos
            nombreUsuario = "";
            password = "";
            tipoUsuario = "Cliente";
        } catch (e) {
            error = e.message;
        }
    }
</script>

<div class="agregar-usuario-container">
    <h2>📝 Agregar Usuario</h2>

    {#if mensaje}
        <p class="alert alert-success">{mensaje}</p>
    {/if}
    {#if error}
        <p class="alert alert-danger">{error}</p>
    {/if}

    <form on:submit|preventDefault={agregarUsuario} class="form-agregar">
        <div class="mb-3">
            <label class="form-label">Nombre de Usuario</label>
            <input
                type="text"
                class="form-control"
                bind:value={nombreUsuario}
                placeholder="Ingrese nombre de usuario"
                required
            />
        </div>

        <div class="mb-3">
            <label class="form-label">Contraseña</label>
            <input
                type="password"
                class="form-control"
                bind:value={password}
                placeholder="Ingrese contraseña"
                required
            />
        </div>

        <div class="mb-3">
            <label class="form-label">Tipo de Usuario</label>
            <select class="form-select" bind:value={tipoUsuario}>
                <option value="Cliente">Cliente</option>
                <option value="Administrador">Administrador</option>
                <option value="Proveedor">Proveedor</option>
            </select>
        </div>

        <button type="submit" class="btn btn-primary">Agregar Usuario</button>
    </form>
</div>
