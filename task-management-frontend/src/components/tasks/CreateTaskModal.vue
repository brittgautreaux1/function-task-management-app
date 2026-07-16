<template>
    <!-- Create Task Modal -->
    <div v-if="showCreateModal" class="modal-overlay">
        <div class="modal-content">
          <h2>New Task</h2>
          
          <form @submit.prevent="createNewTask">
            <input 
              v-model="newTask.title" 
              placeholder="Task title *" 
              required 
            />
            
            <textarea 
              v-model="newTask.description" 
              placeholder="Description (optional)"
              rows="4"
            ></textarea>
            
            <input 
              v-model="newTask.dueDate" 
              type="date"
              placeholder="Due date"
            />
            
            <div class="modal-actions">
              <button type="button" @click="closeModal" class="btn-secondary">
                Cancel
              </button>
              <button type="submit" :disabled="submitting || !newTask.title.trim()" class="btn-primary">
                {{ submitting ? 'Creating...' : 'Create Task' }}
              </button>
            </div>
          </form>
        </div>
      </div>
</template>
<script setup lang="ts">
import { computed, ref } from 'vue';
import { useTaskStore } from '@/stores/taskStore';
import type { CreateTaskForm } from '@/models/task';

const taskStore = useTaskStore();

const showCreateModal = defineModel<boolean>('showCreateModal');

// maybe use a type here (CreateTaskDTO?)
const newTask = ref<CreateTaskForm>({
    title: '',
    description: '',
    dueDate: ''
});
const submitting = ref(false);

const createNewTask = async () => {
    if (!newTask.value.title.trim())
        return;

    submitting.value = true;

    try {
        await taskStore.createTask({
            title: newTask.value.title.trim(),
            description: newTask.value.description?.trim() || undefined,
            dueDate: newTask.value.dueDate || undefined
        });

        closeModal();
    } catch (err) {
        alert('Failed to create task. Please try again.');
    } finally {
        submitting.value = false;
    }
}

const closeModal = () => {
    newTask.value = { title: '', description: '', dueDate: '' };
    showCreateModal.value = false;
}
</script>
<style>
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  width: 90%;
  max-width: 480px;
}

.modal-content input,
.modal-content textarea {
  width: 100%;
  padding: 12px 14px;
  margin: 10px 0;
  border: 2px solid #ddd;
  border-radius: 8px;
  font-size: 1rem;
}

.modal-actions {
  margin-top: 20px;
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}
</style>