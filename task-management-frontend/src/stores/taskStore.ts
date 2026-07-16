import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';
import type { CreateTaskForm, Task } from '@/models/task';

export const useTaskStore = defineStore('tasks', () => {
  const tasks = ref<Task[]>([]);
  const loading = ref(false);
  const error = ref('');

  const loadTasks = async () => {
    loading.value = true;
    error.value = '';
    try {
      const response = await api.get('/tasks');
      tasks.value = response.data;
    } catch (err: any) {
      error.value = 'Failed to load tasks';
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const toggleComplete = async (id: number) => {
    try {
      await api.patch(`/tasks/${id}/complete`);
      await loadTasks();
    } catch (err) {
      console.error(err);
    }
  };

  const deleteTask = async (id: number) => {
    if (!confirm('Delete this task?')) return;
    try {
      await api.delete(`/tasks/${id}`);
      await loadTasks();
    } catch (err) {
      console.error(err);
    }
  };

  const createTask = async (createTask: CreateTaskForm) => {
    try {
      await api.post('/CreateTask', createTask);
      await loadTasks();
    } catch (err) {
      console.error(err);
    }
  };

  return {
    tasks,
    loading,
    error,
    loadTasks,
    toggleComplete,
    deleteTask,
    createTask
  };
});