<script>
	import DireccionesUsuario from './views/DireccionesUsuario.svelte';
    import Router from "svelte-spa-router";
    import Navbar from "./components/Navbar.svelte";
    import Home from "./views/Home.svelte";
    import Login from "./views/Login.svelte";
    import AdminUsuarios from "./views/AdminUsuarios.svelte";
    import AgregarUsuario from "./views/AgregarUsuario.svelte";
    import GestionarProductos from "./views/GestionarProductos.svelte";
    import MisFacturas from "./views/MisFacturas.svelte";
    import VerTransacciones from "./views/VerTransacciones.svelte";


    const routes = {
        "/": Home,
        "/login": Login,
        "/admin": AdminUsuarios,
        "/agregar-usuario": AgregarUsuario,
        "/gestionar-productos": GestionarProductos,
        "/mis-facturas": MisFacturas,
        "/ver-transacciones": VerTransacciones,
        "/direcciones-usuario": DireccionesUsuario,
    };
</script>

<main>
    <Navbar />
    <Router {routes} />
</main>