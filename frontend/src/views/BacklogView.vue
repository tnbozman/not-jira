<template>
  <div class="backlog-view">
    <div class="header">
      <h1>Product Backlog</h1>
      <div class="actions">
        <Button label="New Sprint" icon="pi pi-plus" @click="showNewSprintDialog = true" />
        <Button label="Manage Teams" icon="pi pi-users" outlined @click="showTeamsDialog = true" />
        <Button label="Manage Releases" icon="pi pi-bookmark" outlined @click="showReleasesDialog = true" />
      </div>
    </div>

    <!-- Filters -->
    <div class="filters">
      <div class="filter-group">
        <label>Team</label>
        <Dropdown v-model="filters.teamId" :options="teams" optionLabel="name" optionValue="id" 
                  placeholder="All Teams" showClear @change="loadBacklog" />
      </div>
      <div class="filter-group">
        <label>Assignee</label>
        <Dropdown v-model="filters.assigneeId" :options="projectMembers" optionLabel="userName" optionValue="userId" 
                  placeholder="All Assignees" showClear @change="loadBacklog" />
      </div>
      <div class="filter-group">
        <label>Release</label>
        <Dropdown v-model="filters.releaseId" :options="releases" optionLabel="name" optionValue="id" 
                  placeholder="All Releases" showClear @change="loadBacklog" />
      </div>
      <div class="filter-group">
        <label>Epic</label>
        <Dropdown v-model="filters.epicId" :options="epics" optionLabel="name" optionValue="id" 
                  placeholder="All Epics" showClear @change="loadBacklog" />
      </div>
    </div>

    <div v-if="loading" class="loading">
      <ProgressSpinner />
    </div>

    <div v-else-if="error" class="error">
      <Message severity="error" :closable="false">{{ error }}</Message>
    </div>

    <div v-else class="backlog-content">
      <!-- Sprints (Oldest to Newest) -->
      <div v-for="sprintGroup in backlog.sprints" :key="sprintGroup.sprintId" class="sprint-section">
        <Card>
          <template #header>
            <div class="sprint-header">
              <div class="sprint-info">
                <h3>{{ sprintGroup.sprintName }}</h3>
                <Tag :value="sprintGroup.status" :severity="getStatusSeverity(sprintGroup.status)" />
                <span class="sprint-dates">
                  {{ formatDate(sprintGroup.startDate) }} - {{ formatDate(sprintGroup.endDate) }}
                </span>
              </div>
              <div class="sprint-actions">
                <Button label="Planning" icon="pi pi-calendar-plus" text @click="openPlanning(sprintGroup)" />
                <Button label="Review" icon="pi pi-check-circle" text @click="openReview(sprintGroup)" />
                <Button label="Retro" icon="pi pi-comments" text @click="openRetro(sprintGroup)" />
              </div>
            </div>
          </template>
          <template #content>
            <p v-if="sprintGroup.sprintGoal" class="sprint-goal">
              <strong>Goal:</strong> {{ sprintGroup.sprintGoal }}
            </p>
            
            <DataTable :value="sprintGroup.items" class="backlog-table">
              <Column field="type" header="Type" style="width: 80px">
                <template #body="slotProps">
                  <Tag :value="slotProps.data.type" 
                       :severity="slotProps.data.type === 'Story' ? 'info' : 'warning'" />
                </template>
              </Column>
              <Column field="title" header="Title">
                <template #body="slotProps">
                  <div class="item-title">
                    <i :class="slotProps.data.type === 'Story' ? 'pi pi-bookmark' : 'pi pi-bolt'"></i>
                    {{ slotProps.data.title }}
                  </div>
                </template>
              </Column>
              <Column field="epicName" header="Epic" style="width: 150px" />
              <Column field="teamName" header="Team" style="width: 120px" />
              <Column field="assigneeName" header="Assignee" style="width: 120px" />
              <Column field="releaseName" header="Release" style="width: 120px" />
              <Column field="priority" header="Priority" style="width: 100px">
                <template #body="slotProps">
                  <Tag :value="slotProps.data.priority" :severity="getPrioritySeverity(slotProps.data.priority)" />
                </template>
              </Column>
              <Column field="storyPoints" header="Points" style="width: 80px">
                <template #body="slotProps">
                  <Tag v-if="slotProps.data.storyPoints" :value="slotProps.data.storyPoints" />
                </template>
              </Column>
              <Column field="status" header="Status" style="width: 100px">
                <template #body="slotProps">
                  <Tag :value="slotProps.data.status" />
                </template>
              </Column>
            </DataTable>
          </template>
        </Card>
      </div>

      <!-- Backlog (Unassigned Items) -->
      <div class="backlog-section">
        <Card>
          <template #header>
            <div class="backlog-header">
              <h3>Backlog (Not in Sprint)</h3>
              <span class="item-count">{{ backlog.backlogItems.length }} items</span>
            </div>
          </template>
          <template #content>
            <DataTable :value="backlog.backlogItems" class="backlog-table">
              <Column field="type" header="Type" style="width: 80px">
                <template #body="slotProps">
                  <Tag :value="slotProps.data.type" 
                       :severity="slotProps.data.type === 'Story' ? 'info' : 'warning'" />
                </template>
              </Column>
              <Column field="title" header="Title">
                <template #body="slotProps">
                  <div class="item-title">
                    <i :class="slotProps.data.type === 'Story' ? 'pi pi-bookmark' : 'pi pi-bolt'"></i>
                    {{ slotProps.data.title }}
                  </div>
                </template>
              </Column>
              <Column field="epicName" header="Epic" style="width: 150px" />
              <Column field="teamName" header="Team" style="width: 120px" />
              <Column field="assigneeName" header="Assignee" style="width: 120px" />
              <Column field="releaseName" header="Release" style="width: 120px" />
              <Column field="priority" header="Priority" style="width: 100px">
                <template #body="slotProps">
                  <Tag :value="slotProps.data.priority" :severity="getPrioritySeverity(slotProps.data.priority)" />
                </template>
              </Column>
              <Column field="storyPoints" header="Points" style="width: 80px">
                <template #body="slotProps">
                  <Tag v-if="slotProps.data.storyPoints" :value="slotProps.data.storyPoints" />
                </template>
              </Column>
              <Column field="status" header="Status" style="width: 100px">
                <template #body="slotProps">
                  <Tag :value="slotProps.data.status" />
                </template>
              </Column>
            </DataTable>
          </template>
        </Card>
      </div>
    </div>

    <!-- Sprint Planning Dialog -->
    <Dialog v-model:visible="showPlanningDialog" header="Sprint Planning" :modal="true" :style="{ width: '800px' }">
      <div v-if="selectedSprint" class="planning-dialog">
        <h4>{{ selectedSprint.sprintName }}</h4>
        
        <div class="planning-section">
          <h5>Planning 1 (Overall)</h5>
          <Textarea v-model="planningData.planningOneNotes" rows="5" style="width: 100%" 
                    placeholder="Notes from overall planning session..." />
        </div>

        <div class="planning-section">
          <h5>Planning 2 (Per Team)</h5>
          <div v-for="team in teams" :key="team.id" class="team-planning">
            <label>{{ team.name }}</label>
            <Textarea v-model="getTeamPlanning(team.id).planningTwoNotes" rows="3" style="width: 100%" 
                      :placeholder="`Planning notes for ${team.name}...`" />
          </div>
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="showPlanningDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="savePlanning" />
      </template>
    </Dialog>

    <!-- Sprint Review Dialog -->
    <Dialog v-model:visible="showReviewDialog" header="Sprint Review" :modal="true" :style="{ width: '600px' }">
      <div v-if="selectedSprint" class="review-dialog">
        <h4>{{ selectedSprint.sprintName }}</h4>
        <p class="dialog-description">Combined review session for all teams</p>
        <Textarea v-model="reviewNotes" rows="10" style="width: 100%" 
                  placeholder="Notes from combined sprint review..." />
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="showReviewDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="saveReview" />
      </template>
    </Dialog>

    <!-- Sprint Retro Dialog -->
    <Dialog v-model:visible="showRetroDialog" header="Sprint Retrospective" :modal="true" :style="{ width: '600px' }">
      <div v-if="selectedSprint" class="retro-dialog">
        <h4>{{ selectedSprint.sprintName }}</h4>
        <p class="dialog-description">Combined retrospective for all teams</p>
        <Textarea v-model="retroNotes" rows="10" style="width: 100%" 
                  placeholder="Notes from combined sprint retrospective..." />
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="showRetroDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="saveRetro" />
      </template>
    </Dialog>

    <!-- New Sprint Dialog -->
    <Dialog v-model:visible="showNewSprintDialog" header="Create Sprint" :modal="true" :style="{ width: '500px' }">
      <div class="sprint-form">
        <div class="form-field">
          <label for="sprintName">Name</label>
          <InputText id="sprintName" v-model="newSprint.name" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="sprintGoal">Goal</label>
          <Textarea id="sprintGoal" v-model="newSprint.goal" rows="3" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="startDate">Start Date</label>
          <DatePicker id="startDate" v-model="newSprint.startDate" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="endDate">End Date</label>
          <DatePicker id="endDate" v-model="newSprint.endDate" style="width: 100%" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="showNewSprintDialog = false" />
        <Button label="Create" icon="pi pi-check" @click="createSprint" />
      </template>
    </Dialog>

    <!-- Teams Management Dialog -->
    <Dialog v-model:visible="showTeamsDialog" header="Manage Teams" :modal="true" :style="{ width: '600px' }">
      <div class="teams-management">
        <Button label="Add Team" icon="pi pi-plus" @click="showAddTeamDialog = true" class="mb-3" />
        <DataTable :value="teams">
          <Column field="name" header="Name" />
          <Column field="description" header="Description" />
          <Column header="Actions" style="width: 100px">
            <template #body="slotProps">
              <Button icon="pi pi-trash" text severity="danger" @click="deleteTeam(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </Dialog>

    <!-- Add Team Dialog -->
    <Dialog v-model:visible="showAddTeamDialog" header="Add Team" :modal="true" :style="{ width: '400px' }">
      <div class="team-form">
        <div class="form-field">
          <label for="teamName">Name</label>
          <InputText id="teamName" v-model="newTeam.name" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="teamDescription">Description</label>
          <Textarea id="teamDescription" v-model="newTeam.description" rows="3" style="width: 100%" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="showAddTeamDialog = false" />
        <Button label="Add" icon="pi pi-check" @click="addTeam" />
      </template>
    </Dialog>

    <!-- Releases Management Dialog -->
    <Dialog v-model:visible="showReleasesDialog" header="Manage Releases" :modal="true" :style="{ width: '700px' }">
      <div class="releases-management">
        <Button label="Add Release" icon="pi pi-plus" @click="showAddReleaseDialog = true" class="mb-3" />
        <DataTable :value="releases">
          <Column field="name" header="Name" />
          <Column field="status" header="Status">
            <template #body="slotProps">
              <Tag :value="slotProps.data.status" />
            </template>
          </Column>
          <Column field="releaseDate" header="Release Date">
            <template #body="slotProps">
              {{ slotProps.data.releaseDate ? formatDate(slotProps.data.releaseDate) : 'Not set' }}
            </template>
          </Column>
          <Column header="Actions" style="width: 100px">
            <template #body="slotProps">
              <Button icon="pi pi-trash" text severity="danger" @click="deleteRelease(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </Dialog>

    <!-- Add Release Dialog -->
    <Dialog v-model:visible="showAddReleaseDialog" header="Add Release" :modal="true" :style="{ width: '500px' }">
      <div class="release-form">
        <div class="form-field">
          <label for="releaseName">Name</label>
          <InputText id="releaseName" v-model="newRelease.name" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="releaseDescription">Description</label>
          <Textarea id="releaseDescription" v-model="newRelease.description" rows="3" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="releaseStartDate">Start Date</label>
          <DatePicker id="releaseStartDate" v-model="newRelease.startDate" style="width: 100%" />
        </div>
        <div class="form-field">
          <label for="releaseReleaseDate">Release Date</label>
          <DatePicker id="releaseReleaseDate" v-model="newRelease.releaseDate" style="width: 100%" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" text @click="showAddReleaseDialog = false" />
        <Button label="Add" icon="pi pi-check" @click="addRelease" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import Button from 'primevue/button'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import Dropdown from 'primevue/dropdown'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Tag from 'primevue/tag'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
