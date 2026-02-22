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
          <div class="panel-card">
            <div class="panel-cta">
              <div class="cta-icon cta-icon--blue">
                <i class="pi pi-users"></i>
              </div>
              <h3 class="cta-title">External Entities</h3>
              <p class="cta-desc">
                Manage people and clients, track their problems, outcomes, and success metrics.
              </p>
              <Button
                label="Manage Entities"
                icon="pi pi-arrow-right"
                @click="navigateToEntities"
              />
            </div>
          </div>
        </TabPanel>

        <TabPanel header="Knowledge Graph" value="graph">
          <div class="panel-card">
            <div class="panel-cta">
              <div class="cta-icon cta-icon--teal">
                <i class="pi pi-sitemap"></i>
              </div>
              <h3 class="cta-title">Knowledge Graph</h3>
              <p class="cta-desc">
                Visualize relationships between entities, problems, outcomes, and interviews.
              </p>
              <Button label="View Graph" icon="pi pi-arrow-right" @click="navigateToGraph" />
            </div>
          </div>
        </TabPanel>

        <TabPanel header="User Story Map" value="story-map">
          <div class="panel-card">
            <div class="panel-cta">
              <div class="cta-icon cta-icon--amber">
                <i class="pi pi-table"></i>
              </div>
              <h3 class="cta-title">User Story Map</h3>
              <p class="cta-desc">
                Outcome-focused themes, epics, stories, and spikes with sprint planning.
              </p>
              <Button label="View Story Map" icon="pi pi-arrow-right" @click="navigateToStoryMap" />
            </div>
          </div>
        </TabPanel>

        <TabPanel header="Backlog" value="backlog">
          <div class="panel-card">
            <div class="panel-cta">
              <div class="cta-icon cta-icon--purple">
                <i class="pi pi-list"></i>
              </div>
              <h3 class="cta-title">Product Backlog</h3>
              <p class="cta-desc">
                View sprints and backlog items with filtering. Manage sprint planning, reviews, and
                retrospectives.
              </p>
              <Button label="View Backlog" icon="pi pi-arrow-right" @click="navigateToBacklog" />
            </div>
          </div>
        </TabPanel>

        <TabPanel header="Members" value="members">
          <div class="panel-card">
            <div class="members-header">
              <h2 class="members-title">Project Members</h2>
              <Button
                label="Add Member"
                icon="pi pi-plus"
                size="small"
                @click="showAddMemberDialog"
              />
            </div>
            <DataTable :value="project.members" :rows="10" stripedRows>
              <Column field="userName" header="Name"></Column>
              <Column field="userEmail" header="Email"></Column>
              <Column field="role" header="Role">
                <template #body="slotProps">
                  <Tag
                    :value="getRoleLabel(slotProps.data.role)"
                    :severity="getRoleSeverity(slotProps.data.role)"
                  />
                </template>
              </Column>
              <Column header="Actions" style="width: 5rem">
                <template #body="slotProps">
                  <Button
                    icon="pi pi-trash"
                    severity="danger"
                    text
                    size="small"
                    @click="confirmRemoveMember(slotProps.data)"
                  />
                </template>
              </Column>
            </DataTable>
          </div>
        </TabPanel>
      </TabView>

      <!-- Add Member Dialog -->
      <Dialog
        v-model:visible="addMemberDialogVisible"
        header="Add Project Member"
        :modal="true"
        :style="{ width: '28rem' }"
      >
        <form @submit.prevent="addMember" class="dialog-form">
          <div class="form-field">
            <label for="userId" class="field-label">User ID *</label>
            <InputText
              id="userId"
              v-model="newMember.userId"
              placeholder="Enter Keycloak user ID"
              class="w-full"
            />
          </div>
          <div class="form-field">
            <label for="userName" class="field-label">User Name *</label>
            <InputText
              id="userName"
              v-model="newMember.userName"
              placeholder="Enter user name"
              class="w-full"
            />
          </div>
          <div class="form-field">
            <label for="userEmail" class="field-label">User Email *</label>
            <InputText
              id="userEmail"
              v-model="newMember.userEmail"
              type="email"
              placeholder="Enter user email"
              class="w-full"
            />
          </div>
          <div class="form-field">
            <label for="role" class="field-label">Role *</label>
            <Dropdown
              id="role"
              v-model="newMember.role"
              :options="roleOptions"
              option-label="label"
              option-value="value"
              placeholder="Select a role"
              class="w-full"
            />
          </div>
          <Message v-if="memberError" severity="error" :closable="false">{{ memberError }}</Message>
        </form>
        <template #footer>
          <Button label="Cancel" text @click="addMemberDialogVisible = false" />
          <Button
            label="Add Member"
            icon="pi pi-check"
            @click="addMember"
            :loading="addingMember"
          />
        </template>
      </Dialog>

      <!-- Remove Member Confirmation Dialog -->
      <Dialog
        v-model:visible="removeMemberDialogVisible"
        header="Remove Member"
        :modal="true"
        :style="{ width: '26rem' }"
      >
        <div class="confirm-body">
          <div class="confirm-icon-wrap">
            <i class="pi pi-exclamation-triangle confirm-icon"></i>
          </div>
          <p class="confirm-text">
            Are you sure you want to remove <strong>{{ memberToRemove?.userName }}</strong> from
            this project?
          </p>
        </div>
        <template #footer>
          <Button label="Cancel" text @click="removeMemberDialogVisible = false" />
          <Button label="Remove" severity="danger" @click="removeMember" />
        </template>
      </Dialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Message from "primevue/message";
