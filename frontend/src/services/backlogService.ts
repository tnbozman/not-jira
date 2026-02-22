import axios from "axios";
import keycloakService from "./keycloakService";
import config from "@/config";

const API_BASE_URL = config.API_BASE_URL;

export interface BacklogItemDto {
  id: number;
  type: string;
  title: string;
  description?: string;
  order: number;
  priority: string;
  status: string;
  storyPoints?: number;
  epicId: number;
  epicName?: string;
  sprintId?: number;
  sprintName?: string;
  teamId?: number;
  teamName?: string;
  releaseId?: number;
  releaseName?: string;
  assigneeId?: string;
  assigneeName?: string;
}

export interface SprintGroup {
  sprintId: number;
  sprintName: string;
  sprintGoal?: string;
  startDate: string;
  endDate: string;
  status: string;
  items: BacklogItemDto[];
}

export interface BacklogResponse {
  sprints: SprintGroup[];
  backlogItems: BacklogItemDto[];
}

export interface TeamDto {
  id: number;
  name: string;
  description?: string;
}

export interface ReleaseDto {
  id: number;
  name: string;
  description?: string;
  status: string;
  startDate?: string;
  releaseDate?: string;
}

export interface BacklogFilters {
  teamId?: number | null;
  assigneeId?: string | null;
  releaseId?: number | null;
  epicId?: number | null;
}

class BacklogService {
  private getHeaders() {
    const token = keycloakService.getToken();
    return {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    };
  }

  async getBacklog(projectId: number, filters?: BacklogFilters): Promise<BacklogResponse> {
    const params = new URLSearchParams();
    if (filters?.teamId) params.append("teamId", filters.teamId.toString());
    if (filters?.assigneeId) params.append("assigneeId", filters.assigneeId);
    if (filters?.releaseId) params.append("releaseId", filters.releaseId.toString());
    if (filters?.epicId) params.append("epicId", filters.epicId.toString());

    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/backlog?${params}`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async getTeams(projectId: number): Promise<TeamDto[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/teams`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async createTeam(projectId: number, team: Partial<TeamDto>): Promise<TeamDto> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/teams`, team, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async deleteTeam(projectId: number, teamId: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/projects/${projectId}/teams/${teamId}`, {
      headers: this.getHeaders(),
    });
  }

  async getReleases(projectId: number): Promise<ReleaseDto[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/releases`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async createRelease(projectId: number, release: Partial<ReleaseDto>): Promise<ReleaseDto> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/releases`, release, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async deleteRelease(projectId: number, releaseId: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/projects/${projectId}/releases/${releaseId}`, {
      headers: this.getHeaders(),
    });
  }

  async getThemes(projectId: number): Promise<any[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/themes`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async getProject(projectId: number): Promise<any> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async createSprint(projectId: number, sprint: any): Promise<any> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/sprints`, sprint, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async getSprintPlanning(projectId: number, sprintId: number): Promise<any> {
    const response = await axios.get(
      `${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}/planning`,
      { headers: this.getHeaders() },
    );
    return response.data;
  }

  async saveSprintPlanning(projectId: number, sprintId: number, data: any): Promise<void> {
    await axios.put(`${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}/planning`, data, {
      headers: this.getHeaders(),
    });
  }

  async getSprint(projectId: number, sprintId: number): Promise<any> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }

  async saveSprintReview(projectId: number, sprintId: number, notes: string): Promise<void> {
    await axios.put(
      `${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}/review`,
      { notes },
      { headers: this.getHeaders() },
    );
  }

  async saveSprintRetro(projectId: number, sprintId: number, notes: string): Promise<void> {
    await axios.put(
      `${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}/retro`,
      { notes },
      { headers: this.getHeaders() },
    );
  }
}

export const backlogService = new BacklogService();
