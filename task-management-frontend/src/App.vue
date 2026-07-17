<template>
  <div class="app">
    <!-- Header -->
    <header class="header">
      <div class="header-content">
        <div class="logo-text">
          <h1>Functional Task Manager</h1>
        </div>

        <nav class="nav">
          <RouterLink v-if="authStore.isAuthenticated" to="/tasks" class="nav-link">Tasks</RouterLink>

          <template v-if="authStore.isAuthenticated && authStore.user">
            <span class="nav-link">Hi, {{ authStore.user?.firstName }}</span>
            <button @click="handleLogout" class="nav-link logout-btn">Logout</button>
          </template>

          <template v-else>
            <RouterLink to="/login" class="nav-link">Login</RouterLink>
            <RouterLink to="/register" class="nav-link">Register</RouterLink>
          </template>
        </nav>
      </div>
    </header>

    <!-- Main Content -->
    <main class="main-content">
      <RouterView />
    </main>
  </div>
</template>
<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

const authStore = useAuthStore();
const router = useRouter();

const handleLogout = async () => {
  await authStore.logout();
  router.push('/login');
}

</script>

<style scoped>
.app {
  min-height: 100vh;
  background-color: #f8fafc;
}

.header {
  background-color: white;
  border-bottom: 1px solid #e2e8f0;
  position: sticky;
  top: 0;
  z-index: 50;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 1rem 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 1rem;
}

.logo-text h1 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e2937;
}

.nav {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.nav-link {
  text-decoration: none;
  color: #64748b;
  font-weight: 500;
  transition: color 0.2s;
}

.logout-btn {
  color: #64748b;
  background-color: #f1f5f9;
}

.logout-btn:hover {
  color: #fff !important;
  background-color: #64748b;
  font-weight: normal !important;
}

.nav-link:hover,
.nav-link.router-link-exact-active {
  color: #3b82f6;
  font-weight: 600;
}

.main-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1.5rem;
  min-height: calc(100vh - 70px);
}

/* Responsive */
@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    text-align: center;
  }

  .nav {
    gap: 1rem;
    justify-content: center;
  }
}
</style>