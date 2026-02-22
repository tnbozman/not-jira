<template>
  <div class="user-story-map-view">
    <div class="header">
      <h1>User Story Map</h1>
      <div class="actions">
        <Button label="Add Theme" icon="pi pi-plus" @click="showThemeDialog = true" />
        <Button label="Manage Sprints" icon="pi pi-calendar" outlined @click="showSprintsDialog = true" />
      </div>
    </div>

    <div v-if="loading" class="loading">
      <ProgressSpinner />
    </div>

    <div v-else-if="error" class="error">
      <Message severity="error" :closable="false">{{ error }}</Message>
    </div>

    <div v-else class="story-map">
      <!-- Themes Row -->
      <div class="themes-row">
        <div v-for="theme in themes" :key="theme.id" class="theme-card">
          <Card>
            <template #header>
              <div class="theme-header">
                <h3>{{ theme.name }}</h3>
                <div class="theme-actions">
                  <Button icon="pi pi-pencil" text @click="editTheme(theme)" />
                  <Button icon="pi pi-plus" text @click="showAddEpicDialog(theme)" />
                  <Button icon="pi pi-trash" text severity="danger" @click="confirmDeleteTheme(theme)" />
                </div>
              </div>
            </template>
            <template #content>
              <p v-if="theme.description" class="theme-description">{{ theme.description }}</p>
              <Tag v-if="theme.outcome" severity="success" :value="'Outcome: ' + theme.outcome.description" />

              <!-- Epics Column -->
              <div class="epics-column">
                <div v-for="epic in theme.epics" :key="epic.id" class="epic-card">
                  <Card>
                    <template #header>
                      <div class="epic-header">
                        <h4>{{ epic.name }}</h4>
                        <div class="epic-actions">
                          <Button icon="pi pi-pencil" text size="small" @click="editEpic(theme, epic)" />
                          <Button icon="pi pi-plus" text size="small" @click="showAddStoryDialog(epic)" />
                          <Button icon="pi pi-trash" text size="small" severity="danger" @click="confirmDeleteEpic(theme, epic)" />
                        </div>
                      </div>
                    </template>
                    <template #content>
                      <p v-if="epic.description" class="epic-description">{{ epic.description }}</p>
                      <Tag v-if="epic.outcome" severity="info" :value="'Outcome: ' + epic.outcome.description" />

                      <!-- Stories/Spikes -->
                      <div class="work-items">
                        <div v-for="story in epic.stories" :key="'story-' + story.id" class="work-item story-item">
                          <div class="work-item-header">
                            <i class="pi pi-bookmark"></i>
                            <span>{{ story.title }}</span>
                          </div>
                          <div class="work-item-meta">
                            <Tag :value="story.status" />
                            <Tag v-if="story.storyPoints" :value="story.storyPoints + ' pts'" severity="info" />
                            <Tag v-if="story.sprint" :value="story.sprint.name" severity="warning" />
                          </div>
                          <div class="work-item-actions">
                            <Button icon="pi pi-pencil" text size="small" @click="editStory(epic, story)" />
                            <Button icon="pi pi-trash" text size="small" severity="danger" @click="confirmDeleteStory(epic, story)" />
                          </div>
                        </div>

                        <div v-for="spike in epic.spikes" :key="'spike-' + spike.id" class="work-item spike-item">
                          <div class="work-item-header">
                            <i class="pi pi-bolt"></i>
                            <span>{{ spike.title }}</span>
                          </div>
                          <div class="work-item-meta">
                            <Tag :value="spike.status" />
                            <Tag v-if="spike.storyPoints" :value="spike.storyPoints + ' pts'" severity="info" />
                            <Tag v-if="spike.sprint" :value="spike.sprint.name" severity="warning" />
                          </div>
                          <div class="work-item-actions">
                            <Button icon="pi pi-pencil" text size="small" @click="editSpike(epic, spike)" />
                            <Button icon="pi pi-trash" text size="small" severity="danger" @click="confirmDeleteSpike(epic, spike)" />
                          </div>
                        </div>
                      </div>
                    </template>
                  </Card>
                </div>
              </div>
            </template>
          </Card>
        </div>
      </div>

      <div v-if="themes.length === 0" class="empty-state">
        <Message severity="info" :closable="false">
          No themes yet. Click "Add Theme" to create your first outcome-focused theme.
        </Message>
      </div>
    </div>

    <!-- Theme Dialog -->
    <Dialog v-model:visible="showThemeDialog" :header="editingTheme ? 'Edit Theme' : 'Add Theme'" :modal="true" :style="{ width: '50vw' }">
      <div class="form-field">
        <label for="theme-name">Name *</label>
        <InputText id="theme-name" v-model="themeForm.name" placeholder="Enter theme name" class="w-full" />
      </div>
      <div class="form-field">
        <label for="theme-description">Description</label>
        <Textarea id="theme-description" v-model="themeForm.description" placeholder="Enter theme description" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="theme-order">Order</label>
        <InputNumber id="theme-order" v-model="themeForm.order" placeholder="Display order" class="w-full" />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showThemeDialog = false" />
        <Button label="Save" @click="saveTheme" :loading="saving" />
      </template>
    </Dialog>

    <!-- Epic Dialog -->
    <Dialog v-model:visible="showEpicDialog" :header="editingEpic ? 'Edit Epic' : 'Add Epic'" :modal="true" :style="{ width: '50vw' }">
      <div class="form-field">
        <label for="epic-name">Name *</label>
        <InputText id="epic-name" v-model="epicForm.name" placeholder="Enter epic name" class="w-full" />
      </div>
      <div class="form-field">
        <label for="epic-description">Description</label>
        <Textarea id="epic-description" v-model="epicForm.description" placeholder="Enter epic description" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="epic-order">Order</label>
        <InputNumber id="epic-order" v-model="epicForm.order" placeholder="Display order" class="w-full" />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showEpicDialog = false" />
        <Button label="Save" @click="saveEpic" :loading="saving" />
      </template>
    </Dialog>

    <!-- Story Dialog -->
    <Dialog v-model:visible="showStoryDialog" :header="editingStory ? 'Edit Story' : 'Add Story'" :modal="true" :style="{ width: '50vw' }">
      <div class="form-field">
        <label for="story-title">Title *</label>
        <InputText id="story-title" v-model="storyForm.title" placeholder="Enter story title" class="w-full" />
      </div>
      <div class="form-field">
        <label for="story-description">Description</label>
        <Textarea id="story-description" v-model="storyForm.description" placeholder="Enter story description" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="story-solution">Solution Description</label>
        <Textarea id="story-solution" v-model="storyForm.solutionDescription" placeholder="Describe the solution" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="story-acceptance">Acceptance Criteria</label>
        <Textarea id="story-acceptance" v-model="storyForm.acceptanceCriteria" placeholder="Define acceptance criteria" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="story-status">Status</label>
        <Dropdown id="story-status" v-model="storyForm.status" :options="statusOptions" placeholder="Select status" class="w-full" />
      </div>
      <div class="form-field">
        <label for="story-points">Story Points</label>
        <InputNumber id="story-points" v-model="storyForm.storyPoints" placeholder="Story points" class="w-full" />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showStoryDialog = false" />
        <Button label="Save" @click="saveStory" :loading="saving" />
      </template>
    </Dialog>

    <!-- Spike Dialog -->
    <Dialog v-model:visible="showSpikeDialog" :header="editingSpike ? 'Edit Spike' : 'Add Spike'" :modal="true" :style="{ width: '50vw' }">
      <div class="form-field">
        <label for="spike-title">Title *</label>
        <InputText id="spike-title" v-model="spikeForm.title" placeholder="Enter spike title" class="w-full" />
      </div>
      <div class="form-field">
        <label for="spike-description">Description</label>
        <Textarea id="spike-description" v-model="spikeForm.description" placeholder="Enter spike description" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="spike-goal">Investigation Goal</label>
        <Textarea id="spike-goal" v-model="spikeForm.investigationGoal" placeholder="What are we investigating?" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="spike-findings">Findings</label>
        <Textarea id="spike-findings" v-model="spikeForm.findings" placeholder="Document findings here" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="spike-status">Status</label>
        <Dropdown id="spike-status" v-model="spikeForm.status" :options="statusOptions" placeholder="Select status" class="w-full" />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showSpikeDialog = false" />
        <Button label="Save" @click="saveSpike" :loading="saving" />
      </template>
    </Dialog>

    <!-- Sprints Dialog -->
    <Dialog v-model:visible="showSprintsDialog" header="Manage Sprints" :modal="true" :style="{ width: '60vw' }">
      <div class="sprints-section">
        <Button label="Add Sprint" icon="pi pi-plus" @click="showSprintDialog = true" class="mb-3" />
        <DataTable :value="sprints" :loading="loadingSprints">
          <Column field="name" header="Name"></Column>
          <Column field="status" header="Status"></Column>
          <Column field="startDate" header="Start Date">
            <template #body="slotProps">
              {{ new Date(slotProps.data.startDate).toLocaleDateString() }}
            </template>
          </Column>
          <Column field="endDate" header="End Date">
            <template #body="slotProps">
              {{ new Date(slotProps.data.endDate).toLocaleDateString() }}
            </template>
          </Column>
          <Column header="Actions">
            <template #body="slotProps">
              <Button icon="pi pi-pencil" text @click="editSprint(slotProps.data)" />
              <Button icon="pi pi-trash" text severity="danger" @click="confirmDeleteSprint(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </Dialog>

    <!-- Sprint Dialog -->
    <Dialog v-model:visible="showSprintDialog" :header="editingSprint ? 'Edit Sprint' : 'Add Sprint'" :modal="true" :style="{ width: '50vw' }">
      <div class="form-field">
        <label for="sprint-name">Name *</label>
        <InputText id="sprint-name" v-model="sprintForm.name" placeholder="Enter sprint name" class="w-full" />
      </div>
      <div class="form-field">
        <label for="sprint-goal">Goal</label>
        <Textarea id="sprint-goal" v-model="sprintForm.goal" placeholder="Sprint goal" rows="3" class="w-full" />
      </div>
      <div class="form-field">
        <label for="sprint-start">Start Date *</label>
        <Calendar id="sprint-start" v-model="sprintFormStartDate" showIcon dateFormat="yy-mm-dd" class="w-full" />
      </div>
      <div class="form-field">
        <label for="sprint-end">End Date *</label>
        <Calendar id="sprint-end" v-model="sprintFormEndDate" showIcon dateFormat="yy-mm-dd" class="w-full" />
      </div>
      <div class="form-field">
        <label for="sprint-status">Status</label>
        <Dropdown id="sprint-status" v-model="sprintForm.status" :options="sprintStatusOptions" placeholder="Select status" class="w-full" />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showSprintDialog = false" />
        <Button label="Save" @click="saveSprint" :loading="saving" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { userStoryMapService, type Theme, type Epic, type Story, type Spike, type Sprint, Priority } from '@/services/userStoryMapService'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'

