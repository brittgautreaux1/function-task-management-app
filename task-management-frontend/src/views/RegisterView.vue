<template>
  <div class="auth-container">
    <h1>Create Account</h1>
    <form @submit.prevent="handleRegister">
      <input v-model="form.firstName" type="text" placeholder="First Name" required />
      <input v-model="form.lastName" type="text" placeholder="Last Name" required />
      <input v-model="form.email" type="email" placeholder="Email" required />
      <input v-model="form.password" type="password" placeholder="Password" required />
      <input v-model="form.confirmPassword" type="password" placeholder="Confirm Password" required />

      <button type="submit" :disabled="loading || disableRegister">
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
import { useAuthStore } from '@/stores/authStore';
import { computed } from 'vue';

const authStore = useAuthStore();
const router = useRouter();

const disableRegister = computed(() => !form.value.firstName || !form.value.lastName || !form.value.email || !form.value.password || !form.value.confirmPassword)

const form = ref({
  firstName: '',
  lastName: '',
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

  const result = await authStore.register(form.value.firstName, form.value.lastName, form.value.email, form.value.password)

  if (result) {
    success.value = "Account created successfully!";

    router.push('/tasks');
  }
  else {
    //todo: handle existing email
    console.log(authStore.error);
    error.value = authStore.error || 'Registration failed';
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

.error {
  color: red;
}

.success {
  color: green;
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