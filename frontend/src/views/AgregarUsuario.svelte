<script>
    import { checkAuth } from "../logic/auth.js";
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

        const errorValidacion = validarFormulario();
        if (errorValidacion) {
            error = errorValidacion;
            window.scrollTo({ top: 0, behavior: "smooth" });
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
                EstadoUsuario: "Inactivo",
            };

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
                let mensajeError = "";

                try {
                    const data = await res.clone().json();
                    mensajeError =
                        data.mensaje || data.error || JSON.stringify(data);
                } catch {
                    mensajeError = await res.text(); 
                }

                error = mensajeError;

                window.scrollTo({ top: 0, behavior: "smooth" });

                throw new Error(mensajeError);
            }

            const data = await res.json();
            mensaje = `Usuario ${data.nombreUsuario} agregado correctamente.`;
            nombreUsuario = "";
            usuario = "";
            password = "";
            tipoUsuario = "Cliente";
            correo = "";
            telefono = "";
            nombreProveedor = "";
            rut = "";
            giro = "";
            direccionComercial = "";
            ciudad = "";
            tipoEstablecimiento = "";

            window.scrollTo({ top: 0, behavior: "smooth" });

            setTimeout(() => {
                window.location.href = "#/admin";
            }, 2000);
        } catch (e) {
            error = e.message;
        }
    }

    function formatearRut(valor) {
        let limpio = valor.replace(/\./g, "").replace(/-/g, "");

        if (limpio.length > 9) limpio = limpio.substring(0, 9);

        if (limpio.length < 2) return limpio;

        const cuerpo = limpio.slice(0, -1);
        const dv = limpio.slice(-1);

        const conPuntos = cuerpo.replace(/\B(?=(\d{3})+(?!\d))/g, "."); // puntos cada 3 digitos

        return conPuntos + "-" + dv.toUpperCase();
    }

    function validarDV(rutCompleto) {
        rutCompleto = rutCompleto
            .replace(/\./g, "")
            .replace(/-/g, "")
            .toUpperCase();
        const cuerpo = rutCompleto.slice(0, -1);
        let dv = rutCompleto.slice(-1);

        let suma = 0;
        let multiplo = 2;

        // Recorrer RUT de derecha a izquierda
        for (let i = cuerpo.length - 1; i >= 0; i--) {
            suma += multiplo * parseInt(cuerpo[i]);
            multiplo = multiplo < 7 ? multiplo + 1 : 2;
        }

        const dvEsperado = 11 - (suma % 11);

        let dvFinal;
        if (dvEsperado === 11) dvFinal = "0";
        else if (dvEsperado === 10) dvFinal = "K";
        else dvFinal = dvEsperado.toString();

        return dvFinal === dv;
    }

    function validarFormulario() {
        if (!nombreUsuario.trim())
            return "El nombre completo no puede estar vacío.";

        if (!usuario.trim()) return "El usuario (login) no puede estar vacío.";

        if (!password.trim()) return "La contraseña no puede estar vacía.";

        if (password.length < 6)
            return "La contraseña debe tener al menos 6 caracteres.";

        if (!correo.trim()) return "El correo no puede estar vacío.";

        if (!tipoEstablecimiento.trim())
            return "Debes ingresar un tipo de establecimiento.";

        if (!telefono.trim()) return "Debes ingresar un teléfono.";

        if (telefono.length !== 12 || !telefono.startsWith("+569"))
            return "El teléfono debe tener formato +569XXXXXXXX.";

        if (tipoUsuario === "Proveedor") {
            if (!rut.trim()) return "El RUT es obligatorio para proveedores.";

            // Largo mínimo y máximo (7.111.111-K a 99.999.999-K)
            if (rut.length < 11 || rut.length > 12)
                return "El RUT debe estar entre 11 y 12 caracteres incluyendo puntos y guion.";

            // Validar formato
            if (!/^\d{1,2}\.\d{3}\.\d{3}-[\dkK]$/.test(rut))
                return "El formato del RUT no es válido. Ejemplo correcto: 76.543.210-K";

            // validar digito verificador
            if (!validarDV(rut)) return "El RUT ingresado no es válido.";
        }

        return ""; // Todo OK
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
                    on:input={() => (rut = formatearRut(rut))}
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

        <button type="submit" class="btn btn-primary">Agregar Usuario</button>
    </form>
</div>