const route = useRoute()
const confirm = useConfirm()
const toast = useToast()

const projectId = computed(() => parseInt(route.params.id as string))

const loading = ref(true)
const loadingSprints = ref(false)
const saving = ref(false)
const error = ref('')

const themes = ref<Theme[]>([])
const sprints = ref<Sprint[]>([])

const showThemeDialog = ref(false)
const showEpicDialog = ref(false)
const showStoryDialog = ref(false)
const showSpikeDialog = ref(false)
const showSprintsDialog = ref(false)
const showSprintDialog = ref(false)

const editingTheme = ref(false)
const editingEpic = ref(false)
const editingStory = ref(false)
const editingSpike = ref(false)
const editingSprint = ref(false)

const currentTheme = ref<Theme | null>(null)
const currentEpic = ref<Epic | null>(null)

const themeForm = ref<Theme>({ name: '', description: '', order: 0 })
const epicForm = ref<Epic>({ name: '', description: '', order: 0 })
const storyForm = ref<Story>({ title: '', description: '', order: 0, priority: Priority.Medium, status: 'Backlog' })
const spikeForm = ref<Spike>({ title: '', description: '', order: 0, priority: Priority.Medium, status: 'Backlog' })
const sprintForm = ref<Sprint>({ name: '', goal: '', startDate: '', endDate: '', status: 'Planned' })
const sprintFormStartDate = ref<Date | null>(null)
const sprintFormEndDate = ref<Date | null>(null)

