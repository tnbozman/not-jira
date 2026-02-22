<template>
  <div>
    <!-- Loading -->
    <div v-if="loading" class="flex justify-center items-center py-24">
      <ProgressSpinner />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="space-y-4">
      <Message severity="error" :closable="false">{{ error }}</Message>
      <Button label="Go Back" icon="pi pi-arrow-left" text @click="goBack" />
    </div>

    <div v-else-if="project" class="space-y-6">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
        <div class="flex items-start gap-3">
          <Button icon="pi pi-arrow-left" text rounded @click="goBack" />
          <div class="space-y-1">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded text-xs font-mono font-bold bg-blue-100 text-blue-700">
              {{ project.key }}
            </span>
            <h1 class="text-2xl font-bold text-surface-900">{{ project.name }}</h1>
          </div>
        </div>
        <div class="flex gap-2">
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
            <Button label="Save" icon="pi pi-check" size="small" @click="saveProject" :loading="saving" />
          </template>
        </div>
      </div>

      <TabView>
        <TabPanel header="Overview" value="overview">
          <Card class="border border-surface-200">
            <template #content>
              <div v-if="!editing" class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-surface-400 mb-1">Project Key</label>
                  <p class="text-surface-800">{{ project.key }}</p>
                </div>
                <div>
                  <label class="block text-sm font-medium text-surface-400 mb-1">Project Name</label>
                  <p class="text-surface-800">{{ project.name }}</p>
                </div>
                <div>
                  <label class="block text-sm font-medium text-surface-400 mb-1">Description</label>
                  <p class="text-surface-800">{{ project.description || 'No description' }}</p>
                </div>
              </div>

              <form v-else @submit.prevent="saveProject" class="space-y-5">
                <div class="space-y-1.5">
                  <label for="key" class="block text-sm font-medium text-surface-700">Project Key</label>
                  <InputText id="key" v-model="editedProject!.key" disabled class="w-full" />
                  <small class="text-surface-400 text-xs">Project key cannot be changed</small>
                </div>
                <div class="space-y-1.5">
                  <label for="name" class="block text-sm font-medium text-surface-700">Project Name *</label>
                  <InputText id="name" v-model="editedProject!.name" placeholder="Enter project name" class="w-full" />
                </div>
                <div class="space-y-1.5">
                  <label for="description" class="block text-sm font-medium text-surface-700">Description</label>
                  <Textarea id="description" v-model="editedProject!.description" placeholder="Enter project description" rows="5" auto-resize class="w-full" />
                </div>
              </form>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="External Entities" value="entities">
          <Card class="border border-surface-200">
            <template #content>
              <div class="flex flex-col items-center text-center py-8 gap-4">
                <div class="w-16 h-16 rounded-full bg-blue-100 flex items-center justify-center">
                  <i class="pi pi-users text-blue-600 text-2xl"></i>
                </div>
                <h3 class="text-xl font-semibold text-surface-800">External Entities</h3>
                <p class="text-surface-500 max-w-lg">Manage people and clients, track their problems, outcomes, and success metrics.</p>
                <Button label="Manage Entities" icon="pi pi-arrow-right" @click="navigateToEntities" />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="Knowledge Graph" value="graph">
          <Card class="border border-surface-200">
            <template #content>
              <div class="flex flex-col items-center text-center py-8 gap-4">
                <div class="w-16 h-16 rounded-full bg-emerald-100 flex items-center justify-center">
                  <i class="pi pi-sitemap text-emerald-600 text-2xl"></i>
                </div>
                <h3 class="text-xl font-semibold text-surface-800">Knowledge Graph</h3>
                <p class="text-surface-500 max-w-lg">Visualize relationships between entities, problems, outcomes, and interviews.</p>
                <Button label="View Graph" icon="pi pi-arrow-right" @click="navigateToGraph" />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="User Story Map" value="story-map">
          <Card class="border border-surface-200">
            <template #content>
              <div class="flex flex-col items-center text-center py-8 gap-4">
                <div class="w-16 h-16 rounded-full bg-amber-100 flex items-center justify-center">
                  <i class="pi pi-table text-amber-600 text-2xl"></i>
                </div>
                <h3 class="text-xl font-semibold text-surface-800">User Story Map</h3>
                <p class="text-surface-500 max-w-lg">Outcome-focused themes, epics, stories, and spikes with sprint planning.</p>
                <Button label="View Story Map" icon="pi pi-arrow-right" @click="navigateToStoryMap" />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="Backlog" value="backlog">
          <Card class="border border-surface-200">
            <template #content>
              <div class="flex flex-col items-center text-center py-8 gap-4">
                <div class="w-16 h-16 rounded-full bg-purple-100 flex items-center justify-center">
                  <i class="pi pi-list text-purple-600 text-2xl"></i>
                </div>
                <h3 class="text-xl font-semibold text-surface-800">Product Backlog</h3>
                <p class="text-surface-500 max-w-lg">View sprints and backlog items with filtering. Manage sprint planning, reviews, and retrospectives.</p>
                <Button label="View Backlog" icon="pi pi-arrow-right" @click="navigateToBacklog" />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="Members" value="members">
          <Card class="border border-surface-200">
            <template #title>
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-semibold text-surface-800">Project Members</h2>
                <Button label="Add Member" icon="pi pi-plus" size="small" @click="showAddMemberDialog" />
              </div>
            </template>
            <template #content>
              <DataTable :value="project.members" :rows="10" stripedRows>
                <Column field="userName" header="Name"></Column>
                <Column field="userEmail" header="Email"></Column>
                <Column field="role" header="Role">
                  <template #body="slotProps">
                    <Tag :value="getRoleLabel(slotProps.data.role)" :severity="getRoleSeverity(slotProps.data.role)" />
                  </template>
                </Column>
                <Column header="Actions" style="width: 5rem">
                  <template #body="slotProps">
                    <Button icon="pi pi-trash" severity="danger" text size="small" @click="confirmRemoveMember(slotProps.data)" />
                  </template>
                </Column>
              </DataTable>
            </template>
          </Card>
        </TabPanel>
      </TabView>

      <!-- Add Member Dialog -->
      <Dialog v-model:visible="addMemberDialogVisible" header="Add Project Member" :modal="true" :style="{ width: '28rem' }">
        <form @submit.prevent="addMember" class="space-y-4">
          <div class="space-y-1.5">
            <label for="userId" class="block text-sm font-medium text-surface-700">User ID *</label>
            <InputText id="userId" v-model="newMember.userId" placeholder="Enter Keycloak user ID" class="w-full" />
          </div>
          <div class="space-y-1.5">
            <label for="userName" class="block text-sm font-medium text-surface-700">User Name *</label>
            <InputText id="userName" v-model="newMember.userName" placeholder="Enter user name" class="w-full" />
          </div>
          <div class="space-y-1.5">
            <label for="userEmail" class="block text-sm font-medium text-surface-700">User Email *</label>
            <InputText id="userEmail" v-model="newMember.userEmail" type="email" placeholder="Enter user email" class="w-full" />
          </div>
          <div class="space-y-1.5">
            <label for="role" class="block text-sm font-medium text-surface-700">Role *</label>
            <Dropdown id="role" v-model="newMember.role" :options="roleOptions" option-label="label" option-value="value" placeholder="Select a role" class="w-full" />
          </div>
          <Message v-if="memberError" severity="error" :closable="false">{{ memberError }}</Message>
        </form>
        <template #footer>
          <Button label="Cancel" text @click="addMemberDialogVisible = false" />
          <Button label="Add Member" icon="pi pi-check" @click="addMember" :loading="addingMember" />
        </template>
      </Dialog>

      <!-- Remove Member Confirmation Dialog -->
      <Dialog v-model:visible="removeMemberDialogVisible" header="Remove Member" :modal="true" :style="{ width: '26rem' }">
        <div class="flex items-start gap-4">
          <div class="w-10 h-10 rounded-full bg-red-100 flex items-center justify-center shrink-0">
            <i class="pi pi-exclamation-triangle text-red-600"></i>
          </div>
          <p class="text-surface-700">Are you sure you want to remove <strong>{{ memberToRemove?.userName }}</strong> from this project?</p>
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
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Button from 'primevue/button'
import Card from 'primevue/card'
import ProgressSpinner from 'primevue/progressspinner'
import Message from 'primevue/message'
import Tag from 'primevue/tag'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import Dropdown from 'primevue/dropdown'
import { projectService, type Project, type ProjectMember, ProjectRole } from '@/services/projectService'

