<template>
  <div>
    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <ProgressSpinner />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-block">
      <Message severity="error" :closable="false">{{ error }}</Message>
      <Button label="Go Back" icon="pi pi-arrow-left" text @click="goBack" />
    </div>

    <div v-else-if="project" class="detail-page">
      <!-- Header -->
      <div class="detail-header">
        <div class="detail-header-left">
          <Button icon="pi pi-arrow-left" text rounded @click="goBack" />
          <div>
            <span class="detail-key">{{ project.key }}</span>
            <h1 class="detail-title">{{ project.name }}</h1>
          </div>
        </div>
        <div class="detail-header-actions">
          <Button
            v-if="!editing"
            label="Edit"
            icon="pi pi-pencil"
            outlined
            size="small"
            @click="startEdit"
          />
          <template v-if="editing">
            <Button label="Cancel" severity="secondary" text size="small" @click="cancelEdit" />
            <Button
              label="Save"
              icon="pi pi-check"
              size="small"
              @click="saveProject"
              :loading="saving"
            />
          </template>
        </div>
      </div>

      <TabView>
        <TabPanel header="Overview" value="overview">
          <div class="panel-card">
            <div v-if="!editing" class="overview-fields">
              <div class="ov-field">
                <label class="ov-label">Project Key</label>
                <p class="ov-value">{{ project.key }}</p>
              </div>
              <div class="ov-field">
                <label class="ov-label">Project Name</label>
                <p class="ov-value">{{ project.name }}</p>
              </div>
              <div class="ov-field">
                <label class="ov-label">Description</label>
                <p class="ov-value">{{ project.description || "No description" }}</p>
              </div>
            </div>

            <form v-else @submit.prevent="saveProject" class="edit-form">
              <div class="form-field">
                <label for="key" class="field-label">Project Key</label>
                <InputText id="key" v-model="editedProject!.key" disabled class="w-full" />
                <small class="field-hint">Project key cannot be changed</small>
              </div>
              <div class="form-field">
                <label for="name" class="field-label">Project Name *</label>
                <InputText
                  id="name"
                  v-model="editedProject!.name"
                  placeholder="Enter project name"
                  class="w-full"
                />
              </div>
              <div class="form-field">
                <label for="description" class="field-label">Description</label>
                <Textarea
                  id="description"
                  v-model="editedProject!.description"
                  placeholder="Enter project description"
                  rows="5"
                  auto-resize
                  class="w-full"
                />
              </div>
            </form>
          </div>
        </TabPanel>

        <TabPanel header="External Entities" value="entities">
          <ProjectEntities :projectId="project.id!" />
        </TabPanel>

        <TabPanel header="Knowledge Graph" value="graph">
          <ProjectGraph :projectId="project.id!" />
        </TabPanel>

        <TabPanel header="User Story Map" value="story-map">
          <ProjectStoryMap :projectId="project.id!" />
        </TabPanel>

        <TabPanel header="Backlog" value="backlog">
          <ProjectBacklog :projectId="project.id!" />
        </TabPanel>

        <TabPanel header="Members" value="members">
          <ProjectMembers
            :projectId="project.id!"
            :members="project.members || []"
            @membersChanged="loadProject"
          />
        </TabPanel>
      </TabView>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Message from "primevue/message";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import TabView from "primevue/tabview";
import TabPanel from "primevue/tabpanel";
import { projectService, type Project } from "@/services/projectService";
import ProjectEntities from "@/components/project/ProjectEntities.vue";
import ProjectGraph from "@/components/project/ProjectGraph.vue";
import ProjectStoryMap from "@/components/project/ProjectStoryMap.vue";
import ProjectBacklog from "@/components/project/ProjectBacklog.vue";
import ProjectMembers from "@/components/project/ProjectMembers.vue";

const router = useRouter();
const route = useRoute();

const project = ref<Project | null>(null);
const editedProject = ref<Project | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const editing = ref(false);
const saving = ref(false);

const loadProject = async () => {
  try {
    loading.value = true;
    error.value = null;
    const id = parseInt(route.params.id as string);
    project.value = await projectService.getProject(id);
  } catch (err) {
    error.value = "Failed to load project. Please try again.";
    console.error("Error loading project:", err);
  } finally {
    loading.value = false;
  }
};

const startEdit = () => {
  editedProject.value = { ...project.value! };
  editing.value = true;
};

const cancelEdit = () => {
  editedProject.value = null;
  editing.value = false;
};

const saveProject = async () => {
  if (!editedProject.value || !project.value?.id) return;

  try {
    saving.value = true;
    await projectService.updateProject(project.value.id, editedProject.value);
    project.value = { ...editedProject.value };
    editing.value = false;
  } catch (err) {
    error.value = "Failed to save project. Please try again.";
    console.error("Error saving project:", err);
  } finally {
    saving.value = false;
  }
};

const goBack = () => {
  router.push("/projects");
};

onMounted(() => {
  loadProject();
});
</script>

<style scoped>
.loading-state {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 6rem 0;
}

.error-block {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: flex-start;
}

.detail-page {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.detail-header {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
}

.detail-header-left {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.detail-key {
  display: inline-flex;
  align-items: center;
  padding: 0.125rem 0.625rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-family: ui-monospace, monospace;
  font-weight: 700;
  background: #dbeafe;
  color: #1d4ed8;
}

.detail-title {
  margin: 0.25rem 0 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #0f172a;
}

.detail-header-actions {
  display: flex;
  gap: 0.5rem;
}

/* Panel cards inside tabs */
.panel-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  padding: 1.5rem;
}

/* Overview fields */
.overview-fields {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.ov-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #94a3b8;
  margin-bottom: 0.25rem;
}

.ov-value {
  margin: 0;
  color: #1e293b;
}

/* Edit form */
.edit-form {
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
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #334155;
}

.field-hint {
  font-size: 0.75rem;
  color: #94a3b8;
}
</style>
