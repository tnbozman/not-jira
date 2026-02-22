<template>
  <div class="projects-page">
    <!-- Page Header -->
    <div class="page-header">
      <div>
        <h1 class="page-title">Projects</h1>
        <p class="page-sub">Manage and organize your projects</p>
      </div>
      <Button label="New Project" icon="pi pi-plus" @click="navigateToCreate" />
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <ProgressSpinner />
    </div>

    <!-- Error -->
    <Message v-else-if="error" severity="error" :closable="false">{{ error }}</Message>

    <!-- Empty State -->
    <div v-else-if="projects.length === 0" class="empty-state">
      <div class="empty-icon-wrap">
        <i class="pi pi-folder-open empty-icon"></i>
      </div>
      <h3 class="empty-title">No projects found</h3>
      <p class="empty-desc">Create your first project to get started!</p>
      <Button label="Create Project" icon="pi pi-plus" @click="navigateToCreate" />
    </div>

    <!-- Projects Grid -->
    <div v-else class="project-grid">
      <div
        v-for="project in projects"
        :key="project.id"
        class="project-card"
        @click="navigateToProject(project.id!)"
      >
        <div class="project-card-top">
          <span class="project-key">{{ project.key }}</span>
        </div>
        <h3 class="project-name">{{ project.name }}</h3>
        <p class="project-desc">
          {{ project.description || "No description" }}
        </p>
        <div class="project-footer">
          <span class="project-members">
            <i class="pi pi-users"></i>
            {{ project.members?.length || 0 }} members
          </span>
          <div class="project-actions">
            <Button
              label="Open"
              icon="pi pi-arrow-right"
              text
              size="small"
              @click.stop="navigateToProject(project.id!)"
            />
            <Button
              icon="pi pi-trash"
              severity="danger"
              text
              size="small"
              @click.stop="confirmDelete(project)"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation -->
    <Dialog
      v-model:visible="deleteDialogVisible"
      header="Confirm Delete"
      :modal="true"
      :style="{ width: '28rem' }"
    >
      <div class="confirm-body">
        <div class="confirm-icon-wrap">
          <i class="pi pi-exclamation-triangle confirm-icon"></i>
        </div>
        <p class="confirm-text">
          Are you sure you want to delete project <strong>"{{ projectToDelete?.name }}"</strong>?
          This action cannot be undone.
        </p>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="deleteDialogVisible = false" />
        <Button label="Delete" icon="pi pi-trash" severity="danger" @click="deleteProject" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Message from "primevue/message";
import Dialog from "primevue/dialog";
import { projectService, type Project } from "@/services/projectService";

const router = useRouter();
const projects = ref<Project[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const deleteDialogVisible = ref(false);
const projectToDelete = ref<Project | null>(null);

const loadProjects = async () => {
  try {
    loading.value = true;
    error.value = null;
    projects.value = await projectService.getAllProjects();
  } catch (err) {
    error.value = "Failed to load projects. Please try again.";
    console.error("Error loading projects:", err);
  } finally {
    loading.value = false;
  }
};

const navigateToCreate = () => {
  router.push("/projects/new");
};

const navigateToProject = (id: number) => {
  router.push(`/projects/${id}`);
};

const confirmDelete = (project: Project) => {
  projectToDelete.value = project;
  deleteDialogVisible.value = true;
};

const deleteProject = async () => {
  if (!projectToDelete.value?.id) return;

  try {
    await projectService.deleteProject(projectToDelete.value.id);
    deleteDialogVisible.value = false;
    projectToDelete.value = null;
    await loadProjects();
  } catch (err) {
    error.value = "Failed to delete project. Please try again.";
    console.error("Error deleting project:", err);
  }
};

onMounted(() => {
  loadProjects();
});
</script>

<style scoped>
.projects-page {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.page-header {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
}

.page-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: #0f172a;
}

.page-sub {
  font-size: 0.8125rem;
  color: #64748b;
  margin-top: 0.125rem;
}

.loading-state {
  display: flex;
  justify-content: center;
  padding: 4rem 0;
}

/* ── Empty state ─────────────────────────────────────── */
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
}

.empty-icon-wrap {
  width: 4rem;
  height: 4rem;
  border-radius: 50%;
  background-color: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
}

.empty-icon {
  font-size: 1.5rem;
  color: #94a3b8;
}

.empty-title {
  font-size: 1.0625rem;
  font-weight: 600;
  color: #334155;
  margin-bottom: 0.25rem;
}

.empty-desc {
  font-size: 0.875rem;
  color: #64748b;
  margin-bottom: 1.25rem;
}

/* ── Project grid ────────────────────────────────────── */
.project-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.75rem;
}

@media (min-width: 768px) {
  .project-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1280px) {
  .project-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

.project-card {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  padding: 1.25rem;
  cursor: pointer;
  transition:
    box-shadow 0.15s ease,
    border-color 0.15s ease;
  display: flex;
  flex-direction: column;
}

.project-card:hover {
  box-shadow: 0 4px 12px rgb(0 0 0 / 0.06);
  border-color: #cbd5e1;
}

.project-card-top {
  margin-bottom: 0.5rem;
}

.project-key {
  display: inline-flex;
  align-items: center;
  padding: 0.125rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.6875rem;
  font-weight: 700;
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
  background-color: #dbeafe;
  color: #1e40af;
  letter-spacing: 0.02em;
}

.project-name {
  font-size: 1rem;
  font-weight: 600;
  color: #0f172a;
  margin-bottom: 0.25rem;
  line-height: 1.4;
}

.project-desc {
  font-size: 0.8125rem;
  color: #64748b;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  flex: 1;
}

.project-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.project-members {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #94a3b8;
}

.project-members i {
  font-size: 0.6875rem;
}

.project-actions {
  display: flex;
  gap: 0.25rem;
}

/* ── Confirm dialog ──────────────────────────────────── */
.confirm-body {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
}

.confirm-icon-wrap {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  background-color: #fef2f2;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.confirm-icon {
  color: #dc2626;
}

.confirm-text {
  font-size: 0.875rem;
  color: #334155;
  line-height: 1.5;
}
</style>
