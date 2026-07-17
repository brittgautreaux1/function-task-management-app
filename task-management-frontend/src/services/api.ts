import axios from 'axios';
import { useRouter } from 'vue-router';

const api = axios.create({
  baseURL: 'http://localhost:5155/api',   // Update port if yours is different
  headers: {
    'Content-Type': 'application/json',
  },
});

const router = useRouter();

// Optional: Add token interceptor later
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor - handle 401
api.interceptors.response.use(
  response => response,
  error => {
    const isAuthEndpoint = error.config?.url?.includes('/login') || error.config?.url?.includes('/auth/register');
    if (error.response?.status === 401 && !isAuthEndpoint) {
      localStorage.removeItem('token');
      router.push('/login');
    }
    return Promise.reject(error);
  }
);

export default api;