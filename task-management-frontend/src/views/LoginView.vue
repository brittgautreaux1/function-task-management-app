<template>
  <div class="login-container">
    <h1>Task Manager</h1>
    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit" :disabled="loading">Login</button>
    </form>
    <p v-if="error" style="color: red;">{{ error }}</p>
    <p>Don't have an account? <router-link to="/register">Sign up</router-link></p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';

const router = useRouter();
const email = ref('');
const password = ref('');
const loading = ref(false);
const error = ref('');

const handleLogin = async () => {
  loading.value = true;
  error.value = '';

  try {
    // TODO: Replace with your actual login endpoint when ready
    const response = await api.post('/auth/login', {
      email: email.value,
      password: password.value
    });
    
    localStorage.setItem('token', response.data.token);
    router.push('/tasks');
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Login failed';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.login-container {
  max-width: 400px;
  margin: 100px auto;
  padding: 2rem;
  text-align: center;
}

form input {
  width: 100%;
  margin: 12px 0;
  padding: 12px;
}

form button {
  width: 100%;
  padding: 12px;
  margin-top: 10px;
}
</style>