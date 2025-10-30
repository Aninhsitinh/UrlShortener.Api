<template>
    <div class="dashboard">
        <h2>Dashboard</h2>

        <div class="input-group">
            <input v-model="url" placeholder="Enter a URL to shorten" />
            <button @click="shortenUrl">Shorten</button>
        </div>

        <div v-if="links.length > 0" class="links">
            <h3>Your shortened links:</h3>
            <ul>
                <li v-for="(item, index) in links" :key="index">
                    <a :href="item.originalUrl" target="_blank">{{ item.originalUrl }}</a>
                    →
                    <a :href="`https://localhost:7176/r/${item.code}`"
                       target="_blank">
                        {{ `localhost:7176/r/${item.code}` }}
                    </a>
                </li>
            </ul>
        </div>

        <div v-else>
            <p>No links yet. Try shortening one!</p>
        </div>

        <button class="logout" @click="logout">Logout</button>
    </div>
</template>


<script setup>
    import { ref, onMounted } from "vue";
    import api from "../api/axios";
    import { useAuthStore } from "../store/authStore";
    import { useRouter } from "vue-router";

    const url = ref("");
    const links = ref([]);
    const auth = useAuthStore();
    const router = useRouter();

    async function shortenUrl() {
        try {
            const res = await api.post("/shorturl", { originalUrl: url.value });
            console.log("Shortened:", res.data);
            await loadLinks();
            url.value = "";
        } catch (err) {
            console.error("Shorten failed:", err.response?.data || err.message);
            alert("Failed to shorten URL");
        }
    }

    async function loadLinks() {
        try {
            const res = await api.get("/shorturl");
            console.log("Links from backend:", res.data);
            links.value = res.data;
        } catch (err) {
            console.error("Load links failed:", err);
        }
    }

    function logout() {
        auth.logout();
        router.push("/login");
    }

    onMounted(loadLinks);
</script>


<style>
    .dashboard {
        width: 500px;
        margin: 50px auto;
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    button {
        padding: 8px;
        background-color: #42b883;
        color: white;
        border: none;
        border-radius: 5px;
    }

    .logout {
        background-color: #ff5c5c;
    }

    input {
        padding: 8px;
    }

    ul {
        list-style: none;
        padding: 0;
    }
</style>
