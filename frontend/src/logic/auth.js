import { push } from 'svelte-spa-router';

/**
 * Verifica si el usuario está autenticado y tiene el rol requerido.
 */
export function checkAuth(options = {}) {
  const { rolRequerido = null } = options;

  const token = localStorage.getItem('token');
  const tipoUsuario = localStorage.getItem('tipoUsuario');

  if (!token) {
    push('/login');
    return;
  }

  if (rolRequerido && tipoUsuario !== rolRequerido) {
    alert('No tienes permiso para acceder a esta página.');
    push('/');
  }
}

/**
 * Inicia sesión guardando la sesión del usuario en localStorage
 * @param {object} data - Datos devueltos por la API de login
 */
export function login(data) {
  if (!data) throw new Error("No se recibieron datos de login");

  localStorage.setItem('token', data.token);
  localStorage.setItem('tipoUsuario', data.tipoUsuario);
  localStorage.setItem('idUsuario', data.idUsuario ?? '');
  localStorage.setItem('idProveedor', data.idProveedor ?? '');
}
