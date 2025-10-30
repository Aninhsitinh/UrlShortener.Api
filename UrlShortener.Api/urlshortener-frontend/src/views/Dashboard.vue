<template>
    <div class="dashboard">
        <h2>Dashboard</h2>
        <input v-model="url" placeholder="Enter a URL" />
        <button @click="shorten">Shorten</button>
        <ul>
            <li v-for="item in links" :key="item.id">
                <a :href="item.originalUrl" target="_blank">{{ item.originalUrl }}</a> →
                <a :href="`http://localhost:5000/r/${item.code}`" target="_blank">
                    {{ `localhost:5000/r/${item.code}` }}
                </a>
            </li>
        </ul>
        <button @click="logout">Logout</button>
    </div>
</template>

<script setup>
    import { ref, onMounted } from 'vue';
    import api from '../api/axios';
    import { useAuthStore } from '../store/authStore';
    import { useRouter } from 'vue-router';

    const url = ref('');
    const links = ref([]);
    const auth = useAuthStore();
    const router = useRouter();

    async function shorten() {
        await api.post('/api/shorturl', { originalUrl: url.value });
        await loadLinks();
    }

    async function loadLinks() {
        const res = await api.get('/api/shorturl');
        links.value = res.data;
    }

    function logout() {
        auth.logout();
        router.push('/login');
    }

    onMounted(loadLinks);
</script>
