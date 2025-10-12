<script>
    import { onMount } from "svelte";
    import { checkAuth } from './auth.js';
    checkAuth({ rolRequerido: 'Administrador' });

    let nombreUsuario = "";
    let usuario = "";
    let password = "";
    let tipoUsuario = "Cliente";
    let tipoEstablecimiento = "Cliente";
    let direccion1 = "";
    let direccion2 = "";
    let direccion3 = "";
    let correo = "";
    let telefono = "";
    let mensaje = "";
    let error = "";

        
    let telefono_usuario = "";
    

    let telefono_completo;

   
    $: telefono_completo = `+56 9 ${telefono_usuario}`;


    async function agregarUsuario() {
        mensaje = "";
        error = "";

        // Validar campos básicos
        if (!nombreUsuario || !usuario || !password || !direccion1 || !correo || !telefono) {
            error = "Completa todos los campos obligatorios";
            return;
        }

        if (telefono.length !== 12) {
            error = "El teléfono debe tener exactamente 8 dígitos";
            return;
        }

        try {
            const res = await fetch("http://localhost:5029/api/Usuario", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    NombreUsuario: nombreUsuario,
                    Usuario: usuario,
                    Password: password,
                    TipoUsuario: tipoUsuario,
                    TipoEstablecimiento: tipoEstablecimiento,
                    Direccion1: direccion1,
                    Direccion2: direccion2 || null,
                    Direccion3: direccion3 || null,
                    EstadoUsuario: "Inactivo",
                    Correo: correo,
                    Telefono: telefono // Backend agrega +56 9 automáticamente
                }),
            });

            if (!res.ok) {
                const data = await res.json();
                throw new Error(data.mensaje || "Error al agregar usuario");
            }

            const data = await res.json();
            mensaje = `Usuario ${data.nombreUsuario} agregado correctamente!`;

            // Limpiar campos
            nombreUsuario = "";
            usuario = "";
            password = "";
            tipoUsuario = "Cliente";
            tipoEstablecimiento = "Cliente";
            direccion1 = "";
            direccion2 = "";
            direccion3 = "";
            correo = "";
            telefono = "";
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
            <label class="form-label">Nombre Completo</label>
            <input type="text" class="form-control" bind:value={nombreUsuario} required />
        </div>

        <div class="mb-3">
            <label class="form-label">Usuario (login)</label>
            <input type="text" class="form-control" bind:value={usuario} required />
        </div>

        <div class="mb-3">
            <label class="form-label">Contraseña</label>
            <input type="password" class="form-control" bind:value={password} required />
        </div>

        <div class="mb-3">
            <label class="form-label">Correo</label>
            <input type="email" class="form-control" bind:value={correo} required />
        </div>

        <div class="mb-3">
            <label class="form-label">Teléfono</label>
            <input 
                type="text"
                class="form-control"
                maxlength="12"
                bind:value={telefono}
                placeholder="Ingresa el número"
                required
            />
            <small class="form-text text-muted">
                Ejemplo: +56912345678
            </small>
        </div>

        <div class="mb-3">
            <label class="form-label">Tipo de Usuario</label>
            <select class="form-select" bind:value={tipoUsuario}>
                <option value="Cliente">Cliente</option>
                <option value="Administrador">Administrador</option>
                <option value="Proveedor">Proveedor</option>
            </select>
        </div>

        <div class="mb-3">
            <label class="form-label">Tipo de Establecimiento</label>
            <input type="text" class="form-control" bind:value={tipoEstablecimiento} />
        </div>

        <div class="mb-3">
            <label class="form-label">Dirección 1</label>
            <input type="text" class="form-control" bind:value={direccion1} required />
        </div>

        <div class="mb-3">
            <label class="form-label">Dirección 2 (opcional)</label>
            <input type="text" class="form-control" bind:value={direccion2} />
        </div>

        <div class="mb-3">
            <label class="form-label">Dirección 3 (opcional)</label>
            <input type="text" class="form-control" bind:value={direccion3} />
        </div>

        <button type="submit" class="btn btn-primary">Agregar Usuario</button>
    </form>
</div>
