import { defineStore } from 'pinia';
import api from '@/services/api';
import type { TaskForm, Task } from '@/models/task';
import { parseApiError } from '@/utils/api';

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
        console.log(err);
        this.error = parseApiError(err);
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
        console.log(err);
        this.error = parseApiError(err);
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
        console.log(err);
        this.error = parseApiError(err);
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
      } catch (err: any) {
        console.log(err);
        this.error = parseApiError(err);
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
      } catch (err: any) {
        console.log(err);
        this.error = parseApiError(err);
        return false;
      }
    }
  }
});