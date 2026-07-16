<template>
    <div class="tasks-page">
      <header class="header">
        <h1>My Tasks ({{ filteredTasks.length }})</h1>
        <button @click="showCreateModal = true" class="btn-primary">+ New Task</button>
      </header>
  
      <div class="controls">
        <input 
          v-model="searchQuery" 
          placeholder="Search tasks..." 
          class="search-input"
        />
        <select v-model="filterStatus" class="filter-select">
          <option value="all">All Tasks</option>
          <option value="active">Active</option>
          <option value="completed">Completed</option>
        </select>
      </div>
  
      <table class="tasks-table">
        <thead>
          <tr>
            <th width="60">Status</th>
            <th>Title</th>
            <th>Description</th>
            <th width="140">Due Date</th>
            <th width="160">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="task in paginatedTasks" :key="task.id" :class="{ completed: task.isCompleted }">
            <td>
              <input 
                type="checkbox" 
                :checked="task.isCompleted"
                @change="taskStore.toggleComplete(task.id)"
              />
            </td>
            <td><strong>{{ task.title }}</strong></td>
            <td>{{ task.description || '—' }}</td>
            <td>
              <span v-if="task.dueDate" :class="getDueDateClass(task.dueDate)">
                {{ formatDate(task.dueDate) }}
              </span>
              <span v-else class="no-date">—</span>
            </td>
            <td>
              <button @click="editTask(task)" class="btn-small edit">Edit</button>
              <button @click="taskStore.deleteTask(task.id)" class="btn-small delete">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
  
      <!-- Pagination -->
      <div class="pagination" v-if="totalPages > 1">
        <button @click="prevPage" :disabled="currentPage === 1">← Previous</button>
        <span class="page-info">Page {{ currentPage }} of {{ totalPages }}</span>
        <button @click="nextPage" :disabled="currentPage === totalPages">Next →</button>
      </div>
  
      <!-- Empty State -->
      <div v-if="filteredTasks.length === 0" class="empty-state">
        No tasks found.
      </div>
    </div>
    <!-- Modal -->
    <CreateTaskModal 
      v-if="showCreateModal" 
      v-model:show-create-modal="showCreateModal"
      @close="showCreateModal = false"
    />
  </template>
  
  <script setup lang="ts">
  import { ref, computed, onMounted } from 'vue';
  import { useTaskStore } from '@/stores/taskStore';
  import CreateTaskModal from '@/components/tasks/CreateTaskModal.vue'
  
  const taskStore = useTaskStore();
  
  const searchQuery = ref('');
  const filterStatus = ref<'all' | 'active' | 'completed'>('all');
  const currentPage = ref(1);
  const itemsPerPage = 10;
  const showCreateModal = ref(false);
  
  const filteredTasks = computed(() => {
    let result = [...taskStore.tasks];
  
    // Filter by status
    if (filterStatus.value === 'active') {
      result = result.filter(t => !t.isCompleted);
    } else if (filterStatus.value === 'completed') {
      result = result.filter(t => t.isCompleted);
    }
  
    // Search
    if (searchQuery.value.trim()) {
      const q = searchQuery.value.toLowerCase().trim();
      result = result.filter(t => 
        t.title.toLowerCase().includes(q) || 
        (t.description && t.description.toLowerCase().includes(q))
      );
    }
  
    return result;
  });
  
  const totalPages = computed(() => Math.ceil(filteredTasks.value.length / itemsPerPage));
  
  const paginatedTasks = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage;
    return filteredTasks.value.slice(start, start + itemsPerPage);
  });
  
  const prevPage = () => { if (currentPage.value > 1) currentPage.value--; };
  const nextPage = () => { if (currentPage.value < totalPages.value) currentPage.value++; };
  
  const formatDate = (dateStr: string) => new Date(dateStr).toLocaleDateString();
  const getDueDateClass = (dueDate: string) => {
    const due = new Date(dueDate);
    const today = new Date();
    today.setHours(0,0,0,0);
    return due < today ? 'overdue' : 'due-soon';
  };
  
  const editTask = (task: any) => {
    alert(`Edit functionality coming soon: ${task.title}`);
  };
  
  onMounted(() => {
    taskStore.loadTasks();
  });
  </script>
  
  <style scoped>
  .tasks-page { padding: 2rem; max-width: 1200px; margin: 0 auto; }
  
  .header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
  }
  
  .controls {
    display: flex;
    gap: 1rem;
    margin-bottom: 1.5rem;
  }
  
  .search-input, .filter-select {
    padding: 10px 14px;
    border: 2px solid #ddd;
    border-radius: 8px;
    font-size: 1rem;
  }
  
  .tasks-table {
    width: 100%;
    border-collapse: collapse;
    background: white;
    border-radius: 12px;
    overflow: hidden;
    box-shadow: 0 4px 15px rgba(0,0,0,0.08);
  }
  
  .tasks-table th, .tasks-table td {
    padding: 14px 16px;
    text-align: left;
    border-bottom: 1px solid #eee;
  }
  
  .tasks-table th {
    background: #f8f9fa;
    font-weight: 600;
  }
  
  .completed {
    text-decoration: line-through;
    opacity: 0.75;
  }
  
  .overdue { color: #dc3545; font-weight: 500; }
  .due-soon { color: #ffc107; }
  
  .btn-small {
    padding: 6px 12px;
    margin: 0 4px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 0.9rem;
  }
  
  .edit { background: #17a2b8; color: white; }
  .delete { background: #dc3545; color: white; }
  
  .pagination {
    margin-top: 2rem;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 1rem;
  }
  
  .empty-state {
    text-align: center;
    padding: 4rem 2rem;
    color: #666;
    font-size: 1.1rem;
  }
  </style>