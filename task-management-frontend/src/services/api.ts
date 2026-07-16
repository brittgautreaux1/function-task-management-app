import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5155/api',   // Update port if yours is different
  headers: {
    'Content-Type': 'application/json',
  },
});

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
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;