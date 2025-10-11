<script>
    import RutaProtegida from "./RutaProtegida.svelte";
    import MisFacturas from "./MisFacturas.svelte";
    import Router, { location } from "svelte-spa-router";
    import { wrap } from 'svelte-spa-router/wrap'; 
    import Navbar from "./Navbar.svelte";
    import Home from "./Home.svelte";
    import Login from "./Login.svelte";
    import AdminUsuarios from "./AdminUsuarios.svelte";
    import AgregarUsuario from "./AgregarUsuario.svelte";
    import GestionarProductos from "./GestionarProductos.svelte";
    import VerTransacciones from "./VerTransacciones.svelte";

    const routes = {
        "/": Home,
        "/login": Login,

        "/admin": wrap({
            component: AdminUsuarios,
            layout: RutaProtegida,
            conditions: [
                () => localStorage.getItem("tipoUsuario") === "Administrador",
            ],
        }),

        "/agregar-usuario": wrap({
            component: AgregarUsuario,
            layout: RutaProtegida,
            conditions: [
                () => localStorage.getItem("tipoUsuario") === "Administrador",
            ],
        }),

        "/gestionar-productos": wrap({
            component: GestionarProductos,
            layout: RutaProtegida,
            conditions: [
                () => localStorage.getItem("tipoUsuario") === "Proveedor",
            ],
        }),

        "/mis-facturas": wrap({
            component: MisFacturas,
            layout: RutaProtegida,
        }),

        "/ver-transacciones": wrap({
            component: VerTransacciones,
            layout: RutaProtegida,
        }),
    };
</script>

<main>
    <Navbar />
    <Router {routes} />
</main>
