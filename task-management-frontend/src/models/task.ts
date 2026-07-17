export interface Task {
    id: number;
    title: string;
    description?: string;
    isCompleted: boolean;
    dueDate?: string;
    createdAt?: string;
    updatedAt?: string;
    userId?: string;
}

export interface TaskForm {
    id?: number;
    title: string;
    description?: string;
    dueDate?: string;
}