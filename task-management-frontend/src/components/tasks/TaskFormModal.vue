<template>
  <div v-if="showEditModal" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>{{ isEdit ? 'Edit Task' : 'New Task' }}</h2>
        <button @click="closeModal" class="close-btn">✕</button>
      </div>

      <form @submit.prevent="saveTask" class="modal-form">
        <div class="form-group">
          <label>Task Title <span class="required">*</span></label>
          <input v-model="form.title" placeholder="What needs to be done?" required />
        </div>

        <div class="form-group">
          <label>Description</label>
          <textarea v-model="form.description" placeholder="Additional details (optional)" rows="4"></textarea>
        </div>

        <div class="form-group">
          <label>Due Date <span class="required">*</span></label>
          <input v-model="form.dueDate" type="date" required />
        </div>

        <div class="modal-actions">
          <button type="button" @click="closeModal" class="btn-secondary">
            Cancel
          </button>
          <button type="submit" :disabled="submitting || !form.title?.trim() || !form.dueDate" class="btn-primary">
            {{ submitting ? (isEdit ? 'Updating...' : 'Creating...') : (isEdit ? 'Update Task' : 'Create Task') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useTaskStore } from '@/stores/taskStore';
import type { TaskForm } from '@/models/task';

const taskStore = useTaskStore();

const showEditModal = defineModel<boolean>('showEditModal', { required: true });
const isEdit = defineModel<boolean>('isEdit', { default: false });
const taskToEdit = defineModel<any>('taskToEdit');

const form = ref<TaskForm>({
  title: '',
  description: '',
  dueDate: ''
});

const submitting = ref(false);

const saveTask = async () => {
  if (!form.value.title?.trim()) return;

  submitting.value = true;

  let result = false;

  if (isEdit.value && taskToEdit.value) {
    result = await taskStore.updateTask(form.value, taskToEdit.value.id);
  } else {
    result = await taskStore.createTask(form.value);
  }

  if (!result) {
    alert(taskStore.error || 'Save failed');
  }

  submitting.value = false;
  closeModal();
};

const closeModal = () => {
  form.value = { title: '', description: '', dueDate: '' };
  showEditModal.value = false;
};

// Populate form when editing
watch(taskToEdit, (newTask) => {
  if (newTask) {
    form.value = {
      title: newTask.title,
      description: newTask.description || '',
      dueDate: newTask.dueDate ? newTask.dueDate.split('T')[0] : ''
    };
  }
}, { immediate: true });
</script>
<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  overflow: auto;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 12px;
  width: 100%;
  max-width: 480px;
  box-shadow: 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1);
  overflow: hidden;
}

.modal-header {
  padding: 1.5rem 1.5rem 0.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #e2e8f0;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.25rem;
  color: #1e2937;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #94a3b8;
  padding: 0.25rem;
}

.close-btn:hover {
  color: #ef4444;
}

.modal-form {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.25rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #475569;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 1rem;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1.5rem;
}

.btn-secondary {
  padding: 0.75rem 1.25rem;
  background: #f1f5f9;
  color: #475569;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
}

.btn-primary {
  padding: 0.75rem 1.5rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
}

.btn-primary:disabled {
  background: #94a3b8;
  cursor: not-allowed;
}
</style>