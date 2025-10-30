import { defineStore } from "pinia";
import axios from "../axios"; // import đúng axios instance

const TOKEN_KEY = "token";

export const useAuthStore = defineStore("auth", {
    state: () => ({
        user: null,
        token: localStorage.getItem(TOKEN_KEY) || null,
    }),

    actions: {
        // --- REGISTER ---
        async register(email, password) {
            try {
                // Chú ý: path viết thường
                const res = await axios.post("/Auth/register", { email, password });

                this.token = res.data.token || res.data.accessToken; // backend trả token
                localStorage.setItem(TOKEN_KEY, this.token);
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

        // --- LOGIN ---
        async login(email, password) {
            try {
                // Chú ý: path viết thường
                const res = await axios.post("/Auth/login", { email, password });

                this.token = res.data.token || res.data.accessToken;
                localStorage.setItem(TOKEN_KEY, this.token);
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

        // --- LOGOUT ---
        logout() {
            this.token = null;
            this.user = null;
            localStorage.removeItem(TOKEN_KEY);
            console.log("Logged out");
        },

        // --- OPTIONAL: refresh token ---
        async refreshToken() {
            try {
                const refreshToken = localStorage.getItem("refreshToken");
                if (!refreshToken) throw new Error("No refresh token");

                const res = await axios.post("/auth/refresh", { refreshToken });
                this.token = res.data.token || res.data.accessToken;
                localStorage.setItem(TOKEN_KEY, this.token);

                console.log("Token refreshed:", res.data);
            } catch (err) {
                console.error("Refresh token failed:", err);
                this.logout();
            }
        },
    },
});
