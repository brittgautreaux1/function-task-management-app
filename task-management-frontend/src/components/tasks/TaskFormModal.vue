<template>
  <div v-if="showEditModal" class="modal-overlay">
    <div class="modal-content">
      <h2>{{ isEdit ? 'Edit Task' : 'New Task' }}</h2>

      <form @submit.prevent="saveTask">
        <input v-model="form.title" placeholder="Task title *" required />

        <textarea v-model="form.description" placeholder="Description (optional)" rows="4"></textarea>

        <input v-model="form.dueDate" type="date" />

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
import type { TaskForm } from '@/types/task';

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