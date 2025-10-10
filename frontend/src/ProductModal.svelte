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
        marca: "",
        codigoBarras: "",
        imagenUrl: "",
        precio: 0,
        stock: 0,
    });

    let submitting = $state(false);

    onMount(() => {
        // Precargar datos si estamos editando
        if (isEditing) {
            const p = product.producto;
            formData = {
                nombreProducto: p.nombreProducto || "",
                principioActivo: p.principioActivo || "",
                concentracion: p.concentracion || "",
                formaFarmaceutica: p.formaFarmaceutica || "",
                presentacionComercial: p.presentacionComercial || "",
                laboratorioFabricante: p.laboratorioFabricante || "",
                registroSanitario: p.registroSanitario || "",
                fechaVencimiento: p.fechaVencimiento
                    ? p.fechaVencimiento.split("T")[0]
                    : "",
                condicionesAlmacenamiento: p.condicionesAlmacenamiento || "",
                marca: p.marca || "",
                codigoBarras: p.codigoBarras || "",
                imagenUrl: p.imagenUrl || "",
                precio: product.precio || 0,
                stock: product.stock || 0,
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
                "condicionesAlmacenamiento", "fechaVencimiento", "marca", "codigoBarras"
            ];
            const missing = required.filter(
                (k) => (formData[k] ?? "").toString().trim().length === 0,
            );
            if (missing.length) {
                alert("Complete los campos obligatorios: " + missing.join(", "));
                submitting = false;
                return;
            }

            // Validar código de barras: debe ser único y 12 dígitos
            if (!/^\d{12}$/.test(formData.codigoBarras)) {
                alert("El código de barras debe tener exactamente 12 dígitos numéricos.");
                submitting = false;
                return;
            }

            const body = {
                nombreProducto: formData.nombreProducto.trim(),
                principioActivo: formData.principioActivo.trim(),
                concentracion: formData.concentracion.trim(),
                formaFarmaceutica: formData.formaFarmaceutica.trim(),
                presentacionComercial: formData.presentacionComercial.trim(),
                laboratorioFabricante: formData.laboratorioFabricante.trim(),
                registroSanitario: formData.registroSanitario.trim(),
                fechaVencimiento: new Date(formData.fechaVencimiento).toISOString(),
                condicionesAlmacenamiento: formData.condicionesAlmacenamiento.trim(),
                marca: formData.marca.trim(),
                codigoBarras: String(formData.codigoBarras).trim(),
                imagenUrl: formData.imagenUrl?.trim() || "",
                precio: Number(formData.precio),
                stock: Number(formData.stock),
            };

            let method, url;
            if (isEditing) {
                // MODO EDICIÓN
                const idProducto = product.idProducto;
                method = "PUT";
                url = `http://localhost:5029/api/proveedor/${idProveedor}/producto/${idProducto}`;
                body.idProducto = idProducto;
            } else {
                // MODO CREACIÓN
                method = "POST";
                url = `http://localhost:5029/api/proveedor/${idProveedor}/producto`;
            }

            const res = await fetch(url, {
                method,
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body),
            });

            if (!res.ok) {
                const text = await res.text();
                console.error("API error:", res.status, text);
                alert(`Error al ${isEditing ? "editar" : "guardar"} el producto.`);
                return;
            }

            alert(isEditing ? "Producto editado con éxito." : "Producto publicado con éxito.");
            sucess();
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
            <label>Marca: <input type="text" bind:value={formData.marca} required /></label>
            <label>Código de Barras: 
                <input
                    id="codigoBarras"
                    type="number"
                    bind:value={formData.codigoBarras}
                    min="100000000000"   
                    max="999999999999"   
                    on:input={(e) => {
                        // Evitar más de 12 caracteres
                        if (e.target.value.length > 12) {
                            e.target.value = e.target.value.slice(0, 12);
                            formData.codigoBarras = e.target.value;
                        }
                    }}
                    required/>
            <label>Vencimiento: <input type="date" bind:value={formData.fechaVencimiento} required /></label>
        </div>

        <label class="full-width">Almacenamiento:
            <textarea
                bind:value={formData.condicionesAlmacenamiento}
                rows="2"
                placeholder="Ej: Mantener en lugar fresco y seco"
            ></textarea>
        </label>
    </fieldset>

    <fieldset>
        <legend>Imagen del Producto</legend>
        <label>URL de la Imagen:
            <input type="url" bind:value={formData.imagenUrl} placeholder="https://..." />
        </label>
        <p class="upload-info">Puedes pegar el enlace directo a la imagen (JPG, PNG).</p>
    </fieldset>

    <fieldset>
        <legend>Inventario y Precios</legend>
        <div class="form-grid">
            <label>Precio de Venta ($): 
                <input type="number" step="0.01" bind:value={formData.precio} min="0.01" required />
            </label>
            <label>Stock Disponible: 
                <input type="number" bind:value={formData.stock} min="0" required />
            </label>
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
