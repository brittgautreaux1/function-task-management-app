<template>
  <div class="login-container">
    <h1>Task Manager</h1>
    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit" :disabled="loading || disableLogin">Login</button>
    </form>
    <p v-if="error" style="color: red;">{{ error }}</p>
    <p>Don't have an account? <router-link to="/register">Sign up</router-link></p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { computed } from 'vue';

const authStore = useAuthStore();

const router = useRouter();
const email = ref('');
const password = ref('');
const loading = ref(false);
const error = ref('');
const disableLogin = computed(() => !email.value || !password.value);

const handleLogin = async () => {
  loading.value = true;

  const result = await authStore.login(email.value, password.value);
  if (result) {
    router.push('/tasks');
  }
  else {
    error.value = authStore.error;
  }

  loading.value = false;
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