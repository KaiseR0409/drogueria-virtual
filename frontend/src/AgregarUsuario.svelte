<script>
    import { checkAuth } from "./auth.js";
    checkAuth({ rolRequerido: "Administrador" });

    let nombreUsuario = "";
    let usuario = "";
    let password = "";
    let tipoUsuario = "Cliente";
    let direccion1 = "";
    let direccion2 = "";
    let direccion3 = "";
    let correo = "";
    let telefono = "";
    let mensaje = "";
    let error = "";
    let tipoEstablecimiento = "";

    // Campos solo para proveedores
    let nombreProveedor = "";
    let rut = "";
    let giro = "";
    let direccionComercial = "";
    let ciudad = "";

    async function agregarUsuario() {
        mensaje = "";
        error = "";

        if (!nombreUsuario || !usuario || !password || !correo || !telefono) {
            error = "Completa todos los campos obligatorios.";
            return;
        }

        if (telefono.length !== 12) {
            error = "El teléfono debe tener formato +569XXXXXXXX.";
            return;
        }

        try {
            const body = {
                NombreUsuario: nombreUsuario,
                Usuario: usuario,
                Password: password,
                TipoUsuario: tipoUsuario,
                tipoEstablecimiento: tipoEstablecimiento,
                Correo: correo,
                Telefono: telefono,
                Direccion1: direccion1,
                Direccion2: direccion2 || null,
                Direccion3: direccion3 || null,
                EstadoUsuario: "Inactivo",
            };
            console.log("Cuerpo de la solicitud:", body);

            // Si es proveedor, añadimos los campos extra
            if (tipoUsuario === "Proveedor") {
                body.NombreProveedor = nombreProveedor;
                body.Rut = rut;
                body.Giro = giro;
                body.DireccionComercial = direccionComercial;
                body.Ciudad = ciudad;
            }

            const res = await fetch("http://localhost:5029/api/Usuario", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body),
            });

            if (!res.ok) {
                const data = await res.json();
                throw new Error(data.mensaje || "Error al agregar usuario");
            }

            const data = await res.json();
            mensaje = `✅ Usuario ${data.nombreUsuario} agregado correctamente.`;

            // Limpiar campos
            nombreUsuario = "";
            usuario = "";
            password = "";
            tipoUsuario = "Cliente";
            correo = "";
            telefono = "";
            direccion1 = "";
            direccion2 = "";
            direccion3 = "";
            nombreProveedor = "";
            rut = "";
            giro = "";
            direccionComercial = "";
            ciudad = "";
            tipoEstablecimiento = "";
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
    <!-- svelte-ignore a11y_label_has_associated_control -->
    <form on:submit|preventDefault={agregarUsuario} class="form-agregar">
        <div class="mb-3">
            <label class="form-label">Nombre Completo</label>
            <input
                type="text"
                class="form-control"
                bind:value={nombreUsuario}
                required
            />
        </div>

        <div class="mb-3">
            <label class="form-label">Usuario (login)</label>
            <input
                type="text"
                class="form-control"
                bind:value={usuario}
                required
            />
        </div>

        <div class="mb-3">
            <label class="form-label">Contraseña</label>
            <input
                type="password"
                class="form-control"
                bind:value={password}
                required
            />
        </div>

        <div class="mb-3">
            <label class="form-label">Correo</label>
            <input
                type="email"
                class="form-control"
                bind:value={correo}
                required
            />
        </div>
        <div class="mb-3">
            <label class="form-label">Tipo de Establecimiento</label>
            <input
                type="text"
                class="form-control"
                bind:value={tipoEstablecimiento}
                placeholder="Ej: Farmacia, Laboratorio, Cliente final"
                required
            />
        </div>

        <div class="mb-3">
            <label class="form-label">Teléfono</label>
            <input
                type="text"
                class="form-control"
                maxlength="12"
                bind:value={telefono}
                placeholder="+56912345678"
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

        <!-- Campos visibles solo si es proveedor -->
        {#if tipoUsuario === "Proveedor"}
            <hr />
            <h4>🧾 Datos del Proveedor</h4>

            <div class="mb-3">
                <label class="form-label">Nombre Comercial</label>
                <input
                    type="text"
                    class="form-control"
                    bind:value={nombreProveedor}
                    required={tipoUsuario === "Proveedor"}
                />
            </div>

            <div class="mb-3">
                <label class="form-label">RUT</label>
                <input
                    type="text"
                    class="form-control"
                    bind:value={rut}
                    placeholder="Ej: 76.543.210-K"
                />
            </div>

            <div class="mb-3">
                <label class="form-label">Giro</label>
                <input
                    type="text"
                    class="form-control"
                    bind:value={giro}
                    placeholder="Ej: Distribución farmacéutica"
                />
            </div>

            <div class="mb-3">
                <label class="form-label">Dirección Comercial</label>
                <input
                    type="text"
                    class="form-control"
                    bind:value={direccionComercial}
                    placeholder="Ej: Av. Las Flores 123"
                />
            </div>

            <div class="mb-3">
                <label class="form-label">Ciudad</label>
                <input
                    type="text"
                    class="form-control"
                    bind:value={ciudad}
                    placeholder="Ej: Santiago"
                />
            </div>
        {/if}

        <hr />

        <div class="mb-3">
            <label class="form-label"
                >Dirección 1 *Dirección Principal y obligatoria*
            </label>
            <input
                type="text"
                class="form-control"
                bind:value={direccion1}
                required
            />
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
