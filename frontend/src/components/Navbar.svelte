<script>
    import { onDestroy, onMount } from "svelte";
    let token = localStorage.getItem("token");
    let tipoUsuario = localStorage.getItem("tipoUsuario");
    let nombreUsuario = localStorage.getItem("nombreUsuario");
    if (tipoUsuario === "Proveedor") {
        nombreUsuario = localStorage.getItem("nombreProveedor");
    }
    let idProveedor = localStorage.getItem("idProveedor");
    let idUsuario = localStorage.getItem("idUsuario");
    console.log("LocalStorage en Navbar:", localStorage);

    function logout() {
        localStorage.removeItem("token");
        localStorage.removeItem("tipoUsuario");
        localStorage.removeItem("nombreUsuario");
        localStorage.removeItem("idProveedor");
        localStorage.removeItem("idUsuario");
        localStorage.removeItem("nombreProveedor");
        window.location.href = "/login";

        token = null;
    }
    function onStorage(e) {
        if (
            e.key === "token" ||
            e.key === "tipoUsuario" ||
            e.key === "nombreUsuario" ||
            e.key === "idProveedor"
        ) {
            token = localStorage.getItem("token");
            tipoUsuario = localStorage.getItem("tipoUsuario");
            nombreUsuario = localStorage.getItem("nombreUsuario");
            idProveedor = localStorage.getItem("idProveedor");
            idUsuario = localStorage.getItem("idUsuario");
            console.log("LocalStorage actualizado en Navbar:", localStorage);
        }
        onMount(() => window.addEventListener("storage", onStorage));
        onDestroy(() => window.removeEventListener("storage", onStorage));
    }
</script>

<header>
    <nav class="navbar navbar-expand-lg navbar-custom bg-light fixed-top">
        <div class="container-fluid">
            <img
                class="icono-nav"
                src="../favico.png"
                alt="drogueria virtual"
            />
            <a class="navbar-brand text-light" href="/">Droguería Virtual</a>
            <button
                class="navbar-toggler"
                type="button"
                data-bs-toggle="collapse"
                data-bs-target="#navbarSupportedContent"
                aria-controls="navbarSupportedContent"
                aria-expanded="false"
                aria-label="Toggle navigation"
            >
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                    {#if token}
                        <li class="nav-item">
                            <a
                                style="text-decoration: none; color:"
                                href="#/ver-transacciones"
                                ><button class="btn btn-warning">
                                    Ver Transacciones</button
                                ></a
                            >
                        </li>
                        <li>
                            <a href="#/direcciones-usuario" class="nav-link text-light>">
                                <button class="btn btn-warning">
                                    Mis Direcciones
                                </button>
                            </a>
                        </li>
                        {#if tipoUsuario === "Administrador"}
                            <li class="nav-item">
                                <a class="nav-link text-light" href="#/admin">
                                    <button class="btn btn-warning"
                                        >Administra usuarios</button
                                    >
                                </a>
                            </li>
                        {:else if tipoUsuario === "Proveedor"}
                            <li class="nav-item">
                                <a
                                    class="nav-link text-light"
                                    href="#/gestionar-productos"
                                    ><button class="btn btn-warning">
                                        Gestionar Productos
                                    </button></a
                                >
                            </li>
                            <li class="nav-item">
                                <a
                                    style="text-decoration: none; color:"
                                    href="#/mis-facturas"
                                    ><button class="btn btn-warning">
                                        Mis Facturas</button
                                    ></a
                                >
                            </li>
                        {/if}
                    {/if}
                </ul>

                {#if token}
                    <!-- Si hay token -->
                    <button on:click={logout} class="btn btn-logout">
                        Cerrar Sesión
                    </button>
                {:else}
                    <!-- Si no hay token -->
                    <a href="#/login">
                        <button type="button" class="btn btn-login">
                            <img
                                src="../avatar.png"
                                alt="iniciarsesion-logo"
                                class="iniciar-sesion-logo"
                            />
                            Iniciar sesión
                        </button>
                    </a>
                {/if}
            </div>
        </div>
    </nav>
</header>
