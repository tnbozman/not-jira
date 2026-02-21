export interface TaskItem {
  id?: number
  title: string
  description?: string
  status: string
  priority: string
  createdAt?: string
  updatedAt?: string
}

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

export const taskService = {
  async getAllTasks(): Promise<TaskItem[]> {
    const response = await fetch(`${API_BASE_URL}/tasks`)
    if (!response.ok) {
      throw new Error('Failed to fetch tasks')
    }
    return response.json()
  },

  async getTask(id: number): Promise<TaskItem> {
    const response = await fetch(`${API_BASE_URL}/tasks/${id}`)
    if (!response.ok) {
      throw new Error('Failed to fetch task')
    }
    return response.json()
  },

  async createTask(task: TaskItem): Promise<TaskItem> {
    const response = await fetch(`${API_BASE_URL}/tasks`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(task),
    })
    if (!response.ok) {
      throw new Error('Failed to create task')
    }
    return response.json()
  },

  async updateTask(id: number, task: TaskItem): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(task),
    })
    if (!response.ok) {
      throw new Error('Failed to update task')
    }
  },

  async deleteTask(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
      method: 'DELETE',
    })
    if (!response.ok) {
      throw new Error('Failed to delete task')
    }
  },
}
