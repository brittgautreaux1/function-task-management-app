<template>
    <div class="auth-container">
      <h1>Create Account</h1>
      <form @submit.prevent="handleRegister">
        <input 
          v-model="form.email" 
          type="email" 
          placeholder="Email" 
          required 
        />
        <input 
          v-model="form.password" 
          type="password" 
          placeholder="Password" 
          required 
        />
        <input 
          v-model="form.confirmPassword" 
          type="password" 
          placeholder="Confirm Password" 
          required 
        />
        
        <button type="submit" :disabled="loading">
          {{ loading ? 'Creating Account...' : 'Register' }}
        </button>
      </form>
  
      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="success" class="success">{{ success }}</p>
      
      <p>
        Already have an account? 
        <router-link to="/login">Login here</router-link>
      </p>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import api from '@/services/api';
  
  const router = useRouter();
  
  const form = ref({
    email: '',
    password: '',
    confirmPassword: ''
  });
  
  const loading = ref(false);
  const error = ref('');
  const success = ref('');
  
  const handleRegister = async () => {
    if (form.value.password !== form.value.confirmPassword) {
      error.value = "Passwords do not match";
      return;
    }
  
    loading.value = true;
    error.value = '';
    success.value = '';
  
    try {
      const response = await api.post('/auth/register', {
        email: form.value.email,
        password: form.value.password
      });
  
      success.value = "Account created successfully!";

      localStorage.setItem('token', response.data.token);
      setTimeout(() => {
        router.push('/tasks');
      }, 1500);
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed';
    } finally {
      loading.value = false;
    }
  };
  </script>
  
  <style scoped>
  .auth-container {
    max-width: 400px;
    margin: 80px auto;
    padding: 2rem;
    text-align: center;
  }
 
  .error { color: red; }
  .success { color: green; }


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