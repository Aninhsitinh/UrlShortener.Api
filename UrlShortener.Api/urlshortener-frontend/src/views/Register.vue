<template>
    <div class="form-container">
        <h2>Register</h2>
        <input v-model="email" type="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />
        <button @click="onRegister">Register</button>
        <p>Already have an account? <router-link to="/login">Login</router-link></p>
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

async function onRegister() {
  try {
    await auth.register(email.value, password.value);
    router.push("/dashboard");
  } catch {
    alert("Register failed!");
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