const statusOptions = ['Backlog', 'To Do', 'In Progress', 'In Review', 'Done']
const sprintStatusOptions = ['Planned', 'Active', 'Completed']

onMounted(async () => {
  await loadThemes()
  await loadSprints()
  loading.value = false
})

async function loadThemes() {
  try {
    themes.value = await userStoryMapService.getThemes(projectId.value)
  } catch (err) {
    error.value = 'Failed to load themes'
    console.error(err)
  }
}

async function loadSprints() {
  try {
    loadingSprints.value = true
    sprints.value = await userStoryMapService.getSprints(projectId.value)
  } catch (err) {
    console.error('Failed to load sprints:', err)
  } finally {
    loadingSprints.value = false
  }
}

function showAddEpicDialog(theme: Theme) {
  currentTheme.value = theme
  epicForm.value = { name: '', description: '', order: theme.epics?.length || 0, themeId: theme.id }
  editingEpic.value = false
  showEpicDialog.value = true
}

function showAddStoryDialog(epic: Epic) {
  currentEpic.value = epic
  storyForm.value = { title: '', description: '', order: epic.stories?.length || 0, priority: Priority.Medium, status: 'Backlog', epicId: epic.id }
  editingStory.value = false
  showStoryDialog.value = true
}

function editTheme(theme: Theme) {
  themeForm.value = { ...theme }
  editingTheme.value = true
  showThemeDialog.value = true
}

