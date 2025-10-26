// Archivo: src/auth.js
import { push } from 'svelte-spa-router';

/**
 * Verifica si el usuario está autenticado y tiene el rol requerido.
 * Si no cumple, lo redirige.
 * @param {object} options - Opciones de autorización.
 * @param {string|null} [options.rolRequerido] - El rol que se necesita para acceder.
 */
export function checkAuth(options = {}) {
    const { rolRequerido = null } = options;

    const token = localStorage.getItem('token');
    const tipoUsuario = localStorage.getItem('tipoUsuario');

    // Si no hay token, redirige a login
    if (!token) {
        push('/login');
        return;
    }

    // Si se necesita un rol específico y el usuario no lo tiene, redirige al inicio
    if (rolRequerido && tipoUsuario !== rolRequerido) {
        alert('No tienes permiso para acceder a esta página.');
        push('/');
    }
}