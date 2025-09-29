<script>
    // Estos son datos de ejemplo para el carrito, no son reactivos.
    // En una aplicación real, estos datos se actualizarían.
    const cartItems = [
        { name: "Ibuprofeno 600 mg", price: 2.50, quantity: 2 },
        { name: "Amoxicilina 500 mg", price: 4.75, quantity: 1 },
        { name: "Vitamina C 1000 mg", price: 3.20, quantity: 3 }
    ];

    // Calcula el subtotal y el total de productos.
    const subtotal = cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const totalItems = cartItems.reduce((sum, item) => sum + item.quantity, 0);
</script>

<div class="container-fluid mt-4">
    <div class="row">
        <div class="col-md-3">
            <div class="cart-container p-3 rounded-3 shadow-sm">
                <h4 class="cart-title mb-3">
                    Tu carrito <span class="badge rounded-pill bg-light text-dark">({totalItems})</span>
                </h4>
                
                {#if cartItems.length > 0}
                    <ul class="list-group list-group-flush mb-3">
                        {#each cartItems as item}
                            <li class="list-group-item d-flex justify-content-between align-items-center cart-item">
                                <span class="item-name">{item.name}</span>
                                <span class="item-details">
                                    <span class="badge bg-secondary me-2">x{item.quantity}</span>
                                    ${(item.price * item.quantity).toFixed(2)}
                                </span>
                            </li>
                        {/each}
                    </ul>
                    <div class="d-flex justify-content-between align-items-center total-box p-2 mt-3 rounded">
                        <strong>Subtotal:</strong>
                        <span>${subtotal.toFixed(2)}</span>
                    </div>
                    <div class="d-grid mt-3">
                        <button class="btn btn-checkout">
                            Finalizar compra
                        </button>
                    </div>
                {:else}
                    <div class="text-center p-4">
                        <p>El carrito está vacío.</p>
                    </div>
                {/if}
            </div>
        </div>
    </div>
</div>
