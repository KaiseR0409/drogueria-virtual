<script>
  import { filters } from '../logic/stores.js';
  import { onMount } from 'svelte';

  let allLaboratorios = [];
  let displayedLaboratorios = [];
  let allProveedores = [];
  let displayedProveedores = [];

    // Llamada al endpoint para obtener la lista de laboratorios
    onMount(async () => {
        const res = await fetch('http://localhost:5029/api/Productos/Laboratorios');
        if (res.ok) {
            allLaboratorios = await res.json();
            displayedLaboratorios = allLaboratorios;
        }
         // Proveedores
        try {
          const resProv = await fetch('http://localhost:5029/api/Proveedores/Listado');
          if (resProv.ok) {
            allProveedores = await resProv.json();
            displayedProveedores = allProveedores;
          }
        } catch (error) {
          console.error("Error al obtener proveedores:", error);
        }
    });
    // Filtra la lista de checkboxes
    $: {
        if ($filters.busquedaLaboratorio) {
            displayedLaboratorios = allLaboratorios.filter(lab => 
                lab.toLowerCase().includes($filters.busquedaLaboratorio.toLowerCase())
            );
        } else {
            displayedLaboratorios = allLaboratorios;
        }
    }
    //filtra la lista de checkboxes proveedores
    $: {
    if ($filters.busquedaProveedor) {
      displayedProveedores = allProveedores.filter(p =>
        p.toLowerCase().includes($filters.busquedaProveedor.toLowerCase())
      );
    } else {
      displayedProveedores = allProveedores;
    }
  }
    
</script>

<div class="card filters-card p-3 mb-3">
  <div class="card-header border-0 bg-transparent p-0">
    <h5 class="card-title text-white">Filtros</h5>
  </div>
  
  <div class="card-body p-0">
    <div class="mb-3">
      <label for="searchNombre" class="form-label text-white">Nombre o Marca</label>
      <input 
        type="text" 
        class="form-control form-control-sm" 
        id="searchNombre" 
        placeholder="Buscar por nombre..." 
        bind:value={$filters.busquedaNombre}
      >
    </div>
    
    <div class="mb-3">
      <label for="searchPA" class="form-label text-white">Principio Activo</label>
      <input 
        type="text" 
        class="form-control form-control-sm" 
        id="searchPA" 
        placeholder="Buscar principio activo..." 
        bind:value={$filters.busquedaPrincipioActivo}
      >
    </div>
    
    <div class="mb-3">
      <label for="searchLab" class="form-label text-white">Laboratorio</label>
      <input 
        type="text" 
        class="form-control form-control-sm" 
        id="searchLab" 
        placeholder="Buscar laboratorio..." 
        bind:value={$filters.busquedaLaboratorio}

      >
    </div>
     <div class="checkbox-list mb-3">
            {#each displayedLaboratorios as lab}
                <div class="form-check text-white">
                    <input
                        class="form-check-input" style="max-width: 20px;"
                        type="checkbox"
                        value={lab}
                        id={`checkbox-${lab}`}
                        bind:group={$filters.laboratoriosSeleccionados}
                    />
                    <label class="form-check-label" for={`checkbox-${lab}`}>
                        {lab}
                    </label>
                </div>
            {/each}
        </div>
      <hr class="border-light my-3">
    <div class="mb-3">
      <label for="searchProv" class="form-label text-white">Proveedor</label>
      <input 
        type="text"
        class="form-control form-control-sm"
        id="searchProv"
        placeholder="Buscar proveedor..."
        bind:value={$filters.busquedaProveedor}
      >
    </div>

    <div class="checkbox-list mb-3">
      {#each displayedProveedores as prov}
        <div class="form-check text-white">
          <input
            class="form-check-input"
            type="checkbox"
            value={prov}
            id={`checkbox-prov-${prov}`}
            bind:group={$filters.proveedoresSeleccionados}
          />
          <label class="form-check-label" for={`checkbox-prov-${prov}`}>
            {prov}
          </label>
        </div>
      {/each}
    </div>
    
    <div class="mb-3">
      <label for="searchFF" class="form-label text-white">Forma Farmacéutica</label>
      <input 
        type="text" 
        class="form-control form-control-sm" 
        id="searchFF" 
        placeholder="Buscar forma..." 
        bind:value={$filters.busquedaFormaFarmaceutica}
      >
    </div>
  </div>
</div>
