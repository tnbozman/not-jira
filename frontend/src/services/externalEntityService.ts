import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api';

export interface ExternalEntity {
  id?: number;
  type: 'Person' | 'Client';
  name: string;
  email?: string;
  organization?: string;
  phone?: string;
  notes?: string;
  projectId: number;
  problems?: Problem[];
  interviews?: Interview[];
  entityTags?: EntityTag[];
  createdAt?: string;
  updatedAt?: string;
}

export interface Problem {
  id?: number;
  description: string;
  severity: 'Low' | 'Medium' | 'High' | 'Critical';
  context?: string;
  externalEntityId: number;
  externalEntity?: ExternalEntity;
  outcomes?: Outcome[];
  problemTags?: ProblemTag[];
  createdAt?: string;
  updatedAt?: string;
}

export interface Outcome {
  id?: number;
  description: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  context?: string;
  externalEntityId: number;
  externalEntity?: ExternalEntity;
  problems?: Problem[];
  successMetrics?: SuccessMetric[];
  outcomeTags?: OutcomeTag[];
  createdAt?: string;
  updatedAt?: string;
}

export interface SuccessMetric {
  id?: number;
  description: string;
  targetValue?: string;
  currentValue?: string;
  unit?: string;
  outcomeId: number;
  outcome?: Outcome;
  createdAt?: string;
  updatedAt?: string;
}

export interface Interview {
  id?: number;
  type: 'Discovery' | 'Feedback' | 'Clarification';
  interviewDate: string;
  interviewer: string;
  summary?: string;
  externalEntityId: number;
  projectId: number;
  externalEntity?: ExternalEntity;
  notes?: InterviewNote[];
  createdAt?: string;
  updatedAt?: string;
}

export interface InterviewNote {
  id?: number;
  content: string;
  relatedProblemId?: number;
  relatedOutcomeId?: number;
  interviewId: number;
  interview?: Interview;
  relatedProblem?: Problem;
  relatedOutcome?: Outcome;
  createdAt?: string;
  updatedAt?: string;
}

export interface Tag {
  id?: number;
  name: string;
  description?: string;
  projectId: number;
  createdAt?: string;
}

export interface EntityTag {
  externalEntityId: number;
  tagId: number;
  tag?: Tag;
}

export interface ProblemTag {
  problemId: number;
  tagId: number;
  tag?: Tag;
}

export interface OutcomeTag {
  outcomeId: number;
  tagId: number;
  tag?: Tag;
}

class ExternalEntityService {
  private getToken(): string | null {
    return localStorage.getItem('keycloak_token');
  }

  private getHeaders() {
    const token = this.getToken();
    return {
      'Content-Type': 'application/json',
      ...(token ? { 'Authorization': `Bearer ${token}` } : {})
    };
  }

  // External Entities
  async getExternalEntities(projectId: number): Promise<ExternalEntity[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/external-entities`, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async getExternalEntity(projectId: number, id: number): Promise<ExternalEntity> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/external-entities/${id}`, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async createExternalEntity(projectId: number, entity: ExternalEntity): Promise<ExternalEntity> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/external-entities`, entity, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async updateExternalEntity(projectId: number, id: number, entity: ExternalEntity): Promise<void> {
    await axios.put(`${API_BASE_URL}/projects/${projectId}/external-entities/${id}`, entity, {
      headers: this.getHeaders()
    });
  }

  async deleteExternalEntity(projectId: number, id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/projects/${projectId}/external-entities/${id}`, {
      headers: this.getHeaders()
    });
  }

  // Problems
  async getProblems(projectId: number): Promise<Problem[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/problems`, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async createProblem(projectId: number, problem: Problem): Promise<Problem> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/problems`, problem, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  // Interviews
  async getInterviews(projectId: number): Promise<Interview[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/interviews`, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async getInterview(projectId: number, id: number): Promise<Interview> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/interviews/${id}`, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async createInterview(projectId: number, interview: Interview): Promise<Interview> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/interviews`, interview, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async addInterviewNote(projectId: number, interviewId: number, note: InterviewNote): Promise<InterviewNote> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/interviews/${interviewId}/notes`, note, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  // Tags
  async getTags(projectId: number): Promise<Tag[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/tags`, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async createTag(projectId: number, tag: Tag): Promise<Tag> {
    const response = await axios.post(`${API_BASE_URL}/projects/${projectId}/tags`, tag, {
      headers: this.getHeaders()
    });
    return response.data;
  }

  async searchTags(projectId: number, query: string): Promise<Tag[]> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/tags/search?query=${encodeURIComponent(query)}`, {
      headers: this.getHeaders()
    });
    return response.data;
  }
}

export default new ExternalEntityService();
