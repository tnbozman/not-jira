<template>
  <div class="create-project-view">
    <Card>
      <template #title>
        <div class="header">
          <Button icon="pi pi-arrow-left" text @click="goBack" />
          <h1>Create New Project</h1>
        </div>
      </template>
      <template #content>
        <form @submit.prevent="handleSubmit">
          <div class="form-field">
            <label for="key">Project Key *</label>
            <InputText 
              id="key"
              v-model="project.key"
              placeholder="e.g., PROJ, WEB, APP"
              :class="{ 'p-invalid': errors.key }"
              @blur="validateKey"
            />
            <small v-if="errors.key" class="p-error">{{ errors.key }}</small>
            <small class="help-text">Must be uppercase letters, numbers, and hyphens only</small>
          </div>

          <div class="form-field">
            <label for="name">Project Name *</label>
            <InputText 
              id="name"
              v-model="project.name"
              placeholder="Enter project name"
              :class="{ 'p-invalid': errors.name }"
            />
            <small v-if="errors.name" class="p-error">{{ errors.name }}</small>
          </div>

          <div class="form-field">
            <label for="description">Description</label>
            <Textarea 
              id="description"
              v-model="project.description"
              placeholder="Enter project description"
              rows="5"
              auto-resize
            />
          </div>

          <Message v-if="error" severity="error" :closable="false">{{ error }}</Message>

          <div class="form-actions">
            <Button label="Cancel" severity="secondary" text @click="goBack" />
            <Button 
              label="Create Project" 
              type="submit" 
              :loading="submitting"
              :disabled="!isValid"
            />
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Message from 'primevue/message'
import { projectService, type Project } from '@/services/projectService'

const router = useRouter()

const project = ref<Project>({
  key: '',
  name: '',
  description: '',
})

const errors = ref<Record<string, string>>({})
const error = ref<string | null>(null)
const submitting = ref(false)

const validateKey = () => {
  errors.value.key = ''
  
  if (!project.value.key) {
    errors.value.key = 'Project key is required'
    return false
  }

  if (!/^[A-Z0-9-]+$/.test(project.value.key)) {
    errors.value.key = 'Project key must contain only uppercase letters, numbers, and hyphens'
    return false
  }

  if (project.value.key.length > 20) {
    errors.value.key = 'Project key must be 20 characters or less'
    return false
  }

  return true
}

const isValid = computed(() => {
  return project.value.key.trim() !== '' && 
         project.value.name.trim() !== '' &&
         /^[A-Z0-9-]+$/.test(project.value.key)
})

const handleSubmit = async () => {
  errors.value = {}
  error.value = null

  // Validate
  if (!validateKey()) {
    return
  }

  if (!project.value.name.trim()) {
    errors.value.name = 'Project name is required'
    return
  }

  try {
    submitting.value = true
    const createdProject = await projectService.createProject(project.value)
    router.push(`/projects/${createdProject.id}`)
  } catch (err) {
    if (err instanceof Error) {
      error.value = err.message
    } else {
      error.value = 'Failed to create project. Please try again.'
    }
    console.error('Error creating project:', err)
  } finally {
    submitting.value = false
  }
}

const goBack = () => {
  router.push('/projects')
}
</script>

<style scoped>
.create-project-view {
  padding: 2rem;
  max-width: 800px;
  margin: 0 auto;
}

.header {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header h1 {
  margin: 0;
  font-size: 1.75rem;
  font-weight: 600;
}

.form-field {
  margin-bottom: 1.5rem;
}

.form-field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-field input,
.form-field textarea {
  width: 100%;
}

.help-text {
  display: block;
  margin-top: 0.25rem;
  color: var(--text-color-secondary);
  font-size: 0.875rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
}

.p-error {
  color: var(--red-500);
  font-size: 0.875rem;
  display: block;
  margin-top: 0.25rem;
}

.p-invalid {
  border-color: var(--red-500);
}
</style>
