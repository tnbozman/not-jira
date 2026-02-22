<template>
  <div class="entities-panel">
    <div class="panel-header">
      <div>
        <h2 class="panel-title">External Entities</h2>
        <p class="panel-subtitle">Manage people and clients for this project</p>
      </div>
      <Button label="Add Entity" icon="pi pi-plus" size="small" @click="showCreateDialog = true" />
    </div>

    <div v-if="loading" class="loading-state">
      <ProgressSpinner />
    </div>

    <DataTable v-else :value="entities" stripedRows responsiveLayout="scroll" class="text-sm">
      <Column field="name" header="Name" sortable></Column>
      <Column field="type" header="Type" sortable style="width: 8rem">
        <template #body="{ data }">
          <Tag :value="data.type" :severity="data.type === 'Person' ? 'info' : 'success'" />
        </template>
      </Column>
      <Column field="email" header="Email"></Column>
      <Column field="organization" header="Organization"></Column>
      <Column header="Actions" style="width: 10rem">
        <template #body="{ data }">
          <div class="action-btns">
            <Button
              icon="pi pi-eye"
              text
              rounded
              size="small"
              @click="viewEntity(data)"
              v-tooltip="'View Details'"
            />
            <Button
              icon="pi pi-pencil"
              text
              rounded
              size="small"
              @click="editEntity(data)"
              v-tooltip="'Edit'"
            />
            <Button
              icon="pi pi-trash"
              text
              rounded
              size="small"
              severity="danger"
              @click="confirmDelete(data)"
              v-tooltip="'Delete'"
            />
          </div>
        </template>
      </Column>
    </DataTable>

    <!-- Create/Edit Dialog -->
    <Dialog
      v-model:visible="showCreateDialog"
      :header="editingEntity?.id ? 'Edit Entity' : 'Create Entity'"
      :style="{ width: '32rem' }"
      :modal="true"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label class="field-label">Type *</label>
          <Dropdown
            v-model="formData.type"
            :options="['Person', 'Client']"
            placeholder="Select Type"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label class="field-label">Name *</label>
          <InputText v-model="formData.name" class="w-full" placeholder="Enter name" />
        </div>
        <div class="form-field">
          <label class="field-label">Email</label>
          <InputText
            v-model="formData.email"
            class="w-full"
            type="email"
            placeholder="email@example.com"
          />
        </div>
        <div class="form-field">
          <label class="field-label">Organization</label>
          <InputText
            v-model="formData.organization"
            class="w-full"
            placeholder="Organization name"
          />
        </div>
        <div class="form-field">
          <label class="field-label">Phone</label>
          <InputText v-model="formData.phone" class="w-full" placeholder="Phone number" />
        </div>
        <div class="form-field">
          <label class="field-label">Notes</label>
          <Textarea
            v-model="formData.notes"
            class="w-full"
            rows="4"
            placeholder="Additional notes"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showCreateDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="saveEntity" :loading="saving" />
      </template>
    </Dialog>

    <!-- View Dialog -->
    <Dialog
      v-model:visible="showViewDialog"
      header="Entity Details"
      :style="{ width: '48rem' }"
      :modal="true"
    >
      <div v-if="selectedEntity" class="view-entity">
        <div class="view-section">
          <h3 class="section-title">Basic Information</h3>
          <div class="detail-grid">
            <div class="detail-item">
              <span class="detail-label">Name</span>
              <p class="detail-value">{{ selectedEntity.name }}</p>
            </div>
            <div class="detail-item">
              <span class="detail-label">Type</span>
              <div style="margin-top: 0.125rem">
                <Tag
                  :value="selectedEntity.type"
                  :severity="selectedEntity.type === 'Person' ? 'info' : 'success'"
                />
              </div>
            </div>
            <div v-if="selectedEntity.email" class="detail-item">
              <span class="detail-label">Email</span>
              <p class="detail-value">{{ selectedEntity.email }}</p>
            </div>
            <div v-if="selectedEntity.organization" class="detail-item">
              <span class="detail-label">Organization</span>
              <p class="detail-value">{{ selectedEntity.organization }}</p>
            </div>
            <div v-if="selectedEntity.phone" class="detail-item">
              <span class="detail-label">Phone</span>
              <p class="detail-value">{{ selectedEntity.phone }}</p>
            </div>
          </div>
        </div>

        <div v-if="selectedEntity.notes" class="view-section">
          <h3 class="section-title">Notes</h3>
          <p class="notes-text">{{ selectedEntity.notes }}</p>
        </div>

        <div
          v-if="selectedEntity.problems && selectedEntity.problems.length > 0"
          class="view-section"
        >
          <h3 class="section-title">Problems ({{ selectedEntity.problems.length }})</h3>
          <div class="item-list">
            <div v-for="problem in selectedEntity.problems" :key="problem.id" class="item-block">
              <Tag
                :value="problem.severity"
                :severity="getSeverityColor(problem.severity)"
                style="margin-bottom: 0.5rem"
              />
              <p class="item-text">{{ problem.description }}</p>
            </div>
          </div>
        </div>

        <div
          v-if="selectedEntity.interviews && selectedEntity.interviews.length > 0"
          class="view-section"
        >
          <h3 class="section-title">Interviews ({{ selectedEntity.interviews.length }})</h3>
          <div class="item-list">
            <div
              v-for="interview in selectedEntity.interviews"
              :key="interview.id"
              class="item-block"
            >
              <div class="interview-row">
                <Tag :value="interview.type" />
                <span class="interview-date">{{ formatDate(interview.interviewDate) }}</span>
              </div>
              <p v-if="interview.summary" class="item-text">
                {{ interview.summary }}
              </p>
            </div>
          </div>
        </div>
      </div>
      <template #footer>
        <Button label="Close" @click="showViewDialog = false" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useToast } from "primevue/usetoast";