import DatePicker from 'primevue/datepicker'

const route = useRoute()
const projectId = computed(() => route.params.id)

const loading = ref(false)
const error = ref('')

const backlog = ref<any>({ sprints: [], backlogItems: [] })
const teams = ref<any[]>([])
const releases = ref<any[]>([])
const epics = ref<any[]>([])
const projectMembers = ref<any[]>([])

const filters = ref<{
  teamId: number | null
  assigneeId: string | null
  releaseId: number | null
  epicId: number | null
}>({
  teamId: null,
  assigneeId: null,
  releaseId: null,
  epicId: null
})

const showPlanningDialog = ref(false)
const showReviewDialog = ref(false)
const showRetroDialog = ref(false)
const showNewSprintDialog = ref(false)
const showTeamsDialog = ref(false)
const showReleasesDialog = ref(false)
const showAddTeamDialog = ref(false)
const showAddReleaseDialog = ref(false)

const selectedSprint = ref<any>(null)
const planningData = ref<any>({ planningOneNotes: '', teamPlannings: [] })
const reviewNotes = ref('')
const retroNotes = ref('')

const newSprint = ref({
  name: '',
  goal: '',
  startDate: null,
  endDate: null,
  status: 'Planned'
})

const newTeam = ref({
  name: '',
  description: ''
})

