import { defineStore } from 'pinia';
import api from '@/services/api';
import type { TaskForm, Task } from '@/models/task';

export const useTaskStore = defineStore('tasks', {
  state: () => ({
    tasks: [] as Task[],
    loading: false,
    error: ''
  }),
  actions: {
    async loadTasks() {
      this.loading = true;
      this.error = '';

      try {
        const response = await api.get('/tasks');
        this.tasks = response.data;
      } catch (err: any) {
        this.error = 'Failed to load tasks';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },
    async createTask(createTask: TaskForm) {
      this.loading = true;
      this.error = '';

      try {
        await api.post('/Tasks', createTask);
        await this.loadTasks();
        return true;
      } catch (err: any) {
        console.error(err.response?.data);
        this.error = err.response?.data || 'Create Task failed';
        return false;
      } finally {
        this.loading = false;
      }
    },
    async updateTask(updateTask: TaskForm, id: number) {

      this.loading = true;
      this.error = '';

      try {
        await api.put(`/Tasks/${id}`, updateTask);
        await this.loadTasks();
        return true;
      } catch (err: any) {
        console.error(err);
        this.error = err.response?.data || 'Update Task failed';
        return false;
      } finally {
        this.loading = false;
      }
    },
    async deleteTask(id: number) {
      this.loading = true;
      this.error = '';

      try {
        await api.delete(`/Tasks/${id}`);
        await this.loadTasks();
        return true;
      } catch (err) {
        console.error(err);
        return false;
      } finally {
        this.loading = false;
      }
    },
    async toggleComplete(id: number) {
      try {
        await api.patch(`/Tasks/${id}/complete`);
        await this.loadTasks();
        return true;
      } catch (err) {
        console.error(err);
        return false;
      }
    }
  }
});