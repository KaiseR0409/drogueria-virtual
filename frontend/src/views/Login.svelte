<script>
  import { onMount } from "svelte";
 import {login } from '../logic/auth.js';

  let usuario = '';
  let password = '';
  let error = null;

  async function handleLogin() {
    error = null;
    try {
      const res = await fetch("http://localhost:5029/api/Usuario/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ usuario, password })
      });
      if (!res.ok) {
        throw new Error("Usuario o contraseña incorrectos");

      }

      const data = await res.json();
              console.log("Error en login", data);
      // normalizar idProveedor usando idUsuario si backend no lo devuelve
      const idProv = data.idProveedor ?? data.proveedor?.idProveedor ?? data.idUsuario ?? data.id ?? null;

      login({
        token: data.token,
        tipoUsuario: data.tipoUsuario,
        idUsuario: data.idUsuario ?? data.id ?? null,
        idProveedor: idProv
      });

      // Redirigir según tipo de usuario
      if (data.tipoUsuario === "Admin") {
        window.location.href = "/admin";
      } else {
        window.location.href = "/home";
      }

    } catch (e) {
      error = e.message;
    }
  }

  onMount(() => {
    const token = localStorage.getItem("token");
    if (token) {
      const tipoUsuario = localStorage.getItem("tipoUsuario");
      if (tipoUsuario === "Admin") {
        window.location.href = "/admin";
      } else {
        window.location.href = "/home";
      }
    }
  });
</script>

<div class="login-container d-flex justify-content-center align-items-center vh-100">
  <div class="card p-4 rounded-3 shadow-sm login-card">
    <h3 class="card-title text-center mb-4 login-title">Iniciar Sesión</h3>
    <form on:submit|preventDefault={handleLogin}>
      <div class="mb-3">
        <label for="usuarioInput" class="form-label">Nombre de Usuario</label>
        <input
          type="text"
          class="form-control"
          id="usuarioInput"
          placeholder="Ingresa tu usuario..."
          bind:value={usuario}
          required
        />
      </div>
      <div class="mb-3">
        <label for="passwordInput" class="form-label">Contraseña</label>
        <input
          type="password"
          class="form-control"
          id="passwordInput"
          placeholder="Escribe tu contraseña"
          bind:value={password}
          required
        />
      </div>
      {#if error}
        <p style="color:red">{error}</p>
      {/if}
      <div class="d-grid gap-2">
        <button type="submit" class="btn btn-login">
          Iniciar Sesión
        </button>
      </div>
    </form>
  </div>
</div>
