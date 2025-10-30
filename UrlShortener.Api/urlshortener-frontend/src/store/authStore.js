import { defineStore } from "pinia";
import axios from "axios";

const API_BASE_URL = "https://localhost:7176/api"; // ⚠️ Dùng đúng port backend của bạn

export const useAuthStore = defineStore("auth", {
    state: () => ({
        user: null,
        token: localStorage.getItem("token") || null,
    }),

    actions: {
        async register(email, password) {
            try {
                const res = await axios.post(`${API_BASE_URL}/Auth/register`, {
                    email,
                    password,
                });
                console.log("Register success:", res.data);
                this.token = res.data.accessToken;
                localStorage.setItem("token", this.token);
                this.user = { email };
                return true;
            } catch (err) {
                // ✅ Ghi log lỗi chi tiết
                console.error("Register error:", err.response?.data || err.message);

                // ✅ Hiển thị lỗi thực tế (Identity lỗi password, email, v.v.)
                alert(
                    "Register failed: " +
                    (err.response?.data?.message ||
                        JSON.stringify(err.response?.data) ||
                        err.message)
                );
                throw err;
            }
        },

        async login(email, password) {
            try {
                const res = await axios.post(`${API_BASE_URL}/Auth/login`, {
                    email,
                    password,
                });
                console.log("Login success:", res.data);
                this.token = res.data.accessToken;
                localStorage.setItem("token", this.token);
                this.user = { email };
                return true;
            } catch (err) {
                console.error("Login error:", err.response?.data || err.message);
                alert(
                    "Login failed: " +
                    (err.response?.data?.message ||
                        JSON.stringify(err.response?.data) ||
                        err.message)
                );
                throw err;
            }
        },

        logout() {
            this.token = null;
            this.user = null;
            localStorage.removeItem("token");
        },
    },
});
