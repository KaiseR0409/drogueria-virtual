<script>
	import { onMount } from 'svelte';
    import {push} from 'svelte-spa-router';

    let token = localStorage.getItem("token");
    onMount(() => {
        if (!token) {
            push('/login');
        }
    });
</script>
{#if localStorage.getItem("token")}
    <slot></slot>
{/if}