
import { writable } from 'svelte/store';
// Este store contendrá el estado de todos los filtros
export const filters = writable({
    busquedaNombre: '',
    busquedaPrincipioActivo: '',
    busquedaLaboratorio: '',
    busquedaFormaFarmaceutica: '',
    laboratoriosSeleccionados: []
});

// Store para manejar el estado de autenticación del usuario
const initialToken = localStorage.getItem("token");
const initialTipoUsuario = localStorage.getItem("tipoUsuario");
const initialIdProveedor = localStorage.getItem("idProveedor");

export const session = writable({
    token: initialToken,
    tipoUsuario: initialTipoUsuario,
    idProveedor: initialIdProveedor,
    isLoggedIn: !!initialToken
});

/**
 * Función para iniciar sesión y actualizar el store y localStorage.
 * @param {object} data - Datos de la respuesta del login ({ token, tipoUsuario, idProveedor })
 */
export function login(data) {
    // 1. Guardar en localStorage
    localStorage.setItem("token", data.token);
    localStorage.setItem("tipoUsuario", data.tipoUsuario);
    
        // Guardar el ID del proveedor si el usuario es un proveedor
    if (data.tipoUsuario === 'Proveedor' && data.idProveedor) {
        localStorage.setItem("idProveedor", data.idProveedor);
    }
    
    //Actualiza el store con los nuevos datos
    session.set({
        token: data.token,
        tipoUsuario: data.tipoUsuario,
        // Usamos el ID de los datos de login, o null si no existe
        idProveedor: data.idProveedor || null, 
        isLoggedIn: true
    });
}

/**
 * Función para cerrar sesión y limpiar el store y localStorage.
 */
export function logout() {
    //Limpiar localStorage
    localStorage.removeItem("token");
    localStorage.removeItem("tipoUsuario");
    localStorage.removeItem("idProveedor"); // 🎯 Limpiar el ID
    
    //Actualiza el store a estado vacío
    session.set({
        token: null,
        tipoUsuario: null,
        idProveedor: null,
        isLoggedIn: false
    });
    
    // Opcional: Redirigir a la página de inicio
    window.location.hash = '/'; 
}