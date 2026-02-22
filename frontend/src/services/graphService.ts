import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api';

export interface GraphNode {
  id: string;
  label: string;
  type: string;
  data?: any;
}

export interface GraphEdge {
  id: string;
  source: string;
  target: string;
  label: string;
}

export interface GraphStats {
  entityCount: number;
  problemCount: number;
  outcomeCount: number;
  interviewCount: number;
}

export interface GraphData {
  nodes: GraphNode[];
  edges: GraphEdge[];
  stats?: GraphStats;
}

class GraphService {
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

  async getGraphData(projectId: number): Promise<GraphData> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/graph`, {
      headers: this.getHeaders()
    });
    return response.data;
  }
}

export default new GraphService();