function editEpic(theme: Theme, epic: Epic) {
  currentTheme.value = theme
  epicForm.value = { ...epic }
  editingEpic.value = true
  showEpicDialog.value = true
}

function editStory(epic: Epic, story: Story) {
  currentEpic.value = epic
  storyForm.value = { ...story }
  editingStory.value = true
  showStoryDialog.value = true
}

function editSpike(epic: Epic, spike: Spike) {
  currentEpic.value = epic
  spikeForm.value = { ...spike }
  editingSpike.value = true
  showSpikeDialog.value = true
}

function editSprint(sprint: Sprint) {
  sprintForm.value = { ...sprint }
  sprintFormStartDate.value = new Date(sprint.startDate)
  sprintFormEndDate.value = new Date(sprint.endDate)
  editingSprint.value = true
  showSprintDialog.value = true
}

async function saveTheme() {
  try {
    saving.value = true
    if (editingTheme.value && themeForm.value.id) {
      await userStoryMapService.updateTheme(projectId.value, themeForm.value.id, themeForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Theme updated', life: 3000 })
    } else {
      await userStoryMapService.createTheme(projectId.value, themeForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Theme created', life: 3000 })
    }
    showThemeDialog.value = false
    await loadThemes()
  } catch (err) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to save theme', life: 3000 })
    console.error(err)
  } finally {
    saving.value = false
  }
}

async function saveEpic() {
  try {
    saving.value = true
    if (editingEpic.value && epicForm.value.id) {
      await userStoryMapService.updateEpic(projectId.value, currentTheme.value!.id!, epicForm.value.id, epicForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Epic updated', life: 3000 })
    } else {
      await userStoryMapService.createEpic(projectId.value, currentTheme.value!.id!, epicForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Epic created', life: 3000 })
    }
    showEpicDialog.value = false
    await loadThemes()
  } catch (err) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to save epic', life: 3000 })
    console.error(err)
  } finally {
    saving.value = false
  }
}

async function saveStory() {
  try {
    saving.value = true
    if (editingStory.value && storyForm.value.id) {
      await userStoryMapService.updateStory(projectId.value, currentEpic.value!.id!, storyForm.value.id, storyForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Story updated', life: 3000 })
    } else {
      await userStoryMapService.createStory(projectId.value, currentEpic.value!.id!, storyForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Story created', life: 3000 })
    }
    showStoryDialog.value = false
    await loadThemes()
  } catch (err) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to save story', life: 3000 })
    console.error(err)
  } finally {
    saving.value = false
  }
}

async function saveSpike() {
  try {
    saving.value = true
    if (editingSpike.value && spikeForm.value.id) {
      await userStoryMapService.updateSpike(projectId.value, currentEpic.value!.id!, spikeForm.value.id, spikeForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Spike updated', life: 3000 })
    } else {
      await userStoryMapService.createSpike(projectId.value, currentEpic.value!.id!, spikeForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Spike created', life: 3000 })
    }
    showSpikeDialog.value = false
    await loadThemes()
  } catch (err) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to save spike', life: 3000 })
    console.error(err)
  } finally {
    saving.value = false
  }
}

async function saveSprint() {
  try {
    saving.value = true
    if (sprintFormStartDate.value) {
      sprintForm.value.startDate = sprintFormStartDate.value.toISOString()
    }
    if (sprintFormEndDate.value) {
      sprintForm.value.endDate = sprintFormEndDate.value.toISOString()
    }
    
    if (editingSprint.value && sprintForm.value.id) {
      await userStoryMapService.updateSprint(projectId.value, sprintForm.value.id, sprintForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Sprint updated', life: 3000 })
    } else {
      await userStoryMapService.createSprint(projectId.value, sprintForm.value)
      toast.add({ severity: 'success', summary: 'Success', detail: 'Sprint created', life: 3000 })
    }
    showSprintDialog.value = false
    await loadSprints()
  } catch (err) {
    toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to save sprint', life: 3000 })
    console.error(err)
  } finally {
    saving.value = false
  }
}

function confirmDeleteTheme(theme: Theme) {
  confirm.require({
    message: `Are you sure you want to delete theme "${theme.name}"? This will also delete all epics, stories, and spikes under this theme.`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await userStoryMapService.deleteTheme(projectId.value, theme.id!)
        toast.add({ severity: 'success', summary: 'Success', detail: 'Theme deleted', life: 3000 })
        await loadThemes()
      } catch (err) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete theme', life: 3000 })
        console.error(err)
      }
    }
  })
}

function confirmDeleteEpic(theme: Theme, epic: Epic) {
  confirm.require({
    message: `Are you sure you want to delete epic "${epic.name}"? This will also delete all stories and spikes under this epic.`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await userStoryMapService.deleteEpic(projectId.value, theme.id!, epic.id!)
        toast.add({ severity: 'success', summary: 'Success', detail: 'Epic deleted', life: 3000 })
        await loadThemes()
      } catch (err) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete epic', life: 3000 })
        console.error(err)
      }
    }
  })
}