const router = useRouter()
const route = useRoute()

const project = ref<Project | null>(null)
const editedProject = ref<Project | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)
const editing = ref(false)
const saving = ref(false)

const addMemberDialogVisible = ref(false)
const removeMemberDialogVisible = ref(false)
const memberToRemove = ref<ProjectMember | null>(null)
const addingMember = ref(false)
const memberError = ref<string | null>(null)

const newMember = ref<ProjectMember>({
  userId: '',
  userName: '',
  userEmail: '',
  role: ProjectRole.Developer,
})

const roleOptions = [
  { label: 'Developer', value: ProjectRole.Developer },
  { label: 'Product Manager', value: ProjectRole.ProductManager },
  { label: 'Project Sponsor', value: ProjectRole.ProjectSponsor },
]

const loadProject = async () => {
  try {
    loading.value = true
    error.value = null
    const id = parseInt(route.params.id as string)
    project.value = await projectService.getProject(id)
  } catch (err) {
    error.value = 'Failed to load project. Please try again.'
    console.error('Error loading project:', err)
  } finally {
    loading.value = false
  }
}

const startEdit = () => {
  editedProject.value = { ...project.value! }
  editing.value = true
}

const cancelEdit = () => {
  editedProject.value = null
  editing.value = false
}

const saveProject = async () => {
  if (!editedProject.value || !project.value?.id) return

  try {
    saving.value = true
    await projectService.updateProject(project.value.id, editedProject.value)
    project.value = { ...editedProject.value }
    editing.value = false
  } catch (err) {
    error.value = 'Failed to save project. Please try again.'
    console.error('Error saving project:', err)
  } finally {
    saving.value = false
  }
}

