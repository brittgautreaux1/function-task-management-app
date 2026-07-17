<template>
    <Teleport to="body">
      <div v-if="isLoading" class="modal-overlay">
        <div class="modal-content">
          <div class="spinner"></div>
          <p>Loading, please wait...</p>
        </div>
      </div>
    </Teleport>
  </template>
  
  <script setup lang="ts">
  
  // Control the visibility of the modal
  const isLoading = defineProps<{isLoading: boolean}>();

  </script>
  
  <style scoped>
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
  }
  
  .modal-content {
    background: white;
    padding: 2rem;
    border-radius: 8px;
    text-align: center;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
  
  .spinner {
    border: 4px solid rgba(0, 0, 0, 0.1);
    width: 40px;
    height: 40px;
    border-radius: 50%;
    border-left-color: #42b883; /* Vue Green */
    animation: spin 1s linear infinite;
    margin: 0 auto 1rem;
  }
  
  @keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
  }
  </style>
  