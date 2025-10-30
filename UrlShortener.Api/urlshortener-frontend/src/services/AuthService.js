import { defineStore } from "pinia";
import axios from "../axios"; // ✅ import đúng

export const useAuthStore = defineStore("auth", {
    state: () => ({
        user: null,
        token: localStorage.getItem("token") || null,
    }),

    actions: {
        async register(email, password) {
            try {
                const res = await axios.post("/Auth/register", { email, password });
                this.token = res.data.accessToken;
                localStorage.setItem("token", this.token);
                this.user = { email };
                console.log("Register success:", res.data);
            } catch (err) {
                console.error("Register error:", err.response?.data || err.message);
                alert(
                    "Register failed: " +
                    (err.response?.data?.message ||
                        JSON.stringify(err.response?.data) ||
                        err.message)
                );
            }
        },

        async login(email, password) {
            try {
                const res = await axios.post("/Auth/login", { email, password });
                this.token = res.data.accessToken;
                localStorage.setItem("token", this.token);
                this.user = { email };
                console.log("Login success:", res.data);
            } catch (err) {
                console.error("Login error:", err.response?.data || err.message);
                alert(
                    "Login failed: " +
                    (err.response?.data?.message ||
                        JSON.stringify(err.response?.data) ||
                        err.message)
                );
            }
        },
    },
});
