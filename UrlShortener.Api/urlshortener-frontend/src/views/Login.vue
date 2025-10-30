<template>
    <div class="form-container">
        <h2>Login</h2>
        <input v-model="email" type="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />
        <button @click="onLogin">Login</button>
        <p>Don't have an account? <router-link to="/register">Register</router-link></p>
    </div>
</template>

<script setup>
    import { ref } from "vue";
    import { useRouter } from "vue-router";
    import { useAuthStore } from "../store/authStore";

    const email = ref("");
    const password = ref("");
    const router = useRouter();
    const auth = useAuthStore();

    async function onLogin() {
        try {
            await auth.login(email.value, password.value);
            alert("Login successful!");
            console.log("Logged in:", auth.user);
            router.push("/dashboard");
        } catch (error) {
            alert("Login failed!");
            console.error("Login failed:", error);
        }
    }
</script>

<style>
    .form-container {
        width: 300px;
        margin: 100px auto;
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    input {
        padding: 8px;
        font-size: 14px;
    }

    button {
        padding: 8px;
        background-color: #42b883;
        color: white;
        border: none;
        border-radius: 5px;
    }

        button:hover {
            background-color: #2a9d8f;
        }
</style>
