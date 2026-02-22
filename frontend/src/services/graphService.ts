import axios from "axios";
import keycloakService from "./keycloakService";
import config from "@/config";

const API_BASE_URL = config.API_BASE_URL;

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
  private getHeaders() {
    const token = keycloakService.getToken();
    return {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    };
  }

  async getGraphData(projectId: number): Promise<GraphData> {
    const response = await axios.get(`${API_BASE_URL}/projects/${projectId}/graph`, {
      headers: this.getHeaders(),
    });
    return response.data;
  }
}

export default new GraphService();
