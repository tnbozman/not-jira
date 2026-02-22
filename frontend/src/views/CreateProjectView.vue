<template>
  <div class="max-w-2xl mx-auto">
    <Card class="border border-surface-200">
      <template #title>
        <div class="flex items-center gap-3">
          <Button icon="pi pi-arrow-left" text rounded @click="goBack" />
          <h1 class="text-xl font-bold text-surface-900">Create New Project</h1>
        </div>
      </template>
      <template #content>
        <form @submit.prevent="handleSubmit" class="space-y-5">
          <div class="space-y-1.5">
            <label for="key" class="block text-sm font-medium text-surface-700">Project Key *</label>
            <InputText 
              id="key"
              v-model="project.key"
              placeholder="e.g., PROJ, WEB, APP"
              :class="{ 'p-invalid': errors.key }"
              class="w-full"
              @blur="validateKey"
            />
            <small v-if="errors.key" class="text-red-500 text-xs">{{ errors.key }}</small>
            <small class="block text-surface-400 text-xs">Must be uppercase letters, numbers, and hyphens only</small>
          </div>

          <div class="space-y-1.5">
            <label for="name" class="block text-sm font-medium text-surface-700">Project Name *</label>
            <InputText 
              id="name"
              v-model="project.name"
              placeholder="Enter project name"
              :class="{ 'p-invalid': errors.name }"
              class="w-full"
            />
            <small v-if="errors.name" class="text-red-500 text-xs">{{ errors.name }}</small>
          </div>

          <div class="space-y-1.5">
            <label for="description" class="block text-sm font-medium text-surface-700">Description</label>
            <Textarea 
              id="description"
              v-model="project.description"
              placeholder="Enter project description"
              rows="5"
              auto-resize
              class="w-full"
            />
          </div>

          <Message v-if="error" severity="error" :closable="false">{{ error }}</Message>

          <div class="flex justify-end gap-3 pt-4 border-t border-surface-200">
            <Button label="Cancel" severity="secondary" text @click="goBack" />
            <Button 
              label="Create Project" 
              type="submit" 
              icon="pi pi-check"
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

