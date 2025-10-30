<template>
    <div class="form-container">
        <h2>Register</h2>
        <input v-model="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />
        <button @click="onRegister">Register</button>
        <p>Already have an account? <router-link to="/login">Login</router-link></p>
    </div>
</template>

<script setup>
    import { ref } from 'vue';
    import { useAuthStore } from '../store/authStore';
    import { useRouter } from 'vue-router';

    const email = ref('');
    const password = ref('');
    const auth = useAuthStore();
    const router = useRouter();

    async function onRegister() {
        try {
            await auth.register(email.value, password.value);
            router.push('/dashboard');
        } catch {
            alert('Register failed!');
        }
    }
</script>
