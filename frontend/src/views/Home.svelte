
<script>
    import Filtros from '../components/Filtros.svelte';
    import Productos from '../components/Productos.svelte';
    import Carrito from '../components/Carrito.svelte';
    import { cart, subtotal, totalItems } from '../logic/stores.js';

    let mostrarCarritoMovil = false;
    let mostrarFiltrosMovil = false;
    

    function toggleFiltrosMovil() {
        mostrarFiltrosMovil = !mostrarFiltrosMovil;
    }

    function toggleCarritoMovil() {
        mostrarCarritoMovil = !mostrarCarritoMovil;
    }
</script>


<div class="home-grid">
    <div class="filtros-container"><Filtros/></div>
    <div class="productos-container"><Productos/></div>
    <div class="carrito-container"><Carrito/></div>
</div>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="cart-mobile" on:click={toggleCarritoMovil}>
    <div>🛒 {$totalItems} producto{ $totalItems !== 1 ? "s" : "" }</div>
    <div>Total: <strong>${$subtotal.toFixed(2)}</strong></div>
    <button>{mostrarCarritoMovil ? "▼" : "▲"}</button>
</div>

{#if mostrarCarritoMovil}
    <div class="cart-mobile-modal">
        <div class="cart-mobile-modal-header">
            <h4>Tu Carrito ({$totalItems})</h4>
            <button class="close-mobile-cart" on:click={toggleCarritoMovil}>✕</button>
        </div>

        <!-- Reutilizar el componente existente -->
        <Carrito />
    </div>
{/if}

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="filtros-mobile-bar" on:click={toggleFiltrosMovil}>
    <span>🔍 Filtros</span>
    <button>{mostrarFiltrosMovil ? "▲" : "▼"}</button>
</div>

{#if mostrarFiltrosMovil}
    <div class="filtros-mobile-panel">
        <Filtros />
    </div>
{/if}

