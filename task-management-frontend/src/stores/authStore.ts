import { defineStore } from 'pinia';
import api from '@/services/api';

interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
}

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: {} as User | null,
        token: localStorage.getItem('token') || null,
        error: ''
    }),
    getters: {
        isAuthenticated: (state) => !!state.token && !!state.user
    },
    actions: {
        async restoreUser() {
            if (!this.token) return false;

            try {
                const response = await api.get('/auth/user');  // Use your existing endpoint
                this.user = response.data;
                return true;
            } catch (err) {
                console.log('Failed to restore user');
                this.logout();  // Clear invalid token
                return false;
            }
        },
        async login(email: string, password: string) {
            this.error = '';

            try {
                const response = await api.post('/auth/login', { email, password });

                this.user = response.data.user;
                this.token = response.data.token;

                localStorage.setItem('token', response.data.token);

                return true;
            } catch (err: any) {
                this.error = err.response?.data?.message || 'Login failed';
                return false;
            }
        },
        async register(firstName: string, lastName: string, email: string, password: string) {
            this.error = '';

            try {
                const response = await api.post('/auth/register', { firstName, lastName, email, password });
                this.user = response.data.user;
                this.token = response.data.token;

                localStorage.setItem('token', response.data.token);

                return true;
            } catch (err: any) {
                console.log(err.response);
                this.error = err.response?.data || 'Registration failed';
                return false;
            }
        },
        logout() {
            this.token = null;
            this.user = null;
            localStorage.removeItem('token');
        }
    }
});