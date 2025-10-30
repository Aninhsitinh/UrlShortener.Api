import { defineStore } from 'pinia';
import api from '../api/axios';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        accessToken: localStorage.getItem('accessToken') || '',
        refreshToken: localStorage.getItem('refreshToken') || '',
    }),
    actions: {
        async login(email, password) {
            const res = await api.post('/Auth/login', { email, password });
            this.accessToken = res.data.accessToken;
            this.refreshToken = res.data.refreshToken;
            localStorage.setItem('accessToken', this.accessToken);
            localStorage.setItem('refreshToken', this.refreshToken);
        },
        async register(email, password) {
            const res = await api.post('/Auth/register', { email, password });
            this.accessToken = res.data.accessToken;
            this.refreshToken = res.data.refreshToken;
            localStorage.setItem('accessToken', this.accessToken);
            localStorage.setItem('refreshToken', this.refreshToken);
        },
        logout() {
            this.accessToken = '';
            this.refreshToken = '';
            localStorage.clear();
        },
    },
});