const showAddMemberDialog = () => {
  newMember.value = {
    userId: '',
    userName: '',
    userEmail: '',
    role: ProjectRole.Developer,
  }
  memberError.value = null
  addMemberDialogVisible.value = true
}

const addMember = async () => {
  if (!project.value?.id) return

  memberError.value = null

  if (!newMember.value.userId || !newMember.value.userName || !newMember.value.userEmail) {
    memberError.value = 'All fields are required'
    return
  }

  try {
    addingMember.value = true
    await projectService.addMember(project.value.id, newMember.value)
    addMemberDialogVisible.value = false
    await loadProject()
  } catch (err) {
    if (err instanceof Error) {
      memberError.value = err.message
    } else {
      memberError.value = 'Failed to add member. Please try again.'
    }
    console.error('Error adding member:', err)
  } finally {
    addingMember.value = false
  }
}

const confirmRemoveMember = (member: ProjectMember) => {
  memberToRemove.value = member
  removeMemberDialogVisible.value = true
}

const removeMember = async () => {
  if (!project.value?.id || !memberToRemove.value?.id) return

  try {
    await projectService.removeMember(project.value.id, memberToRemove.value.id)
    removeMemberDialogVisible.value = false
    memberToRemove.value = null
    await loadProject()
  } catch (err) {
    error.value = 'Failed to remove member. Please try again.'
    console.error('Error removing member:', err)
  }
}

const getRoleLabel = (role: ProjectRole): string => {
  switch (role) {
    case ProjectRole.Developer:
      return 'Developer'
    case ProjectRole.ProductManager:
      return 'Product Manager'
    case ProjectRole.ProjectSponsor:
      return 'Project Sponsor'
    default:
      return 'Unknown'
  }
}

const getRoleSeverity = (role: ProjectRole): string => {
  switch (role) {
    case ProjectRole.Developer:
      return 'info'
    case ProjectRole.ProductManager:
      return 'success'
    case ProjectRole.ProjectSponsor:
      return 'warning'
    default:
      return 'secondary'
  }
}

const goBack = () => {
  router.push('/projects')
}

const navigateToEntities = () => {
  router.push(`/projects/${project.value?.id}/entities`)
}

const navigateToGraph = () => {
  router.push(`/projects/${project.value?.id}/graph`)
}

const navigateToStoryMap = () => {
  router.push(`/projects/${project.value?.id}/story-map`)
}

const navigateToBacklog = () => {
  router.push(`/projects/${project.value?.id}/backlog`)
}

onMounted(() => {
  loadProject()
})
</script>

