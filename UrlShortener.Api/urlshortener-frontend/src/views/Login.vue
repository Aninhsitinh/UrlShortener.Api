<template>
    <div class="form-container">
        <h2>Login</h2>
        <input v-model="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />
        <button @click="onLogin">Login</button>
        <p>Don't have an account? <router-link to="/register">Register</router-link></p>
    </div>
</template>

<script setup>
    import { ref } from 'vue';
    import { useAuthStore } from '../store/authStore';
    import { useRouter } from 'vue-router';

    const email = ref('');
    const password = ref('');
    const router = useRouter();
    const auth = useAuthStore();

    async function onLogin() {
        try {
            await auth.login(email.value, password.value);
            router.push('/dashboard');
        } catch {
            alert('Login failed!');
        }
    }
</script>
