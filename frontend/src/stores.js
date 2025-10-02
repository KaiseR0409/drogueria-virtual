import { writable, derived } from 'svelte/store';

// --- Filtros de búsqueda ---
export const filters = writable({
    busquedaNombre: '',
    busquedaPrincipioActivo: '',
    busquedaLaboratorio: '',
    busquedaFormaFarmaceutica: '',
    laboratoriosSeleccionados: []
});

// --- Sesión ---
const initialToken = localStorage.getItem("token");
const initialTipoUsuario = localStorage.getItem("tipoUsuario");
const initialIdUsuario = localStorage.getItem("idUsuario");
const initialIdProveedor = localStorage.getItem("idProveedor"); // puede no estar

export const session = writable({
    token: initialToken,
    tipoUsuario: initialTipoUsuario,
    idUsuario: initialIdUsuario,
    idProveedor: initialIdProveedor,
    isLoggedIn: !!initialToken
});

/**
 * Iniciar sesión: guardar token y datos en localStorage + actualizar store.
 * @param {object} data - { token, tipoUsuario, idUsuario, idProveedor? }
 */
export function login(data) {
    // Guardar siempre estos datos
    localStorage.setItem("token", data.token);
    localStorage.setItem("tipoUsuario", data.tipoUsuario);
    localStorage.setItem("idUsuario", data.idUsuario);

    // Guardar idProveedor solo si corresponde
    if (data.tipoUsuario === "Proveedor" && data.idProveedor) {
        localStorage.setItem("idProveedor", data.idProveedor);
    }

    // Actualizar store
    session.set({
        token: data.token,
        tipoUsuario: data.tipoUsuario,
        idUsuario: data.idUsuario,
        idProveedor: data.idProveedor || null,
        isLoggedIn: true
    });
}

/**
 * Cerrar sesión: limpiar localStorage + resetear store.
 */
export function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("tipoUsuario");
    localStorage.removeItem("idUsuario");
    localStorage.removeItem("idProveedor");

    session.set({
        token: null,
        tipoUsuario: null,
        idUsuario: null,
        idProveedor: null,
        isLoggedIn: false
    });

    // Redirigir a login/home
    window.location.href = '/';
}

// --- Carrito global ---
export const cart = writable([]);

// Subtotales reactivos
export const subtotal = derived(cart, ($cart) =>
  $cart.reduce((sum, item) => sum + item.price * item.quantity, 0)
);

export const totalItems = derived(cart, ($cart) =>
  $cart.reduce((sum, item) => sum + item.quantity, 0)
);
