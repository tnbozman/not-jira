<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-2xl font-bold text-surface-900">Projects</h1>
        <p class="text-surface-500 text-sm mt-1">Manage and organize your projects</p>
      </div>
      <Button label="New Project" icon="pi pi-plus" @click="navigateToCreate" />
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center items-center py-24">
      <ProgressSpinner />
    </div>

    <!-- Error -->
    <Message v-else-if="error" severity="error" :closable="false">{{ error }}</Message>

    <!-- Empty State -->
    <div v-else-if="projects.length === 0" class="text-center py-16">
      <div
        class="w-20 h-20 rounded-full bg-surface-100 flex items-center justify-center mx-auto mb-4"
      >
        <i class="pi pi-folder-open text-surface-400 text-3xl"></i>
      </div>
      <h3 class="text-lg font-semibold text-surface-700 mb-2">No projects found</h3>
      <p class="text-surface-500 mb-6">Create your first project to get started!</p>
      <Button label="Create Project" icon="pi pi-plus" @click="navigateToCreate" />
    </div>

    <!-- Projects Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5">
      <Card
        v-for="project in projects"
        :key="project.id"
        class="border border-surface-200 hover:shadow-lg transition-shadow cursor-pointer group"
        @click="navigateToProject(project.id!)"
      >
        <template #header>
          <div class="px-5 pt-5 pb-0">
            <span
              class="inline-flex items-center px-2.5 py-0.5 rounded text-xs font-mono font-bold bg-blue-100 text-blue-700"
            >
              {{ project.key }}
            </span>
          </div>
        </template>
        <template #title>
          <span class="text-lg group-hover:text-primary transition-colors">{{ project.name }}</span>
        </template>
        <template #content>
          <p class="text-surface-500 text-sm line-clamp-2 mb-4">
            {{ project.description || "No description" }}
          </p>
          <div class="flex items-center gap-4 text-sm text-surface-400">
            <span class="flex items-center gap-1.5">
              <i class="pi pi-users text-xs"></i>
              {{ project.members?.length || 0 }} members
            </span>
          </div>
        </template>
        <template #footer>
          <div class="flex justify-end gap-2">
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
        </template>
      </Card>
    </div>

    <!-- Delete Confirmation -->
    <Dialog
      v-model:visible="deleteDialogVisible"
      header="Confirm Delete"
      :modal="true"
      :style="{ width: '28rem' }"
    >
      <div class="flex items-start gap-4">
        <div class="w-10 h-10 rounded-full bg-red-100 flex items-center justify-center shrink-0">
          <i class="pi pi-exclamation-triangle text-red-600"></i>
        </div>
        <p class="text-surface-700">
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
import Card from "primevue/card";
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
