import keycloakService from "./keycloakService";
import config from "@/config";

export interface TaskItem {
  id?: number;
  title: string;
  description?: string;
  status: string;
  priority: string;
  createdAt?: string;
  updatedAt?: string;
}

const API_BASE_URL = config.API_BASE_URL;

async function getHeaders(): Promise<HeadersInit> {
  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };

  const token = keycloakService.getToken();
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  return headers;
}

export const taskService = {
  async getAllTasks(): Promise<TaskItem[]> {
    const response = await fetch(`${API_BASE_URL}/tasks`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch tasks");
    }
    return response.json();
  },

  async getTask(id: number): Promise<TaskItem> {
    const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch task");
    }
    return response.json();
  },

  async createTask(task: TaskItem): Promise<TaskItem> {
    const response = await fetch(`${API_BASE_URL}/tasks`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(task),
    });
    if (!response.ok) {
      throw new Error("Failed to create task");
    }
    return response.json();
  },

  async updateTask(id: number, task: TaskItem): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
      method: "PUT",
      headers: await getHeaders(),
      body: JSON.stringify(task),
    });
    if (!response.ok) {
      throw new Error("Failed to update task");
    }
  },

  async deleteTask(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
      method: "DELETE",
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to delete task");
    }
  },
};