const newRelease = ref({
  name: '',
  description: '',
  startDate: null,
  releaseDate: null,
  status: 'Planned'
})

onMounted(async () => {
  await Promise.all([
    loadBacklog(),
    loadTeams(),
    loadReleases(),
    loadEpics(),
    loadProjectMembers()
  ])
})

async function loadBacklog() {
  loading.value = true
  error.value = ''
  try {
    const params = new URLSearchParams()
    if (filters.value.teamId) params.append('teamId', filters.value.teamId.toString())
    if (filters.value.assigneeId) params.append('assigneeId', filters.value.assigneeId)
    if (filters.value.releaseId) params.append('releaseId', filters.value.releaseId.toString())
    if (filters.value.epicId) params.append('epicId', filters.value.epicId.toString())

    const response = await axios.get(`/api/projects/${projectId.value}/backlog?${params}`)
    backlog.value = response.data
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Failed to load backlog'
  } finally {
    loading.value = false
  }
}

async function loadTeams() {
  try {
    const response = await axios.get(`/api/projects/${projectId.value}/teams`)
    teams.value = response.data
  } catch (err) {
    console.error('Failed to load teams:', err)
  }
}

async function loadReleases() {
  try {
    const response = await axios.get(`/api/projects/${projectId.value}/releases`)
    releases.value = response.data
  } catch (err) {
    console.error('Failed to load releases:', err)
  }
}

