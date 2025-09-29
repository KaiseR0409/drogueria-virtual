<script>
    import Router, { location } from 'svelte-spa-router';
    import Navbar from "./Navbar.svelte";
    import Home from "./Home.svelte";
    import Login from "./Login.svelte";
    import AdminUsuarios from "./AdminUsuarios.svelte";
    import AgregarUsuario from './AgregarUsuario.svelte';
    import GestionarProductos from './GestionarProductos.svelte';

    const routes = {
        '/': Home,
        '/login': Login,
        '/admin': AdminUsuarios,
        '/agregar-usuario': AgregarUsuario,
        '/gestionar-productos': GestionarProductos,
    };
</script>

<main>
    <Navbar />
    <Router {routes} />
</main>