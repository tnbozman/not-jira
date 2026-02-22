<template>
  <div class="projects-view">
    <div class="header">
      <h1>Projects</h1>
      <Button label="New Project" icon="pi pi-plus" @click="navigateToCreate" />
    </div>

    <div v-if="loading" class="loading">
      <ProgressSpinner />
    </div>

    <div v-else-if="error" class="error">
      <Message severity="error" :closable="false">{{ error }}</Message>
    </div>

    <div v-else-if="projects.length === 0" class="empty-state">
      <p>No projects found. Create your first project to get started!</p>
      <Button label="Create Project" icon="pi pi-plus" @click="navigateToCreate" />
    </div>

    <div v-else class="projects-grid">
      <Card v-for="project in projects" :key="project.id" class="project-card">
        <template #header>
          <div class="card-header">
            <Tag :value="project.key" severity="info" class="project-key-tag" />
          </div>
        </template>
        <template #title>
          {{ project.name }}
        </template>
        <template #content>
          <p class="project-description">{{ project.description || 'No description' }}</p>
          <div class="project-stats">
            <div class="stat">
              <i class="pi pi-users"></i>
              <span>{{ project.members?.length || 0 }} members</span>
            </div>
          </div>
        </template>
        <template #footer>
          <div class="card-actions">
            <Button label="View" icon="pi pi-eye" text @click="navigateToProject(project.id!)" />
            <Button 
              label="Delete" 
              icon="pi pi-trash" 
              severity="danger" 
              text 
              @click="confirmDelete(project)" 
            />
          </div>
        </template>
      </Card>
    </div>

    <Dialog v-model:visible="deleteDialogVisible" header="Confirm Delete" :modal="true">
      <p>Are you sure you want to delete project "{{ projectToDelete?.name }}"?</p>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="deleteDialogVisible = false" />
        <Button label="Delete" icon="pi pi-check" severity="danger" @click="deleteProject" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Card from 'primevue/card'
import ProgressSpinner from 'primevue/progressspinner'
import Message from 'primevue/message'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import { projectService, type Project } from '@/services/projectService'

const router = useRouter()
const projects = ref<Project[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const deleteDialogVisible = ref(false)
const projectToDelete = ref<Project | null>(null)

const loadProjects = async () => {
  try {
    loading.value = true
    error.value = null
    projects.value = await projectService.getAllProjects()
  } catch (err) {
    error.value = 'Failed to load projects. Please try again.'
    console.error('Error loading projects:', err)
  } finally {
    loading.value = false
  }
}

const navigateToCreate = () => {
  router.push('/projects/new')
}

const navigateToProject = (id: number) => {
  router.push(`/projects/${id}`)
}

const confirmDelete = (project: Project) => {
  projectToDelete.value = project
  deleteDialogVisible.value = true
}

const deleteProject = async () => {
  if (!projectToDelete.value?.id) return

  try {
    await projectService.deleteProject(projectToDelete.value.id)
    deleteDialogVisible.value = false
    projectToDelete.value = null
    await loadProjects()
  } catch (err) {
    error.value = 'Failed to delete project. Please try again.'
    console.error('Error deleting project:', err)
  }
}

onMounted(() => {
  loadProjects()
})
</script>

<style scoped>
.projects-view {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header h1 {
  margin: 0;
  font-size: 2rem;
  font-weight: 600;
}

.loading {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.error {
  margin-bottom: 2rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
}

.empty-state p {
  font-size: 1.125rem;
  color: var(--text-color-secondary);
  margin-bottom: 1.5rem;
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.project-card {
  height: 100%;
}

.card-header {
  padding: 1rem;
  background: var(--surface-50);
}

.project-key-tag {
  font-family: monospace;
  font-weight: 600;
}

.project-description {
  color: var(--text-color-secondary);
  margin-bottom: 1rem;
  min-height: 3rem;
}

.project-stats {
  display: flex;
  gap: 1.5rem;
  margin-top: 1rem;
}

.stat {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--text-color-secondary);
}

.stat i {
  font-size: 1rem;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}
</style>