async function loadEpics() {
  try {
    // Load all themes with epics
    const response = await axios.get(`/api/projects/${projectId.value}/themes`)
    const themes = response.data
    epics.value = themes.flatMap((theme: any) => theme.epics || [])
  } catch (err) {
    console.error('Failed to load epics:', err)
  }
}

async function loadProjectMembers() {
  try {
    const response = await axios.get(`/api/projects/${projectId.value}`)
    projectMembers.value = response.data.members || []
  } catch (err) {
    console.error('Failed to load project members:', err)
  }
}

function openPlanning(sprint: any) {
  selectedSprint.value = sprint
  // Load existing planning data
  axios.get(`/api/projects/${projectId.value}/sprints/${sprint.sprintId}/planning`)
    .then(response => {
      planningData.value = {
        planningOneNotes: response.data.planningOneNotes || '',
        teamPlannings: response.data.teamPlannings || []
      }
    })
    .catch(err => console.error('Failed to load planning data:', err))
  showPlanningDialog.value = true
}

function getTeamPlanning(teamId: number) {
  let tp = planningData.value.teamPlannings.find((t: any) => t.teamId === teamId)
  if (!tp) {
    tp = { teamId, planningTwoNotes: '' }
    planningData.value.teamPlannings.push(tp)
  }
  return tp
}

async function savePlanning() {
  try {
    await axios.put(
      `/api/projects/${projectId.value}/sprints/${selectedSprint.value.sprintId}/planning`,
      planningData.value
    )
    showPlanningDialog.value = false
    await loadBacklog()
  } catch (err) {
    console.error('Failed to save planning:', err)
  }
}

function openReview(sprint: any) {
  selectedSprint.value = sprint
  // Load existing review notes
  axios.get(`/api/projects/${projectId.value}/sprints/${sprint.sprintId}`)
    .then(response => {
      reviewNotes.value = response.data.reviewNotes || ''
    })
    .catch(err => console.error('Failed to load review notes:', err))
  showReviewDialog.value = true
}

async function saveReview() {
  try {
    await axios.put(
      `/api/projects/${projectId.value}/sprints/${selectedSprint.value.sprintId}/review`,
      { notes: reviewNotes.value }
    )
    showReviewDialog.value = false
    await loadBacklog()
  } catch (err) {
    console.error('Failed to save review:', err)
  }
}