import Tag from "primevue/tag";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import TabView from "primevue/tabview";
import TabPanel from "primevue/tabpanel";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Dialog from "primevue/dialog";
import Dropdown from "primevue/dropdown";
import {
  projectService,
  type Project,
  type ProjectMember,
  ProjectRole,
} from "@/services/projectService";

const router = useRouter();
const route = useRoute();

const project = ref<Project | null>(null);
const editedProject = ref<Project | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const editing = ref(false);
const saving = ref(false);

const addMemberDialogVisible = ref(false);
const removeMemberDialogVisible = ref(false);
const memberToRemove = ref<ProjectMember | null>(null);
const addingMember = ref(false);
const memberError = ref<string | null>(null);

const newMember = ref<ProjectMember>({
  userId: "",
  userName: "",
  userEmail: "",
  role: ProjectRole.Developer,
});

const roleOptions = [
  { label: "Developer", value: ProjectRole.Developer },
  { label: "Product Manager", value: ProjectRole.ProductManager },
  { label: "Project Sponsor", value: ProjectRole.ProjectSponsor },
];

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

const showAddMemberDialog = () => {
  newMember.value = {
    userId: "",
    userName: "",
    userEmail: "",
    role: ProjectRole.Developer,
  };
  memberError.value = null;
  addMemberDialogVisible.value = true;
};

const addMember = async () => {
  if (!project.value?.id) return;

  memberError.value = null;

  if (!newMember.value.userId || !newMember.value.userName || !newMember.value.userEmail) {
    memberError.value = "All fields are required";
    return;
  }

  try {
    addingMember.value = true;
    await projectService.addMember(project.value.id, newMember.value);
    addMemberDialogVisible.value = false;
    await loadProject();
  } catch (err) {
    if (err instanceof Error) {
      memberError.value = err.message;
    } else {
      memberError.value = "Failed to add member. Please try again.";
    }
    console.error("Error adding member:", err);
  } finally {
    addingMember.value = false;
  }
};

const confirmRemoveMember = (member: ProjectMember) => {
  memberToRemove.value = member;
  removeMemberDialogVisible.value = true;
};

const removeMember = async () => {
  if (!project.value?.id || !memberToRemove.value?.id) return;

  try {
    await projectService.removeMember(project.value.id, memberToRemove.value.id);
    removeMemberDialogVisible.value = false;
    memberToRemove.value = null;
    await loadProject();
  } catch (err) {
    error.value = "Failed to remove member. Please try again.";
    console.error("Error removing member:", err);
  }
};

const getRoleLabel = (role: ProjectRole): string => {
  switch (role) {
    case ProjectRole.Developer:
      return "Developer";
    case ProjectRole.ProductManager:
      return "Product Manager";
    case ProjectRole.ProjectSponsor:
      return "Project Sponsor";
    default:
      return "Unknown";
  }
};

const getRoleSeverity = (role: ProjectRole): string => {
  switch (role) {
    case ProjectRole.Developer:
      return "info";
    case ProjectRole.ProductManager:
      return "success";
    case ProjectRole.ProjectSponsor:
      return "warning";
    default:
      return "secondary";
  }
};

const goBack = () => {
  router.push("/projects");
};

const navigateToEntities = () => {
  router.push(`/projects/${project.value?.id}/entities`);
};

const navigateToGraph = () => {
  router.push(`/projects/${project.value?.id}/graph`);
};

const navigateToStoryMap = () => {
  router.push(`/projects/${project.value?.id}/story-map`);
};

const navigateToBacklog = () => {
  router.push(`/projects/${project.value?.id}/backlog`);
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
.edit-form,
.dialog-form {
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

/* CTA panels (entities, graph, story map, backlog) */
.panel-cta {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 2rem 0;
  gap: 1rem;
}

.cta-icon {
  width: 4rem;
  height: 4rem;
  border-radius: 9999px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
}

.cta-icon--blue {
  background: #dbeafe;
  color: #2563eb;
}

.cta-icon--teal {
  background: #ccfbf1;
  color: #0d9488;
}

.cta-icon--amber {
  background: #fef3c7;
  color: #d97706;
}

.cta-icon--purple {
  background: #ede9fe;
  color: #7c3aed;
}

.cta-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
}

.cta-desc {
  margin: 0;
  color: #64748b;
  max-width: 28rem;
}

/* Members tab */
.members-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.members-title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
}

/* Confirm dialog */
.confirm-body {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
}

.confirm-icon-wrap {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 9999px;
  background: #fee2e2;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.confirm-icon {
  color: #dc2626;
}

.confirm-text {
  margin: 0;
  color: #334155;
}
</style>
