import keycloakService from "./keycloakService";
import config from "@/config";

export interface Theme {
  id?: number;
  name: string;
  description?: string;
  order: number;
  projectId?: number;
  outcomeId?: number;
  outcome?: any;
  createdAt?: string;
  updatedAt?: string;
  epics?: Epic[];
}

export interface Epic {
  id?: number;
  name: string;
  description?: string;
  order: number;
  themeId?: number;
  outcomeId?: number;
  outcome?: any;
  createdAt?: string;
  updatedAt?: string;
  stories?: Story[];
  spikes?: Spike[];
}

export interface Story {
  id?: number;
  title: string;
  description?: string;
  solutionDescription?: string;
  acceptanceCriteria?: string;
  order: number;
  priority: Priority;
  status: string;
  storyPoints?: number;
  epicId?: number;
  sprintId?: number;
  outcomeId?: number;
  outcome?: any;
  sprint?: Sprint;
  createdAt?: string;
  updatedAt?: string;
}

export interface Spike {
  id?: number;
  title: string;
  description?: string;
  investigationGoal?: string;
  findings?: string;
  order: number;
  priority: Priority;
  status: string;
  storyPoints?: number;
  epicId?: number;
  sprintId?: number;
  outcomeId?: number;
  outcome?: any;
  sprint?: Sprint;
  createdAt?: string;
  updatedAt?: string;
}

export interface Sprint {
  id?: number;
  name: string;
  goal?: string;
  startDate: string;
  endDate: string;
  status: string;
  projectId?: number;
  createdAt?: string;
  updatedAt?: string;
  stories?: Story[];
  spikes?: Spike[];
}

export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3,
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

export const userStoryMapService = {
  // Theme operations
  async getThemes(projectId: number): Promise<Theme[]> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch themes");
    }
    return response.json();
  },

  async getTheme(projectId: number, themeId: number): Promise<Theme> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes/${themeId}`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch theme");
    }
    return response.json();
  },

  async createTheme(projectId: number, theme: Theme): Promise<Theme> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(theme),
    });
    if (!response.ok) {
      throw new Error("Failed to create theme");
    }
    return response.json();
  },

  async updateTheme(projectId: number, themeId: number, theme: Theme): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes/${themeId}`, {
      method: "PUT",
      headers: await getHeaders(),
      body: JSON.stringify(theme),
    });
    if (!response.ok) {
      throw new Error("Failed to update theme");
    }
  },

  async deleteTheme(projectId: number, themeId: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes/${themeId}`, {
      method: "DELETE",
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to delete theme");
    }
  },

  // Epic operations
  async getEpics(projectId: number, themeId: number): Promise<Epic[]> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes/${themeId}/epics`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch epics");
    }
    return response.json();
  },

  async getEpic(projectId: number, themeId: number, epicId: number): Promise<Epic> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/themes/${themeId}/epics/${epicId}`,
      {
        headers: await getHeaders(),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to fetch epic");
    }
    return response.json();
  },

  async createEpic(projectId: number, themeId: number, epic: Epic): Promise<Epic> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/themes/${themeId}/epics`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(epic),
    });
    if (!response.ok) {
      throw new Error("Failed to create epic");
    }
    return response.json();
  },

  async updateEpic(projectId: number, themeId: number, epicId: number, epic: Epic): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/themes/${themeId}/epics/${epicId}`,
      {
        method: "PUT",
        headers: await getHeaders(),
        body: JSON.stringify(epic),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to update epic");
    }
  },

  async deleteEpic(projectId: number, themeId: number, epicId: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/themes/${themeId}/epics/${epicId}`,
      {
        method: "DELETE",
        headers: await getHeaders(),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to delete epic");
    }
  },

  // Story operations
  async getStories(projectId: number, epicId: number): Promise<Story[]> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/epics/${epicId}/stories`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch stories");
    }
    return response.json();
  },

  async getStory(projectId: number, epicId: number, storyId: number): Promise<Story> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/epics/${epicId}/stories/${storyId}`,
      {
        headers: await getHeaders(),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to fetch story");
    }
    return response.json();
  },

  async createStory(projectId: number, epicId: number, story: Story): Promise<Story> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/epics/${epicId}/stories`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(story),
    });
    if (!response.ok) {
      throw new Error("Failed to create story");
    }
    return response.json();
  },

  async updateStory(
    projectId: number,
    epicId: number,
    storyId: number,
    story: Story,
  ): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/epics/${epicId}/stories/${storyId}`,
      {
        method: "PUT",
        headers: await getHeaders(),
        body: JSON.stringify(story),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to update story");
    }
  },

  async deleteStory(projectId: number, epicId: number, storyId: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/epics/${epicId}/stories/${storyId}`,
      {
        method: "DELETE",
        headers: await getHeaders(),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to delete story");
    }
  },

  // Spike operations
  async getSpikes(projectId: number, epicId: number): Promise<Spike[]> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/epics/${epicId}/spikes`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch spikes");
    }
    return response.json();
  },

  async getSpike(projectId: number, epicId: number, spikeId: number): Promise<Spike> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/epics/${epicId}/spikes/${spikeId}`,
      {
        headers: await getHeaders(),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to fetch spike");
    }
    return response.json();
  },

  async createSpike(projectId: number, epicId: number, spike: Spike): Promise<Spike> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/epics/${epicId}/spikes`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(spike),
    });
    if (!response.ok) {
      throw new Error("Failed to create spike");
    }
    return response.json();
  },

  async updateSpike(
    projectId: number,
    epicId: number,
    spikeId: number,
    spike: Spike,
  ): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/epics/${epicId}/spikes/${spikeId}`,
      {
        method: "PUT",
        headers: await getHeaders(),
        body: JSON.stringify(spike),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to update spike");
    }
  },

  async deleteSpike(projectId: number, epicId: number, spikeId: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/projects/${projectId}/epics/${epicId}/spikes/${spikeId}`,
      {
        method: "DELETE",
        headers: await getHeaders(),
      },
    );
    if (!response.ok) {
      throw new Error("Failed to delete spike");
    }
  },

  // Sprint operations
  async getSprints(projectId: number): Promise<Sprint[]> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/sprints`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch sprints");
    }
    return response.json();
  },

  async getSprint(projectId: number, sprintId: number): Promise<Sprint> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}`, {
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to fetch sprint");
    }
    return response.json();
  },

  async createSprint(projectId: number, sprint: Sprint): Promise<Sprint> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/sprints`, {
      method: "POST",
      headers: await getHeaders(),
      body: JSON.stringify(sprint),
    });
    if (!response.ok) {
      throw new Error("Failed to create sprint");
    }
    return response.json();
  },

  async updateSprint(projectId: number, sprintId: number, sprint: Sprint): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}`, {
      method: "PUT",
      headers: await getHeaders(),
      body: JSON.stringify(sprint),
    });
    if (!response.ok) {
      throw new Error("Failed to update sprint");
    }
  },

  async deleteSprint(projectId: number, sprintId: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/projects/${projectId}/sprints/${sprintId}`, {
      method: "DELETE",
      headers: await getHeaders(),
    });
    if (!response.ok) {
      throw new Error("Failed to delete sprint");
    }
  },
};