import { useConfirm } from "primevue/useconfirm";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import Dropdown from "primevue/dropdown";
import Tag from "primevue/tag";
import ProgressSpinner from "primevue/progressspinner";
import externalEntityService, { type ExternalEntity } from "@/services/externalEntityService";

const props = defineProps<{
  projectId: number;
}>();

const toast = useToast();
const confirm = useConfirm();

const entities = ref<ExternalEntity[]>([]);
const loading = ref(false);
const saving = ref(false);
const showCreateDialog = ref(false);
const showViewDialog = ref(false);
const editingEntity = ref<ExternalEntity | null>(null);
const selectedEntity = ref<ExternalEntity | null>(null);

const formData = ref<Partial<ExternalEntity>>({
  type: "Person",
  name: "",
  email: "",
  organization: "",
  phone: "",
  notes: "",
});

const loadEntities = async () => {
  try {
    loading.value = true;
    entities.value = await externalEntityService.getExternalEntities(props.projectId);
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load entities",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const saveEntity = async () => {
  if (!formData.value.name || !formData.value.type) {
    toast.add({
      severity: "warn",
      summary: "Warning",
      detail: "Name and type are required",
      life: 3000,
    });
    return;
  }

  try {
    saving.value = true;
    const entity = { ...formData.value, projectId: props.projectId } as ExternalEntity;

    if (editingEntity.value?.id) {
      await externalEntityService.updateExternalEntity(
        props.projectId,
        editingEntity.value.id,
        entity,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Entity updated", life: 3000 });
    } else {
      await externalEntityService.createExternalEntity(props.projectId, entity);
      toast.add({ severity: "success", summary: "Success", detail: "Entity created", life: 3000 });
    }

    showCreateDialog.value = false;
    await loadEntities();
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to save entity",
      life: 3000,
    });
  } finally {
    saving.value = false;
  }
};

const editEntity = (entity: ExternalEntity) => {
  editingEntity.value = entity;
  formData.value = { ...entity };
  showCreateDialog.value = true;
};

const viewEntity = async (entity: ExternalEntity) => {
  try {
    selectedEntity.value = await externalEntityService.getExternalEntity(
      props.projectId,
      entity.id!,
    );
    showViewDialog.value = true;
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load entity details",
      life: 3000,
    });
  }
};

const confirmDelete = (entity: ExternalEntity) => {
  confirm.require({
    message: `Are you sure you want to delete ${entity.name}?`,
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    accept: async () => {
      try {
        await externalEntityService.deleteExternalEntity(props.projectId, entity.id!);
        toast.add({
          severity: "success",
          summary: "Success",
          detail: "Entity deleted",
          life: 3000,
        });
        await loadEntities();
      } catch (error) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to delete entity",
          life: 3000,
        });
      }
    },
  });
};

const getSeverityColor = (severity: string) => {
  const colors: Record<string, string> = {
    Low: "info",
    Medium: "warning",
    High: "danger",
    Critical: "danger",
  };
  return colors[severity] || "info";
};

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString();
};

onMounted(() => {
  loadEntities();
});
</script>

<style scoped>
.entities-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.panel-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.panel-title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #0f172a;
}

.panel-subtitle {
  margin: 0.125rem 0 0;
  font-size: 0.8125rem;
  color: #64748b;
}

.loading-state {
  display: flex;
  justify-content: center;
  padding: 3rem 0;
}

.action-btns {
  display: flex;
  gap: 0.25rem;
}

.dialog-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
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

.view-entity {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.view-section {
  display: flex;
  flex-direction: column;
}

.section-title {
  margin: 0 0 0.75rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

@media (max-width: 640px) {
  .detail-grid {
    grid-template-columns: 1fr;
  }
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.detail-label {
  font-size: 0.75rem;
  font-weight: 600;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.detail-value {
  margin: 0;
  color: #1e293b;
}

.notes-text {
  margin: 0;
  font-size: 0.875rem;
  color: #475569;
}

.item-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.item-block {
  padding: 0.75rem;
  background: #f8fafc;
  border-radius: 0.5rem;
}

.item-text {
  margin: 0;
  font-size: 0.875rem;
  color: #334155;
}

.interview-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.25rem;
}

.interview-date {
  font-size: 0.75rem;
  color: #94a3b8;
}
</style>
