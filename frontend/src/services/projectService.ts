import keycloakService from "./keycloakService";
import config from "@/config";

export interface Project {
  id?: number;
  key: string;
  name: string;
  description?: string;
  createdAt?: string;
  updatedAt?: string;
  members?: ProjectMember[];
}

export interface ProjectMember {
  id?: number;
  projectId?: number;
  userId: string;
  userName: string;
  userEmail: string;
  role: ProjectRole;
  addedAt?: string;
}

export enum ProjectRole {
  Developer = 0,
  ProductManager = 1,
  ProjectSponsor = 2,
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

export const projectService = {
  async getAllProjects(): Promise<Project[]> {
    const response = await fetch(`${API_BASE_URL}/projects`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch projects");
    }
    return response.json();
  },

  async getProject(id: number): Promise<Project> {
    const response = await fetch(`${API_BASE_URL}/projects/${id}`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch project");
    }
    return response.json();
  },

  async getProjectByKey(key: string): Promise<Project> {
    const response = await fetch(`${API_BASE_URL}/projects/by-key/${key}`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch project");
    }
    return response.json();
  },

  async createProject(project: Project): Promise<Project> {
    const response = await fetch(`${API_BASE_URL}/projects`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(project),
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || "Failed to create project");
    }
    return response.json();
  },

  async updateProject(id: number, project: Project): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${id}`, {
      method: "PUT",
      headers: await getHeaders(),
      body: JSON.stringify(project),
    });
    if (!response.ok) {
      throw new Error("Failed to update project");
    }
  },

  async deleteProject(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${id}`, {
      method: "DELETE",
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to delete project");
    }
  },

  async addMember(projectId: number, member: ProjectMember): Promise<ProjectMember> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/members`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(member),
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || "Failed to add member");
    }
    return response.json();
  },

  async updateMember(projectId: number, memberId: number, member: ProjectMember): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/members/${memberId}`, {
      method: "PUT",
      headers: await getHeaders(),
      body: JSON.stringify(member),
    });
    if (!response.ok) {
      throw new Error("Failed to update member");
    }
  },

  async removeMember(projectId: number, memberId: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/members/${memberId}`, {
      method: "DELETE",
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to remove member");
    }
  },
};
