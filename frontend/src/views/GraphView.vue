<template>
  <div class="graph-page">
    <div class="graph-card">
      <div class="graph-header">
        <h2 class="graph-title">Knowledge Graph</h2>
        <div class="graph-controls">
          <Button
            label="Refresh"
            icon="pi pi-refresh"
            @click="loadGraphData"
            :loading="loading"
            size="small"
          />
        </div>
      </div>

      <div v-if="stats" class="stats-bar">
        <div class="stat-item">
          <i class="pi pi-users"></i>
          <span>{{ stats.entityCount }} Entities</span>
        </div>
        <div class="stat-item">
          <i class="pi pi-exclamation-circle"></i>
          <span>{{ stats.problemCount }} Problems</span>
        </div>
        <div class="stat-item">
          <i class="pi pi-check-circle"></i>
          <span>{{ stats.outcomeCount }} Outcomes</span>
        </div>
        <div class="stat-item">
          <i class="pi pi-comments"></i>
          <span>{{ stats.interviewCount }} Interviews</span>
        </div>
      </div>

      <div v-if="loading" class="loading-state">
        <ProgressSpinner />
        <p class="loading-text">Loading graph data...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <Message severity="error">{{ error }}</Message>
      </div>

      <div v-else class="graph-container">
        <div ref="cyContainer" class="cy-container"></div>

        <div class="legend">
          <h4 class="legend-title">Legend</h4>
          <div class="legend-item">
            <span class="legend-dot legend-dot--entity"></span>
            <span>Entity (Person/Client)</span>
          </div>
          <div class="legend-item">
            <span class="legend-dot legend-dot--problem"></span>
            <span>Problem</span>
          </div>
          <div class="legend-item">
            <span class="legend-dot legend-dot--outcome"></span>
            <span>Outcome</span>
          </div>
          <div class="legend-item">
            <span class="legend-dot legend-dot--metric"></span>
            <span>Success Metric</span>
          </div>
          <div class="legend-item">
            <span class="legend-dot legend-dot--interview"></span>
            <span>Interview</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from "vue";
import { useRoute } from "vue-router";
import { useToast } from "primevue/usetoast";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Message from "primevue/message";
import cytoscape from "cytoscape";
import type { Core } from "cytoscape";
import graphService, { type GraphData, type GraphStats } from "@/services/graphService";

const route = useRoute();
const toast = useToast();

const projectId = ref<number>(parseInt(route.params.id as string));
const loading = ref(false);
const error = ref<string | null>(null);
const stats = ref<GraphStats | null>(null);
const cyContainer = ref<HTMLElement | null>(null);
let cy: Core | null = null;

const loadGraphData = async () => {
  try {
    loading.value = true;
    error.value = null;

    const data: GraphData = await graphService.getGraphData(projectId.value);
    stats.value = data.stats || null;

    if (cy) {
      renderGraph(data);
    }
  } catch (err: any) {
    error.value = err.message || "Failed to load graph data";
    toast.add({ severity: "error", summary: "Error", detail: error.value, life: 3000 });
  } finally {
    loading.value = false;
  }
};

const renderGraph = (data: GraphData) => {
  if (!cy || !cyContainer.value) return;

  // Clear existing elements
  cy.elements().remove();

  // Color scheme for different node types
  const nodeColors: Record<string, string> = {
    entity: "#3b82f6", // Blue
    problem: "#ef4444", // Red
    outcome: "#10b981", // Green
    metric: "#8b5cf6", // Purple
    interview: "#f59e0b", // Amber
  };

  // Transform data for Cytoscape
  const elements = [
    ...data.nodes.map((node) => ({
      data: {
        id: node.id,
        label: node.label,
        type: node.type,
        ...node.data,
      },
    })),
    ...data.edges.map((edge) => ({
      data: {
        id: edge.id,
        source: edge.source,
        target: edge.target,
        label: edge.label,
      },
    })),
  ];

  // Add elements to graph
  cy.add(elements);

  // Apply styling
  cy.style([
    {
      selector: "node",
      style: {
        label: "data(label)",
        "text-valign": "center",
        "text-halign": "center",
        "background-color": (ele: any) => nodeColors[ele.data("type")] || "#999",
        color: "#fff",
        "text-outline-color": (ele: any) => nodeColors[ele.data("type")] || "#999",
        "text-outline-width": 2,
        "font-size": "12px",
        width: 60,
        height: 60,
      },
    },
    {
      selector: "edge",
      style: {
        width: 2,
        "line-color": "#cbd5e1",
        "target-arrow-color": "#cbd5e1",
        "target-arrow-shape": "triangle",
        "curve-style": "bezier",
        label: "data(label)",
        "font-size": "10px",
        "text-rotation": "autorotate",
        "text-margin-y": -10,
        color: "#64748b",
      },
    },
    {
      selector: ":selected",
      style: {
        "border-width": 3,
        "border-color": "#1e40af",
      },
    },
  ]);

  // Run layout
  cy.layout({
    name: "cose",
    animate: true,
    animationDuration: 500,
    nodeRepulsion: 8000,
    idealEdgeLength: 100,
    edgeElasticity: 100,
    nestingFactor: 5,
    gravity: 80,
    numIter: 1000,
    initialTemp: 200,
    coolingFactor: 0.95,
    minTemp: 1.0,
  }).run();

  // Add click handler to show node details
  cy.on("tap", "node", (evt: any) => {
    const node = evt.target;
    const data = node.data();

    toast.add({
      severity: "info",
      summary: data.label,
      detail: `Type: ${data.type}`,
      life: 3000,
    });
  });
};

const initCytoscape = () => {
  if (!cyContainer.value) return;

  cy = cytoscape({
    container: cyContainer.value,
    style: [],
    layout: { name: "preset" },
    wheelSensitivity: 0.2,
    minZoom: 0.3,
    maxZoom: 3,
  });
};

onMounted(async () => {
  initCytoscape();
  await loadGraphData();
});

onBeforeUnmount(() => {
  if (cy) {
    cy.destroy();
  }
});
</script>

<style scoped>
.graph-page {
  height: calc(100vh - 100px);
}

.graph-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  padding: 1.5rem;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.graph-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.graph-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #0f172a;
}

.graph-controls {
  display: flex;
  gap: 0.5rem;
}

.stats-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  padding: 0.75rem 1rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  margin-bottom: 1rem;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
  font-size: 0.875rem;
  color: #334155;
}

.stat-item i {
  font-size: 1.125rem;
  color: #2563eb;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex: 1;
  gap: 1rem;
}

.loading-text {
  margin: 0;
  color: #64748b;
}

.error-state {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.graph-container {
  flex: 1;
  position: relative;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  overflow: hidden;
}

.cy-container {
  width: 100%;
  height: 100%;
}

.legend {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  background: #ffffff;
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  min-width: 11rem;
}

.legend-title {
  margin: 0 0 0.5rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #1e293b;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.375rem;
  font-size: 0.8125rem;
  color: #475569;
}

.legend-dot {
  width: 1rem;
  height: 1rem;
  border-radius: 50%;
  flex-shrink: 0;
}

.legend-dot--entity {
  background-color: #3b82f6;
}

.legend-dot--problem {
  background-color: #ef4444;
}

.legend-dot--outcome {
  background-color: #10b981;
}

.legend-dot--metric {
  background-color: #8b5cf6;
}

.legend-dot--interview {
  background-color: #f59e0b;
}
</style>
