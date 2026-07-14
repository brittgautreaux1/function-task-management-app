import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5274/api',   // Update port if yours is different
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

export default api;