function confirmDeleteStory(epic: Epic, story: Story) {
  confirm.require({
    message: `Are you sure you want to delete story "${story.title}"?`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await userStoryMapService.deleteStory(projectId.value, epic.id!, story.id!)
        toast.add({ severity: 'success', summary: 'Success', detail: 'Story deleted', life: 3000 })
        await loadThemes()
      } catch (err) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete story', life: 3000 })
        console.error(err)
      }
    }
  })
}

function confirmDeleteSpike(epic: Epic, spike: Spike) {
  confirm.require({
    message: `Are you sure you want to delete spike "${spike.title}"?`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await userStoryMapService.deleteSpike(projectId.value, epic.id!, spike.id!)
        toast.add({ severity: 'success', summary: 'Success', detail: 'Spike deleted', life: 3000 })
        await loadThemes()
      } catch (err) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete spike', life: 3000 })
        console.error(err)
      }
    }
  })
}

function confirmDeleteSprint(sprint: Sprint) {
  confirm.require({
    message: `Are you sure you want to delete sprint "${sprint.name}"?`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await userStoryMapService.deleteSprint(projectId.value, sprint.id!)
        toast.add({ severity: 'success', summary: 'Success', detail: 'Sprint deleted', life: 3000 })
        await loadSprints()
      } catch (err) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete sprint', life: 3000 })
        console.error(err)
      }
    }
  })
}
</script>

<style scoped>
.user-story-map-view {
  padding: 2rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header h1 {
  margin: 0;
}

.actions {
  display: flex;
  gap: 1rem;
}

.loading {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.error {
  padding: 2rem;
}

.story-map {
  overflow-x: auto;
}

.themes-row {
  display: flex;
  gap: 1.5rem;
  min-height: 500px;
}

.theme-card {
  min-width: 350px;
  max-width: 350px;
}

.theme-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
}

.theme-header h3 {
  margin: 0;
  font-size: 1.25rem;
}

.theme-actions {
  display: flex;
  gap: 0.25rem;
}

.theme-description {
  margin-bottom: 1rem;
  color: var(--text-color-secondary);
}

.epics-column {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-top: 1rem;
}

.epic-card {
  width: 100%;
}

.epic-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
}

.epic-header h4 {
  margin: 0;
  font-size: 1rem;
}

.epic-actions {
  display: flex;
  gap: 0.25rem;
}

.epic-description {
  margin-bottom: 0.75rem;
  color: var(--text-color-secondary);
  font-size: 0.9rem;
}

.work-items {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 1rem;
}

.work-item {
  padding: 0.75rem;
  border: 1px solid var(--surface-border);
  border-radius: 6px;
  background: var(--surface-ground);
}

.story-item {
  border-left: 3px solid var(--primary-color);
}

.spike-item {
  border-left: 3px solid var(--yellow-500);
}

.work-item-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.work-item-header i {
  font-size: 0.9rem;
}

.work-item-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.work-item-actions {
  display: flex;
  gap: 0.25rem;
  justify-content: flex-end;
}

.empty-state {
  text-align: center;
  padding: 3rem;
}

.form-field {
  margin-bottom: 1.5rem;
}

.form-field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.w-full {
  width: 100%;
}

.mb-3 {
  margin-bottom: 1rem;
}

.sprints-section {
  padding: 1rem 0;
}
</style>
