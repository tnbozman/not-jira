<template>
  <div class="project-detail-view">
    <div v-if="loading" class="loading">
      <ProgressSpinner />
    </div>

    <div v-else-if="error" class="error">
      <Message severity="error" :closable="false">{{ error }}</Message>
      <Button label="Go Back" @click="goBack" />
    </div>

    <div v-else-if="project">
      <div class="header">
        <div class="title-section">
          <Button icon="pi pi-arrow-left" text @click="goBack" />
          <div>
            <Tag :value="project.key" severity="info" class="project-key-tag" />
            <h1>{{ project.name }}</h1>
          </div>
        </div>
        <div class="actions">
          <Button 
            v-if="!editing" 
            label="Edit" 
            icon="pi pi-pencil" 
            @click="startEdit" 
          />
          <Button 
            v-if="editing" 
            label="Cancel" 
            severity="secondary" 
            text 
            @click="cancelEdit" 
          />
          <Button 
            v-if="editing" 
            label="Save" 
            icon="pi pi-check" 
            @click="saveProject" 
            :loading="saving"
          />
        </div>
      </div>

      <TabView>
        <TabPanel header="Overview" value="overview">
          <Card>
            <template #content>
              <div v-if="!editing" class="project-info">
                <div class="info-field">
                  <label>Project Key</label>
                  <p>{{ project.key }}</p>
                </div>
                <div class="info-field">
                  <label>Project Name</label>
                  <p>{{ project.name }}</p>
                </div>
                <div class="info-field">
                  <label>Description</label>
                  <p>{{ project.description || 'No description' }}</p>
                </div>
              </div>

              <form v-else @submit.prevent="saveProject" class="edit-form">
                <div class="form-field">
                  <label for="key">Project Key</label>
                  <InputText 
                    id="key"
                    v-model="editedProject!.key"
                    disabled
                  />
                  <small class="help-text">Project key cannot be changed</small>
                </div>

                <div class="form-field">
                  <label for="name">Project Name *</label>
                  <InputText 
                    id="name"
                    v-model="editedProject!.name"
                    placeholder="Enter project name"
                  />
                </div>

                <div class="form-field">
                  <label for="description">Description</label>
                  <Textarea 
                    id="description"
                    v-model="editedProject!.description"
                    placeholder="Enter project description"
                    rows="5"
                    auto-resize
                  />
                </div>
              </form>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="External Entities" value="entities">
          <Card>
            <template #content>
              <div class="quick-nav-card">
                <i class="pi pi-users" style="font-size: 2rem; color: #3b82f6;"></i>
                <h3>External Entities</h3>
                <p>Manage people and clients, track their problems, outcomes, and success metrics.</p>
                <Button 
                  label="Manage Entities" 
                  icon="pi pi-arrow-right" 
                  @click="navigateToEntities" 
                />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="Knowledge Graph" value="graph">
          <Card>
            <template #content>
              <div class="quick-nav-card">
                <i class="pi pi-sitemap" style="font-size: 2rem; color: #10b981;"></i>
                <h3>Knowledge Graph</h3>
                <p>Visualize relationships between entities, problems, outcomes, and interviews.</p>
                <Button 
                  label="View Graph" 
                  icon="pi pi-arrow-right" 
                  @click="navigateToGraph" 
                />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="User Story Map" value="story-map">
          <Card>
            <template #content>
              <div class="quick-nav-card">
                <i class="pi pi-table" style="font-size: 2rem; color: #f59e0b;"></i>
                <h3>User Story Map</h3>
                <p>Outcome-focused themes, epics, stories, and spikes with sprint planning.</p>
                <Button 
                  label="View Story Map" 
                  icon="pi pi-arrow-right" 
                  @click="navigateToStoryMap" 
                />
              </div>
            </template>
          </Card>
        </TabPanel>

        <TabPanel header="Members" value="members">
          <Card>
            <template #title>
              <div class="members-header">
                <h2>Project Members</h2>
                <Button 
                  label="Add Member" 
                  icon="pi pi-plus" 
                  @click="showAddMemberDialog" 
                />
              </div>
            </template>
            <template #content>
              <DataTable :value="project.members" :rows="10">
                <Column field="userName" header="Name"></Column>
                <Column field="userEmail" header="Email"></Column>
                <Column field="role" header="Role">
                  <template #body="slotProps">
                    <Tag :value="getRoleLabel(slotProps.data.role)" :severity="getRoleSeverity(slotProps.data.role)" />
                  </template>
                </Column>
                <Column header="Actions">
                  <template #body="slotProps">
                    <Button 
                      icon="pi pi-trash" 
                      severity="danger" 
                      text 
                      @click="confirmRemoveMember(slotProps.data)" 
                    />
                  </template>
                </Column>
              </DataTable>
            </template>
          </Card>
        </TabPanel>
      </TabView>

      <!-- Add Member Dialog -->
      <Dialog v-model:visible="addMemberDialogVisible" header="Add Project Member" :modal="true" style="width: 500px">
        <form @submit.prevent="addMember">
          <div class="form-field">
            <label for="userId">User ID *</label>
            <InputText 
              id="userId"
              v-model="newMember.userId"
              placeholder="Enter Keycloak user ID"
            />
          </div>

          <div class="form-field">
            <label for="userName">User Name *</label>
            <InputText 
              id="userName"
              v-model="newMember.userName"
              placeholder="Enter user name"
            />
          </div>

          <div class="form-field">
            <label for="userEmail">User Email *</label>
            <InputText 
              id="userEmail"
              v-model="newMember.userEmail"
              type="email"
              placeholder="Enter user email"
            />
          </div>

          <div class="form-field">
            <label for="role">Role *</label>
            <Dropdown 
              id="role"
              v-model="newMember.role"
              :options="roleOptions"
              option-label="label"
              option-value="value"
              placeholder="Select a role"
            />
          </div>

          <Message v-if="memberError" severity="error" :closable="false">{{ memberError }}</Message>
        </form>
        
        <template #footer>
          <Button label="Cancel" text @click="addMemberDialogVisible = false" />
          <Button label="Add Member" @click="addMember" :loading="addingMember" />
        </template>
      </Dialog>

      <!-- Remove Member Confirmation Dialog -->
      <Dialog v-model:visible="removeMemberDialogVisible" header="Remove Member" :modal="true">
        <p>Are you sure you want to remove {{ memberToRemove?.userName }} from this project?</p>
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

onMounted(() => {
  loadProject()
})
</script>

<style scoped>
.project-detail-view {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.loading {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.error {
  margin-bottom: 2rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
}

.title-section {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
}

.title-section > div {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.title-section h1 {
  margin: 0;
  font-size: 1.75rem;
  font-weight: 600;
}

.project-key-tag {
  font-family: monospace;
  font-weight: 600;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.project-info {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.info-field label {
  display: block;
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: var(--text-color-secondary);
}

.info-field p {
  margin: 0;
  font-size: 1rem;
}

.edit-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-field {
  margin-bottom: 1rem;
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

.members-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.members-header h2 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.quick-nav-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 2rem;
  gap: 1rem;
}

.quick-nav-card h3 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
}

.quick-nav-card p {
  margin: 0;
  color: #666;
  max-width: 600px;
}
</style>
