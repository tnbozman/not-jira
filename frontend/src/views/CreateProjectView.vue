<template>
  <div class="create-page">
    <div class="create-card">
      <div class="create-card-header">
        <Button icon="pi pi-arrow-left" text rounded @click="goBack" />
        <h1 class="create-title">Create New Project</h1>
      </div>

      <form @submit.prevent="handleSubmit" class="create-form">
        <div class="form-field">
          <label for="key" class="field-label">Project Key *</label>
          <InputText
            id="key"
            v-model="project.key"
            placeholder="e.g., PROJ, WEB, APP"
            :class="{ 'p-invalid': errors.key }"
            class="w-full"
            @blur="validateKey"
          />
          <small v-if="errors.key" class="field-error">{{ errors.key }}</small>
          <small class="field-hint">Must be uppercase letters, numbers, and hyphens only</small>
        </div>

        <div class="form-field">
          <label for="name" class="field-label">Project Name *</label>
          <InputText
            id="name"
            v-model="project.name"
            placeholder="Enter project name"
            :class="{ 'p-invalid': errors.name }"
            class="w-full"
          />
          <small v-if="errors.name" class="field-error">{{ errors.name }}</small>
        </div>

        <div class="form-field">
          <label for="description" class="field-label">Description</label>
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

        <div class="create-actions">
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import Message from "primevue/message";
import { projectService, type Project } from "@/services/projectService";

const router = useRouter();

const project = ref<Project>({
  key: "",
  name: "",
  description: "",
});

const errors = ref<Record<string, string>>({});
const error = ref<string | null>(null);
const submitting = ref(false);

const validateKey = () => {
  errors.value.key = "";

  if (!project.value.key) {
    errors.value.key = "Project key is required";
    return false;
  }

  if (!/^[A-Z0-9-]+$/.test(project.value.key)) {
    errors.value.key = "Project key must contain only uppercase letters, numbers, and hyphens";
    return false;
  }

  if (project.value.key.length > 20) {
    errors.value.key = "Project key must be 20 characters or less";
    return false;
  }

  return true;
};

const isValid = computed(() => {
  return (
    project.value.key.trim() !== "" &&
    project.value.name.trim() !== "" &&
    /^[A-Z0-9-]+$/.test(project.value.key)
  );
});

const handleSubmit = async () => {
  errors.value = {};
  error.value = null;

  // Validate
  if (!validateKey()) {
    return;
  }

  if (!project.value.name.trim()) {
    errors.value.name = "Project name is required";
    return;
  }

  try {
    submitting.value = true;
    const createdProject = await projectService.createProject(project.value);
    router.push(`/projects/${createdProject.id}`);
  } catch (err) {
    if (err instanceof Error) {
      error.value = err.message;
    } else {
      error.value = "Failed to create project. Please try again.";
    }
    console.error("Error creating project:", err);
  } finally {
    submitting.value = false;
  }
};

const goBack = () => {
  router.push("/projects");
};
</script>

<style scoped>
.create-page {
  max-width: 40rem;
  margin: 0 auto;
}

.create-card {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  padding: 1.5rem;
}

.create-card-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.create-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
}

.create-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.field-label {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #334155;
}

.field-error {
  font-size: 0.75rem;
  color: #dc2626;
}

.field-hint {
  font-size: 0.75rem;
  color: #94a3b8;
}

.create-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding-top: 1rem;
  border-top: 1px solid #e2e8f0;
}
</style>
