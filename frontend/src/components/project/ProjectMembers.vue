<template>
  <div class="members-panel">
    <div class="members-header">
      <h2 class="panel-title">Project Members</h2>
      <Button label="Add Member" icon="pi pi-plus" size="small" @click="showAddMemberDialog" />
    </div>

    <DataTable :value="members" :rows="10" stripedRows>
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
        <Button label="Add Member" icon="pi pi-check" @click="addMember" :loading="addingMember" />
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
          Are you sure you want to remove <strong>{{ memberToRemove?.userName }}</strong> from this
          project?
        </p>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="removeMemberDialogVisible = false" />
        <Button label="Remove" severity="danger" @click="removeMember" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import Button from "primevue/button";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Tag from "primevue/tag";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";
import Message from "primevue/message";
import { projectService, type ProjectMember, ProjectRole } from "@/services/projectService";

const props = defineProps<{
  projectId: number;
  members: ProjectMember[];
}>();

const emit = defineEmits<{
  (e: "membersChanged"): void;
}>();

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
  memberError.value = null;

  if (!newMember.value.userId || !newMember.value.userName || !newMember.value.userEmail) {
    memberError.value = "All fields are required";
    return;
  }

  try {
    addingMember.value = true;
    await projectService.addMember(props.projectId, newMember.value);
    addMemberDialogVisible.value = false;
    emit("membersChanged");
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
  if (!memberToRemove.value?.id) return;

  try {
    await projectService.removeMember(props.projectId, memberToRemove.value.id);
    removeMemberDialogVisible.value = false;
    memberToRemove.value = null;
    emit("membersChanged");
  } catch (err) {
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
</script>

<style scoped>
.members-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.members-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.panel-title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
}

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
