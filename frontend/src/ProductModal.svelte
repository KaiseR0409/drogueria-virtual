<script>
    import { onMount } from "svelte";

    // recibir props (Svelte 5 runas)
    let { product, close, sucess } = $props();

    // Determina si estamos editando
    const isEditing = $derived(!!product?.idProducto);

    // Estado reactivo del formulario con campos iniciales
    let formData = $state({
        nombreProducto: "",
        principioActivo: "",
        concentracion: "",
        formaFarmaceutica: "",
        presentacionComercial: "",
        laboratorioFabricante: "",
        registroSanitario: "",
        fechaVencimiento: "",
        condicionesAlmacenamiento: "",
        precio: 0,
        stock: 0,
        imagenUrl: "",
    });
    let submitting = $state(false);

    onMount(() => {
        // Precargar datos si estamos editando
        if (isEditing) {
            const p = product.producto; // objeto completo del producto
            formData = {
                nombreProducto: p.nombreProducto || "",
                principioActivo: p.principioActivo || "",
                concentracion: p.concentracion || "",
                formaFarmaceutica: p.formaFarmaceutica || "",
                presentacionComercial: p.presentacionComercial || "",
                laboratorioFabricante: p.laboratorioFabricante || "",
                registroSanitario: p.registroSanitario || "",
                // Formato de fecha para input type="date"
                fechaVencimiento: p.fechaVencimiento
                    ? p.fechaVencimiento.split("T")[0]
                    : "",
                condicionesAlmacenamiento: p.condicionesAlmacenamiento || "",
                precio: product.precio || 0,
                stock: product.stock || 0,
                imagenUrl: p.imagenUrl || "",
            };
        }
    });

    async function handleSubmit() {
        try {
            submitting = true;
            const idProveedor = localStorage.getItem("idProveedor");
            if (!idProveedor) {
                alert("No se encontró idProveedor en localStorage.");
                submitting = false;
                return;
            }

            // Validación de campos requeridos
            const required = [
                "nombreProducto", "principioActivo", "concentracion", "formaFarmaceutica", 
                "presentacionComercial", "laboratorioFabricante", "registroSanitario", 
                "condicionesAlmacenamiento", "fechaVencimiento"
            ];
            const missing = required.filter(
                (k) => (formData[k] ?? "").toString().trim().length === 0,
            );
            if (missing.length) {
                alert("Complete los campos obligatorios: " + missing.join(", "));
                submitting = false;
                return;
            }

            // Construcción del payload
            const body = {
                nombreProducto: formData.nombreProducto.trim(),
                principioActivo: formData.principioActivo.trim(),
                concentracion: formData.concentracion.trim(),
                formaFarmaceutica: formData.formaFarmaceutica.trim(),
                presentacionComercial: formData.presentacionComercial.trim(),
                laboratorioFabricante: formData.laboratorioFabricante.trim(),
                registroSanitario: formData.registroSanitario.trim(),
                // Asegura formato ISO para la API
                fechaVencimiento: new Date(formData.fechaVencimiento).toISOString(),
                condicionesAlmacenamiento: formData.condicionesAlmacenamiento.trim(),
                imagenUrl: formData.imagenUrl?.trim() || "",
                precio: Number(formData.precio),
                stock: Number(formData.stock),
            };

            let method;
            let url;
            if (isEditing) {
                // MODO EDICION (PUT)
                const idProducto = product.idProducto;
                method = "PUT";
                url = `http://localhost:5029/api/proveedor/${idProveedor}/producto/${idProducto}`;
                body.idProducto = product.idProducto; // ID necesario para el PUT
            } else {
                // MODO CREACION (POST)
                method = "POST";
                url = `http://localhost:5029/api/proveedor/${idProveedor}/producto`;
            }

            const res = await fetch(url, {
                method: method,
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body),
            });

            if (!res.ok) {
                const text = await res.text();
                console.error("API error:", res.status, text);
                alert(`Error al ${isEditing ? "editar" : "guardar"} el producto. Estado: ${res.status}.`);
                return;
            }
            
            // Éxito
            const message = isEditing ? "Producto editado con éxito." : "Producto publicado con éxito.";
            alert(message);
            sucess(); // notificar al padre para refrescar la lista
            close();
        } catch (err) {
            console.error("handleSubmit error:", err);
            alert("Error al enviar datos. Revisa la consola.");
        } finally {
            submitting = false;
        }
    }
</script>

<form on:submit|preventDefault={handleSubmit}>
    <fieldset>
        <legend>Información Farmacéutica</legend>
        <div class="form-grid">
            <label>Nombre: <input type="text" bind:value={formData.nombreProducto} required /></label>
            <label>Principio Activo: <input type="text" bind:value={formData.principioActivo} required /></label>
            <label>Concentración: <input type="text" bind:value={formData.concentracion} /></label>
            <label>Forma Farmacéutica: <input type="text" bind:value={formData.formaFarmaceutica} required /></label>
            <label>Presentación: <input type="text" bind:value={formData.presentacionComercial} /></label>
            <label>Laboratorio: <input type="text" bind:value={formData.laboratorioFabricante} required /></label>
            <label>Registro Sanitario: <input type="text" bind:value={formData.registroSanitario} /></label>
            <label>Vencimiento: <input type="date" bind:value={formData.fechaVencimiento} required /></label>
        </div>
        <label class="full-width">Almacenamiento:
            <textarea
                bind:value={formData.condicionesAlmacenamiento}
                rows="2"
                placeholder="Ej: Ambiente fresco y seco"
            ></textarea>
        </label>
    </fieldset>

    <fieldset>
        <legend>Foto del Producto</legend>
        <div class="file-input-group">
            <input type="file" accept="image/*" class="file-input" />
        </div>
        <p class="upload-info">Archivos permitidos: JPG, PNG. Máx 2MB.</p>
    </fieldset>

    <fieldset>
        <legend>Inventario y Precios</legend>
        <div class="form-grid">
            <label>Precio de Venta ($): <input type="number" step="0.01" bind:value={formData.precio} min="0.01" required /></label>
            <label>Stock Disponible: <input type="number" bind:value={formData.stock} min="0" required /></label>
        </div>
    </fieldset>

    <div class="actions">
        <button type="button" class="btn btn-cancel" on:click={close}>Cancelar</button>
        <button type="submit" class="btn btn-submit" disabled={submitting}>
            {#if submitting}
                Guardando...
            {:else}
                {isEditing ? "Guardar Cambios" : "Publicar Producto"}
            {/if}
        </button>
    </div>
</form>