function openRetro(sprint: any) {
  selectedSprint.value = sprint
  // Load existing retro notes
  axios.get(`/api/projects/${projectId.value}/sprints/${sprint.sprintId}`)
    .then(response => {
      retroNotes.value = response.data.retroNotes || ''
    })
    .catch(err => console.error('Failed to load retro notes:', err))
  showRetroDialog.value = true
}

async function saveRetro() {
  try {
    await axios.put(
      `/api/projects/${projectId.value}/sprints/${selectedSprint.value.sprintId}/retro`,
      { notes: retroNotes.value }
    )
    showRetroDialog.value = false
    await loadBacklog()
  } catch (err) {
    console.error('Failed to save retro:', err)
  }
}

async function createSprint() {
  try {
    await axios.post(`/api/projects/${projectId.value}/sprints`, newSprint.value)
    showNewSprintDialog.value = false
    newSprint.value = { name: '', goal: '', startDate: null, endDate: null, status: 'Planned' }
    await loadBacklog()
  } catch (err) {
    console.error('Failed to create sprint:', err)
  }
}

async function addTeam() {
  try {
    await axios.post(`/api/projects/${projectId.value}/teams`, newTeam.value)
    showAddTeamDialog.value = false
    newTeam.value = { name: '', description: '' }
    await loadTeams()
  } catch (err) {
    console.error('Failed to add team:', err)
  }
}

async function deleteTeam(team: any) {
  if (confirm(`Delete team "${team.name}"?`)) {
    try {
      await axios.delete(`/api/projects/${projectId.value}/teams/${team.id}`)
      await loadTeams()
    } catch (err) {
      console.error('Failed to delete team:', err)
    }
  }
}

async function addRelease() {
  try {
    await axios.post(`/api/projects/${projectId.value}/releases`, newRelease.value)
    showAddReleaseDialog.value = false
    newRelease.value = { name: '', description: '', startDate: null, releaseDate: null, status: 'Planned' }
    await loadReleases()
  } catch (err) {
    console.error('Failed to add release:', err)
  }
}

async function deleteRelease(release: any) {
  if (confirm(`Delete release "${release.name}"?`)) {
    try {
      await axios.delete(`/api/projects/${projectId.value}/releases/${release.id}`)
      await loadReleases()
    } catch (err) {
      console.error('Failed to delete release:', err)
    }
  }
}

function formatDate(date: string) {
  return new Date(date).toLocaleDateString()
}

function getStatusSeverity(status: string) {
  const severities: any = {
    'Planned': 'info',
    'Active': 'success',
    'Completed': 'secondary'
  }
  return severities[status] || 'info'
}

function getPrioritySeverity(priority: string) {
  const severities: any = {
    'Critical': 'danger',
    'High': 'warning',
    'Medium': 'info',
    'Low': 'secondary'
  }
  return severities[priority] || 'info'
}
</script>

<style scoped>
.backlog-view {
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
  font-size: 2rem;
}

.actions {
  display: flex;
  gap: 1rem;
}

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  background: var(--surface-card);
  padding: 1.5rem;
  border-radius: 6px;
}

.filter-group {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-group label {
  font-weight: 600;
  font-size: 0.875rem;
}

.loading, .error {
  display: flex;
  justify-content: center;
  padding: 3rem;
}

.backlog-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.sprint-section, .backlog-section {
  width: 100%;
}

.sprint-header, .backlog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
}

.sprint-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.sprint-info h3 {
  margin: 0;
}

.sprint-dates {
  color: var(--text-color-secondary);
  font-size: 0.875rem;
}

.sprint-actions {
  display: flex;
  gap: 0.5rem;
}

.sprint-goal {
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: var(--surface-ground);
  border-radius: 4px;
}

.backlog-header h3 {
  margin: 0;
}

.item-count {
  color: var(--text-color-secondary);
  font-size: 0.875rem;
}

.backlog-table {
  margin-top: 1rem;
}

.item-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.planning-dialog, .review-dialog, .retro-dialog {
  padding: 1rem 0;
}

.planning-section, .team-planning {
  margin-bottom: 1.5rem;
}

.planning-section h5 {
  margin: 0 0 1rem 0;
}

.team-planning {
  margin-bottom: 1rem;
}

.team-planning label {
  display: block;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.dialog-description {
  color: var(--text-color-secondary);
  margin-bottom: 1rem;
}

.sprint-form, .team-form, .release-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-field label {
  font-weight: 600;
}

.teams-management, .releases-management {
  padding: 1rem 0;
}

.mb-3 {
  margin-bottom: 1rem;
}
</style>
