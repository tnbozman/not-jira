<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h2 class="text-2xl font-bold text-surface-900">External Entities</h2>
        <p class="text-surface-500 text-sm mt-1">Manage people and clients for this project</p>
      </div>
      <Button label="Add Entity" icon="pi pi-plus" @click="showCreateDialog = true" />
    </div>

    <Card class="border border-surface-200">
      <template #content>
        <div v-if="loading" class="flex justify-center py-12">
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
              <div class="flex gap-1">
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
      </template>
    </Card>

    <!-- Create/Edit Dialog -->
    <Dialog
      v-model:visible="showCreateDialog"
      :header="editingEntity?.id ? 'Edit Entity' : 'Create Entity'"
      :style="{ width: '32rem' }"
      :modal="true"
    >
      <div class="space-y-4">
        <div class="space-y-1.5">
          <label class="block text-sm font-medium text-surface-700">Type *</label>
          <Dropdown
            v-model="formData.type"
            :options="['Person', 'Client']"
            placeholder="Select Type"
            class="w-full"
          />
        </div>
        <div class="space-y-1.5">
          <label class="block text-sm font-medium text-surface-700">Name *</label>
          <InputText v-model="formData.name" class="w-full" placeholder="Enter name" />
        </div>
        <div class="space-y-1.5">
          <label class="block text-sm font-medium text-surface-700">Email</label>
          <InputText
            v-model="formData.email"
            class="w-full"
            type="email"
            placeholder="email@example.com"
          />
        </div>
        <div class="space-y-1.5">
          <label class="block text-sm font-medium text-surface-700">Organization</label>
          <InputText
            v-model="formData.organization"
            class="w-full"
            placeholder="Organization name"
          />
        </div>
        <div class="space-y-1.5">
          <label class="block text-sm font-medium text-surface-700">Phone</label>
          <InputText v-model="formData.phone" class="w-full" placeholder="Phone number" />
        </div>
        <div class="space-y-1.5">
          <label class="block text-sm font-medium text-surface-700">Notes</label>
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
      <div v-if="selectedEntity" class="space-y-6">
        <div>
          <h3 class="text-lg font-semibold text-surface-800 mb-3">Basic Information</h3>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <span class="text-xs font-semibold text-surface-400 uppercase tracking-wider"
                >Name</span
              >
              <p class="text-surface-800 mt-0.5">{{ selectedEntity.name }}</p>
            </div>
            <div>
              <span class="text-xs font-semibold text-surface-400 uppercase tracking-wider"
                >Type</span
              >
              <div class="mt-0.5">
                <Tag
                  :value="selectedEntity.type"
                  :severity="selectedEntity.type === 'Person' ? 'info' : 'success'"
                />
              </div>
            </div>
            <div v-if="selectedEntity.email">
              <span class="text-xs font-semibold text-surface-400 uppercase tracking-wider"
                >Email</span
              >
              <p class="text-surface-800 mt-0.5">{{ selectedEntity.email }}</p>
            </div>
            <div v-if="selectedEntity.organization">
              <span class="text-xs font-semibold text-surface-400 uppercase tracking-wider"
                >Organization</span
              >
              <p class="text-surface-800 mt-0.5">{{ selectedEntity.organization }}</p>
            </div>
            <div v-if="selectedEntity.phone">
              <span class="text-xs font-semibold text-surface-400 uppercase tracking-wider"
                >Phone</span
              >
              <p class="text-surface-800 mt-0.5">{{ selectedEntity.phone }}</p>
            </div>
          </div>
        </div>

        <div v-if="selectedEntity.notes">
          <h3 class="text-lg font-semibold text-surface-800 mb-2">Notes</h3>
          <p class="text-surface-600 text-sm">{{ selectedEntity.notes }}</p>
        </div>

        <div v-if="selectedEntity.problems && selectedEntity.problems.length > 0">
          <h3 class="text-lg font-semibold text-surface-800 mb-3">
            Problems ({{ selectedEntity.problems.length }})
          </h3>
          <div class="space-y-2">
            <div
              v-for="problem in selectedEntity.problems"
              :key="problem.id"
              class="p-3 bg-surface-50 rounded-lg"
            >
              <Tag
                :value="problem.severity"
                :severity="getSeverityColor(problem.severity)"
                class="mb-2"
              />
              <p class="text-sm text-surface-700">{{ problem.description }}</p>
            </div>
          </div>
        </div>

        <div v-if="selectedEntity.interviews && selectedEntity.interviews.length > 0">
          <h3 class="text-lg font-semibold text-surface-800 mb-3">
            Interviews ({{ selectedEntity.interviews.length }})
          </h3>
          <div class="space-y-2">
            <div
              v-for="interview in selectedEntity.interviews"
              :key="interview.id"
              class="p-3 bg-surface-50 rounded-lg"
            >
              <div class="flex items-center justify-between mb-1">
                <Tag :value="interview.type" />
                <span class="text-xs text-surface-400">{{
                  formatDate(interview.interviewDate)
                }}</span>
              </div>
              <p v-if="interview.summary" class="text-sm text-surface-700">
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
import { useRoute } from "vue-router";
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

const route = useRoute();
const toast = useToast();
const confirm = useConfirm();

const projectId = ref<number>(parseInt(route.params.id as string));
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
    entities.value = await externalEntityService.getExternalEntities(projectId.value);
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
    const entity = { ...formData.value, projectId: projectId.value } as ExternalEntity;

    if (editingEntity.value?.id) {
      await externalEntityService.updateExternalEntity(
        projectId.value,
        editingEntity.value.id,
        entity,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Entity updated", life: 3000 });
    } else {
      await externalEntityService.createExternalEntity(projectId.value, entity);
      toast.add({ severity: "success", summary: "Success", detail: "Entity created", life: 3000 });
    }

    showCreateDialog.value = false;
    await loadEntities();
  } catch (error) {
    toast.add({ severity: "error", summary: "Error", detail: "Failed to save entity", life: 3000 });
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
      projectId.value,
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
        await externalEntityService.deleteExternalEntity(projectId.value, entity.id!);
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
.external-entities-view {
  padding: 1.5rem;
}

.card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.card-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.loading-container {
  display: flex;
  justify-content: center;
  padding: 3rem;
}

.entity-form .field {
  margin-bottom: 1.5rem;
}

.entity-form label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.entity-details .detail-section {
  margin-bottom: 2rem;
}

.entity-details h3 {
  margin-bottom: 1rem;
  font-size: 1.2rem;
  color: #333;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.detail-item .label {
  font-weight: 600;
  color: #666;
  font-size: 0.9rem;
}

.detail-item .value {
  color: #333;
}

.problem-card,
.interview-card {
  background: #f8f9fa;
  padding: 1rem;
  border-radius: 6px;
  margin-bottom: 0.75rem;
}

.interview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.interview-header span {
  font-size: 0.9rem;
  color: #666;
}
</style>
