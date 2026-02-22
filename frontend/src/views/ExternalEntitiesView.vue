<template>
  <div class="external-entities-view">
    <div class="card">
      <div class="card-header">
        <h2>External Entities</h2>
        <Button label="Add Entity" icon="pi pi-plus" @click="showCreateDialog = true" />
      </div>

      <div v-if="loading" class="loading-container">
        <ProgressSpinner />
      </div>

      <DataTable v-else :value="entities" class="p-datatable-sm" striped-rows>
        <Column field="name" header="Name" sortable></Column>
        <Column field="type" header="Type" sortable>
          <template #body="{ data }">
            <Tag :value="data.type" :severity="data.type === 'Person' ? 'info' : 'success'" />
          </template>
        </Column>
        <Column field="email" header="Email"></Column>
        <Column field="organization" header="Organization"></Column>
        <Column header="Actions" style="width: 12rem">
          <template #body="{ data }">
            <Button icon="pi pi-eye" class="p-button-rounded p-button-text" 
                    @click="viewEntity(data)" title="View Details" />
            <Button icon="pi pi-pencil" class="p-button-rounded p-button-text" 
                    @click="editEntity(data)" title="Edit" />
            <Button icon="pi pi-trash" class="p-button-rounded p-button-text p-button-danger" 
                    @click="confirmDelete(data)" title="Delete" />
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- Create/Edit Dialog -->
    <Dialog v-model:visible="showCreateDialog" :header="editingEntity?.id ? 'Edit Entity' : 'Create Entity'" 
            :style="{ width: '600px' }" :modal="true">
      <div class="entity-form">
        <div class="field">
          <label>Type *</label>
          <Dropdown v-model="formData.type" :options="['Person', 'Client']" placeholder="Select Type" 
                    class="w-full" />
        </div>

        <div class="field">
          <label>Name *</label>
          <InputText v-model="formData.name" class="w-full" placeholder="Enter name" />
        </div>

        <div class="field">
          <label>Email</label>
          <InputText v-model="formData.email" class="w-full" type="email" placeholder="email@example.com" />
        </div>

        <div class="field">
          <label>Organization</label>
          <InputText v-model="formData.organization" class="w-full" placeholder="Organization name" />
        </div>

        <div class="field">
          <label>Phone</label>
          <InputText v-model="formData.phone" class="w-full" placeholder="Phone number" />
        </div>

        <div class="field">
          <label>Notes</label>
          <Textarea v-model="formData.notes" class="w-full" rows="4" placeholder="Additional notes" />
        </div>
      </div>

      <template #footer>
        <Button label="Cancel" icon="pi pi-times" @click="showCreateDialog = false" class="p-button-text" />
        <Button label="Save" icon="pi pi-check" @click="saveEntity" :loading="saving" />
      </template>
    </Dialog>

    <!-- View Dialog -->
    <Dialog v-model:visible="showViewDialog" header="Entity Details" :style="{ width: '800px' }" :modal="true">
      <div v-if="selectedEntity" class="entity-details">
        <div class="detail-section">
          <h3>Basic Information</h3>
          <div class="detail-grid">
            <div class="detail-item">
              <span class="label">Name:</span>
              <span class="value">{{ selectedEntity.name }}</span>
            </div>
            <div class="detail-item">
              <span class="label">Type:</span>
              <Tag :value="selectedEntity.type" :severity="selectedEntity.type === 'Person' ? 'info' : 'success'" />
            </div>
            <div class="detail-item" v-if="selectedEntity.email">
              <span class="label">Email:</span>
              <span class="value">{{ selectedEntity.email }}</span>
            </div>
            <div class="detail-item" v-if="selectedEntity.organization">
              <span class="label">Organization:</span>
              <span class="value">{{ selectedEntity.organization }}</span>
            </div>
            <div class="detail-item" v-if="selectedEntity.phone">
              <span class="label">Phone:</span>
              <span class="value">{{ selectedEntity.phone }}</span>
            </div>
          </div>
        </div>

        <div class="detail-section" v-if="selectedEntity.notes">
          <h3>Notes</h3>
          <p>{{ selectedEntity.notes }}</p>
        </div>

        <div class="detail-section" v-if="selectedEntity.problems && selectedEntity.problems.length > 0">
          <h3>Problems ({{ selectedEntity.problems.length }})</h3>
          <div v-for="problem in selectedEntity.problems" :key="problem.id" class="problem-card">
            <Tag :value="problem.severity" :severity="getSeverityColor(problem.severity)" class="mb-2" />
            <p>{{ problem.description }}</p>
          </div>
        </div>

        <div class="detail-section" v-if="selectedEntity.interviews && selectedEntity.interviews.length > 0">
          <h3>Interviews ({{ selectedEntity.interviews.length }})</h3>
          <div v-for="interview in selectedEntity.interviews" :key="interview.id" class="interview-card">
            <div class="interview-header">
              <Tag :value="interview.type" />
              <span>{{ formatDate(interview.interviewDate) }}</span>
            </div>
            <p v-if="interview.summary">{{ interview.summary }}</p>
          </div>
        </div>
      </div>

      <template #footer>
        <Button label="Close" icon="pi pi-times" @click="showViewDialog = false" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import Textarea from 'primevue/textarea';
import Dropdown from 'primevue/dropdown';
import Tag from 'primevue/tag';
import ProgressSpinner from 'primevue/progressspinner';
import externalEntityService, { type ExternalEntity } from '@/services/externalEntityService';

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
  type: 'Person',
  name: '',
  email: '',
  organization: '',
  phone: '',
  notes: ''
});

const loadEntities = async () => {
  try {
    loading.value = true;
    entities.value = await externalEntityService.getExternalEntities(projectId.value);
  } catch (error) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to load entities', life: 3000 });
  } finally {
    loading.value = false;
  }
};

const saveEntity = async () => {
  if (!formData.value.name || !formData.value.type) {
    toast.add({ severity: 'warn', summary: 'Warning', detail: 'Name and type are required', life: 3000 });
    return;
  }

  try {
    saving.value = true;
    const entity = { ...formData.value, projectId: projectId.value } as ExternalEntity;

    if (editingEntity.value?.id) {
      await externalEntityService.updateExternalEntity(projectId.value, editingEntity.value.id, entity);
      toast.add({ severity: 'success', summary: 'Success', detail: 'Entity updated', life: 3000 });
    } else {
      await externalEntityService.createExternalEntity(projectId.value, entity);
      toast.add({ severity: 'success', summary: 'Success', detail: 'Entity created', life: 3000 });
    }

    showCreateDialog.value = false;
    await loadEntities();
  } catch (error) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to save entity', life: 3000 });
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
    selectedEntity.value = await externalEntityService.getExternalEntity(projectId.value, entity.id!);
    showViewDialog.value = true;
  } catch (error) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to load entity details', life: 3000 });
  }
};

const confirmDelete = (entity: ExternalEntity) => {
  confirm.require({
    message: `Are you sure you want to delete ${entity.name}?`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',
    accept: async () => {
      try {
        await externalEntityService.deleteExternalEntity(projectId.value, entity.id!);
        toast.add({ severity: 'success', summary: 'Success', detail: 'Entity deleted', life: 3000 });
        await loadEntities();
      } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete entity', life: 3000 });
      }
    }
  });
};

const getSeverityColor = (severity: string) => {
  const colors: Record<string, string> = {
    Low: 'info',
    Medium: 'warning',
    High: 'danger',
    Critical: 'danger'
  };
  return colors[severity] || 'info';
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
