<script>
	import DireccionesUsuario from './DireccionesUsuario.svelte';
    import Router from "svelte-spa-router";
    import Navbar from "./Navbar.svelte";
    import Home from "./Home.svelte";
    import Login from "./Login.svelte";
    import AdminUsuarios from "./AdminUsuarios.svelte";
    import AgregarUsuario from "./AgregarUsuario.svelte";
    import GestionarProductos from "./GestionarProductos.svelte";
    import MisFacturas from "./MisFacturas.svelte";
    import VerTransacciones from "./VerTransacciones.svelte";


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