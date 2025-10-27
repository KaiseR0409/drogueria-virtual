<script>
    import { createEventDispatcher } from 'svelte';
    
    export let product; 
    
    const dispatch = createEventDispatcher();
    let quantity = 1;

    // Controla que la cantidad no exceda el stock ni sea menor a 1
    $: maxQuantity = product.stock > 0 ? product.stock : 0;
    $: if (quantity > maxQuantity) quantity = maxQuantity;
    $: if (quantity < 1) quantity = 1;


    function triggerAddToCart() {
        if (quantity < 1 || quantity > maxQuantity || product.stock <= 0) return;
        
        // Objeto que se enviará a la función de cotización del componente padre
        const item = {
            idProducto: product.idProducto,
            name: product.nombreProducto,
            price: product.precio,
            quantity: quantity, 
            proveedor: { 
                idProveedor: product.idProveedor, 
                nombreProveedor: product.nombreProveedor 
            }
        };
        dispatch('addtocart', item);
        
        // Reinicia la cantidad a 1 después de agregar al carrito
        quantity = 1;
    }
</script>

<div class="similar-card">
    <h5 class="similar-name" title={product.nombreProducto}>
        {product.nombreProducto} 
    </h5>
    
    <p class="similar-meta provider-name">
        Proveedor: {product.nombreProveedor}
    </p>
    
    <div class="similar-details">
        <p class="similar-meta price-value">
            Precio: <strong>${product.precio.toFixed(2)}</strong>
        </p>
        <p class="similar-meta stock-value" class:low-stock={product.stock <= 10}>
            Stock: {product.stock}
        </p>
    </div>
    
    <div class="action-group">
        <input 
            type="number" 
            bind:value={quantity} 
            min="1" 
            max={maxQuantity} 
            disabled={product.stock <= 0}
            class="quantity-input"
        >
        <button 
            class="btn-add-similar" 
            on:click={triggerAddToCart}
            disabled={product.stock <= 0 || quantity < 1 || quantity > maxQuantity}
        >
            + Agregar
        </button>
    </div>
</div>
