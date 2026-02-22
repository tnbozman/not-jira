<template>
  <div class="backlog-page">
    <div class="backlog-top">
      <h1 class="backlog-title">Product Backlog</h1>
      <div class="backlog-actions">
        <Button label="New Sprint" icon="pi pi-plus" @click="showNewSprintDialog = true" />
        <Button label="Manage Teams" icon="pi pi-users" outlined @click="showTeamsDialog = true" />
        <Button
          label="Manage Releases"
          icon="pi pi-bookmark"
          outlined
          @click="showReleasesDialog = true"
        />
      </div>
    </div>

    <!-- Filters -->
    <div class="filters-bar">
      <div class="filter-group">
        <label class="filter-label">Team</label>
        <Dropdown
          v-model="filters.teamId"
          :options="teams"
          optionLabel="name"
          optionValue="id"
          placeholder="All Teams"
          showClear
          @change="loadBacklog"
        />
      </div>
      <div class="filter-group">
        <label class="filter-label">Assignee</label>
        <Dropdown
          v-model="filters.assigneeId"
          :options="projectMembers"
          optionLabel="userName"
          optionValue="userId"
          placeholder="All Assignees"
          showClear
          @change="loadBacklog"
        />
      </div>
      <div class="filter-group">
        <label class="filter-label">Release</label>
        <Dropdown
          v-model="filters.releaseId"
          :options="releases"
          optionLabel="name"
          optionValue="id"
          placeholder="All Releases"
          showClear
          @change="loadBacklog"
        />
      </div>
      <div class="filter-group">
        <label class="filter-label">Epic</label>
        <Dropdown
          v-model="filters.epicId"
          :options="epics"
          optionLabel="name"
          optionValue="id"
          placeholder="All Epics"
          showClear
          @change="loadBacklog"
        />
      </div>
    </div>

    <div v-if="loading" class="loading-state">
      <ProgressSpinner />
    </div>

    <div v-else-if="error" class="error-state">
      <Message severity="error" :closable="false">{{ error }}</Message>
    </div>

    <div v-else class="backlog-content">
      <!-- Sprints -->
      <div
        v-for="sprintGroup in backlog.sprints"
        :key="sprintGroup.sprintId"
        class="sprint-section"
      >
        <div class="section-card">
          <div class="sprint-header">
            <div class="sprint-info">
              <h3 class="sprint-name">{{ sprintGroup.sprintName }}</h3>
              <Tag :value="sprintGroup.status" :severity="getStatusSeverity(sprintGroup.status)" />
              <span class="sprint-dates">
                {{ formatDate(sprintGroup.startDate) }} - {{ formatDate(sprintGroup.endDate) }}
              </span>
            </div>
            <div class="sprint-btns">
              <Button
                label="Planning"
                icon="pi pi-calendar-plus"
                text
                size="small"
                @click="openPlanning(sprintGroup)"
              />
              <Button
                label="Review"
                icon="pi pi-check-circle"
                text
                size="small"
                @click="openReview(sprintGroup)"
              />
              <Button
                label="Retro"
                icon="pi pi-comments"
                text
                size="small"
                @click="openRetro(sprintGroup)"
              />
            </div>
          </div>

          <p v-if="sprintGroup.sprintGoal" class="sprint-goal">
            <strong>Goal:</strong> {{ sprintGroup.sprintGoal }}
          </p>

          <DataTable :value="sprintGroup.items">
            <Column field="type" header="Type" style="width: 80px">
              <template #body="slotProps">
                <Tag
                  :value="slotProps.data.type"
                  :severity="slotProps.data.type === 'Story' ? 'info' : 'warning'"
                />
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
                <Tag
                  :value="slotProps.data.priority"
                  :severity="getPrioritySeverity(slotProps.data.priority)"
                />
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
        </div>
      </div>

      <!-- Backlog (Unassigned Items) -->
      <div class="backlog-section">
        <div class="section-card">
          <div class="backlog-header">
            <h3 class="section-heading">Backlog (Not in Sprint)</h3>
            <span class="item-count">{{ backlog.backlogItems.length }} items</span>
          </div>

          <DataTable :value="backlog.backlogItems">
            <Column field="type" header="Type" style="width: 80px">
              <template #body="slotProps">
                <Tag
                  :value="slotProps.data.type"
                  :severity="slotProps.data.type === 'Story' ? 'info' : 'warning'"
                />
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
                <Tag
                  :value="slotProps.data.priority"
                  :severity="getPrioritySeverity(slotProps.data.priority)"
                />
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
        </div>
      </div>
    </div>

    <!-- Sprint Planning Dialog -->
    <Dialog
      v-model:visible="showPlanningDialog"
      header="Sprint Planning"
      :modal="true"
      :style="{ width: '48rem' }"
    >
      <div v-if="selectedSprint" class="dialog-body">
        <h4 class="dialog-subtitle">{{ selectedSprint.sprintName }}</h4>

        <div class="planning-section">
          <h5 class="planning-heading">Planning 1 (Overall)</h5>
          <Textarea
            v-model="planningData.planningOneNotes"
            rows="5"
            class="w-full"
            placeholder="Notes from overall planning session..."
          />
        </div>

        <div class="planning-section">
          <h5 class="planning-heading">Planning 2 (Per Team)</h5>
          <div v-for="team in teams" :key="team.id" class="team-planning">
            <label class="field-label">{{ team.name }}</label>
            <Textarea
              v-model="getTeamPlanning(team.id).planningTwoNotes"
              rows="3"
              class="w-full"
              :placeholder="`Planning notes for ${team.name}...`"
            />
          </div>
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showPlanningDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="savePlanning" />
      </template>
    </Dialog>

    <!-- Sprint Review Dialog -->
    <Dialog
      v-model:visible="showReviewDialog"
      header="Sprint Review"
      :modal="true"
      :style="{ width: '36rem' }"
    >
      <div v-if="selectedSprint" class="dialog-body">
        <h4 class="dialog-subtitle">{{ selectedSprint.sprintName }}</h4>
        <p class="dialog-desc">Combined review session for all teams</p>
        <Textarea
          v-model="reviewNotes"
          rows="10"
          class="w-full"
          placeholder="Notes from combined sprint review..."
        />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showReviewDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="saveReview" />
      </template>
    </Dialog>

    <!-- Sprint Retro Dialog -->
    <Dialog
      v-model:visible="showRetroDialog"
      header="Sprint Retrospective"
      :modal="true"
      :style="{ width: '36rem' }"
    >
      <div v-if="selectedSprint" class="dialog-body">
        <h4 class="dialog-subtitle">{{ selectedSprint.sprintName }}</h4>
        <p class="dialog-desc">Combined retrospective for all teams</p>
        <Textarea
          v-model="retroNotes"
          rows="10"
          class="w-full"
          placeholder="Notes from combined sprint retrospective..."
        />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showRetroDialog = false" />
        <Button label="Save" icon="pi pi-check" @click="saveRetro" />
      </template>
    </Dialog>

    <!-- New Sprint Dialog -->
    <Dialog
      v-model:visible="showNewSprintDialog"
      header="Create Sprint"
      :modal="true"
      :style="{ width: '30rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="sprintName" class="field-label">Name</label>
          <InputText id="sprintName" v-model="newSprint.name" class="w-full" />
        </div>
        <div class="form-field">
          <label for="sprintGoal" class="field-label">Goal</label>
          <Textarea id="sprintGoal" v-model="newSprint.goal" rows="3" class="w-full" />
        </div>
        <div class="form-field">
          <label for="startDate" class="field-label">Start Date</label>
          <DatePicker id="startDate" v-model="newSprint.startDate" class="w-full" />
        </div>
        <div class="form-field">
          <label for="endDate" class="field-label">End Date</label>
          <DatePicker id="endDate" v-model="newSprint.endDate" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showNewSprintDialog = false" />
        <Button label="Create" icon="pi pi-check" @click="createSprint" />
      </template>
    </Dialog>

    <!-- Teams Management Dialog -->
    <Dialog
      v-model:visible="showTeamsDialog"
      header="Manage Teams"
      :modal="true"
      :style="{ width: '36rem' }"
    >
      <div class="dialog-body">
        <Button
          label="Add Team"
          icon="pi pi-plus"
          @click="showAddTeamDialog = true"
          style="margin-bottom: 1rem"
        />
        <DataTable :value="teams">
          <Column field="name" header="Name" />
          <Column field="description" header="Description" />
          <Column header="Actions" style="width: 100px">
            <template #body="slotProps">
              <Button
                icon="pi pi-trash"
                text
                severity="danger"
                size="small"
                @click="deleteTeam(slotProps.data)"
              />
            </template>
          </Column>
        </DataTable>
      </div>
    </Dialog>

    <!-- Add Team Dialog -->
    <Dialog
      v-model:visible="showAddTeamDialog"
      header="Add Team"
      :modal="true"
      :style="{ width: '26rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="teamName" class="field-label">Name</label>
          <InputText id="teamName" v-model="newTeam.name" class="w-full" />
        </div>
        <div class="form-field">
          <label for="teamDescription" class="field-label">Description</label>
          <Textarea id="teamDescription" v-model="newTeam.description" rows="3" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showAddTeamDialog = false" />
        <Button label="Add" icon="pi pi-check" @click="addTeam" />
      </template>
    </Dialog>

    <!-- Releases Management Dialog -->
    <Dialog
      v-model:visible="showReleasesDialog"
      header="Manage Releases"
      :modal="true"
      :style="{ width: '42rem' }"
    >
      <div class="dialog-body">
        <Button
          label="Add Release"
          icon="pi pi-plus"
          @click="showAddReleaseDialog = true"
          style="margin-bottom: 1rem"
        />
        <DataTable :value="releases">
          <Column field="name" header="Name" />
          <Column field="status" header="Status">
            <template #body="slotProps">
              <Tag :value="slotProps.data.status" />
            </template>
          </Column>
          <Column field="releaseDate" header="Release Date">
            <template #body="slotProps">
              {{ slotProps.data.releaseDate ? formatDate(slotProps.data.releaseDate) : "Not set" }}
            </template>
          </Column>
          <Column header="Actions" style="width: 100px">
            <template #body="slotProps">
              <Button
                icon="pi pi-trash"
                text
                severity="danger"
                size="small"
                @click="deleteRelease(slotProps.data)"
              />
            </template>
          </Column>
        </DataTable>
      </div>
    </Dialog>

    <!-- Add Release Dialog -->
    <Dialog
      v-model:visible="showAddReleaseDialog"
      header="Add Release"
      :modal="true"
      :style="{ width: '30rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="releaseName" class="field-label">Name</label>
          <InputText id="releaseName" v-model="newRelease.name" class="w-full" />
        </div>
        <div class="form-field">
          <label for="releaseDescription" class="field-label">Description</label>
          <Textarea
            id="releaseDescription"
            v-model="newRelease.description"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="releaseStartDate" class="field-label">Start Date</label>
          <DatePicker id="releaseStartDate" v-model="newRelease.startDate" class="w-full" />
        </div>
        <div class="form-field">
          <label for="releaseReleaseDate" class="field-label">Release Date</label>
          <DatePicker id="releaseReleaseDate" v-model="newRelease.releaseDate" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showAddReleaseDialog = false" />
        <Button label="Add" icon="pi pi-check" @click="addRelease" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRoute } from "vue-router";
import axios from "axios";
import Button from "primevue/button";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Dialog from "primevue/dialog";
import Dropdown from "primevue/dropdown";
import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";
import Tag from "primevue/tag";
import Message from "primevue/message";
import ProgressSpinner from "primevue/progressspinner";
import DatePicker from "primevue/datepicker";

const route = useRoute();
const projectId = computed(() => route.params.id);

const loading = ref(false);
const error = ref("");

const backlog = ref<any>({ sprints: [], backlogItems: [] });
const teams = ref<any[]>([]);
const releases = ref<any[]>([]);
const epics = ref<any[]>([]);
const projectMembers = ref<any[]>([]);

const filters = ref<{
  teamId: number | null;
  assigneeId: string | null;
  releaseId: number | null;
  epicId: number | null;
}>({
  teamId: null,
  assigneeId: null,
  releaseId: null,
  epicId: null,
});

const showPlanningDialog = ref(false);
const showReviewDialog = ref(false);
const showRetroDialog = ref(false);
const showNewSprintDialog = ref(false);
const showTeamsDialog = ref(false);
const showReleasesDialog = ref(false);
const showAddTeamDialog = ref(false);
const showAddReleaseDialog = ref(false);

const selectedSprint = ref<any>(null);
const planningData = ref<any>({ planningOneNotes: "", teamPlannings: [] });
const reviewNotes = ref("");
const retroNotes = ref("");

const newSprint = ref({
  name: "",
  goal: "",
  startDate: null,
  endDate: null,
  status: "Planned",
});

const newTeam = ref({
  name: "",
  description: "",
});

const newRelease = ref({
  name: "",
  description: "",
  startDate: null,
  releaseDate: null,
  status: "Planned",
});

onMounted(async () => {
  await Promise.all([
    loadBacklog(),
    loadTeams(),
    loadReleases(),
    loadEpics(),
    loadProjectMembers(),
  ]);
});

async function loadBacklog() {
  loading.value = true;
  error.value = "";
  try {
    const params = new URLSearchParams();
    if (filters.value.teamId) params.append("teamId", filters.value.teamId.toString());
    if (filters.value.assigneeId) params.append("assigneeId", filters.value.assigneeId);
    if (filters.value.releaseId) params.append("releaseId", filters.value.releaseId.toString());
    if (filters.value.epicId) params.append("epicId", filters.value.epicId.toString());

    const response = await axios.get(`/api/projects/${projectId.value}/backlog?${params}`);
    backlog.value = response.data;
  } catch (err: any) {
    error.value = err.response?.data?.message || "Failed to load backlog";
  } finally {
    loading.value = false;
  }
}

async function loadTeams() {
  try {
    const response = await axios.get(`/api/projects/${projectId.value}/teams`);
    teams.value = response.data;
  } catch (err) {
    console.error("Failed to load teams:", err);
  }
}

async function loadReleases() {
  try {
    const response = await axios.get(`/api/projects/${projectId.value}/releases`);
    releases.value = response.data;
  } catch (err) {
    console.error("Failed to load releases:", err);
  }
}

async function loadEpics() {
  try {
    // Load all themes with epics
    const response = await axios.get(`/api/projects/${projectId.value}/themes`);
    const themes = response.data;
    epics.value = themes.flatMap((theme: any) => theme.epics || []);
  } catch (err) {
    console.error("Failed to load epics:", err);
  }
}

async function loadProjectMembers() {
  try {
    const response = await axios.get(`/api/projects/${projectId.value}`);
    projectMembers.value = response.data.members || [];
  } catch (err) {
    console.error("Failed to load project members:", err);
  }
}

function openPlanning(sprint: any) {
  selectedSprint.value = sprint;
  // Load existing planning data
  axios
    .get(`/api/projects/${projectId.value}/sprints/${sprint.sprintId}/planning`)
    .then((response) => {
      planningData.value = {
        planningOneNotes: response.data.planningOneNotes || "",
        teamPlannings: response.data.teamPlannings || [],
      };
    })
    .catch((err) => console.error("Failed to load planning data:", err));
  showPlanningDialog.value = true;
}

function getTeamPlanning(teamId: number) {
  let tp = planningData.value.teamPlannings.find((t: any) => t.teamId === teamId);
  if (!tp) {
    tp = { teamId, planningTwoNotes: "" };
    planningData.value.teamPlannings.push(tp);
  }
  return tp;
}

async function savePlanning() {
  try {
    await axios.put(
      `/api/projects/${projectId.value}/sprints/${selectedSprint.value.sprintId}/planning`,
      planningData.value,
    );
    showPlanningDialog.value = false;
    await loadBacklog();
  } catch (err) {
    console.error("Failed to save planning:", err);
  }
}

function openReview(sprint: any) {
  selectedSprint.value = sprint;
  // Load existing review notes
  axios
    .get(`/api/projects/${projectId.value}/sprints/${sprint.sprintId}`)
    .then((response) => {
      reviewNotes.value = response.data.reviewNotes || "";
    })
    .catch((err) => console.error("Failed to load review notes:", err));
  showReviewDialog.value = true;
}

async function saveReview() {
  try {
    await axios.put(
      `/api/projects/${projectId.value}/sprints/${selectedSprint.value.sprintId}/review`,
      { notes: reviewNotes.value },
    );
    showReviewDialog.value = false;
    await loadBacklog();
  } catch (err) {
    console.error("Failed to save review:", err);
  }
}

function openRetro(sprint: any) {
  selectedSprint.value = sprint;
  // Load existing retro notes
  axios
    .get(`/api/projects/${projectId.value}/sprints/${sprint.sprintId}`)
    .then((response) => {
      retroNotes.value = response.data.retroNotes || "";
    })
    .catch((err) => console.error("Failed to load retro notes:", err));
  showRetroDialog.value = true;
}

async function saveRetro() {
  try {
    await axios.put(
      `/api/projects/${projectId.value}/sprints/${selectedSprint.value.sprintId}/retro`,
      { notes: retroNotes.value },
    );
    showRetroDialog.value = false;
    await loadBacklog();
  } catch (err) {
    console.error("Failed to save retro:", err);
  }
}

async function createSprint() {
  try {
    await axios.post(`/api/projects/${projectId.value}/sprints`, newSprint.value);
    showNewSprintDialog.value = false;
    newSprint.value = { name: "", goal: "", startDate: null, endDate: null, status: "Planned" };
    await loadBacklog();
  } catch (err) {
    console.error("Failed to create sprint:", err);
  }
}

async function addTeam() {
  try {
    await axios.post(`/api/projects/${projectId.value}/teams`, newTeam.value);
    showAddTeamDialog.value = false;
    newTeam.value = { name: "", description: "" };
    await loadTeams();
  } catch (err) {
    console.error("Failed to add team:", err);
  }
}

async function deleteTeam(team: any) {
  if (confirm(`Delete team "${team.name}"?`)) {
    try {
      await axios.delete(`/api/projects/${projectId.value}/teams/${team.id}`);
      await loadTeams();
    } catch (err) {
      console.error("Failed to delete team:", err);
    }
  }
}

async function addRelease() {
  try {
    await axios.post(`/api/projects/${projectId.value}/releases`, newRelease.value);
    showAddReleaseDialog.value = false;
    newRelease.value = {
      name: "",
      description: "",
      startDate: null,
      releaseDate: null,
      status: "Planned",
    };
    await loadReleases();
  } catch (err) {
    console.error("Failed to add release:", err);
  }
}

async function deleteRelease(release: any) {
  if (confirm(`Delete release "${release.name}"?`)) {
    try {
      await axios.delete(`/api/projects/${projectId.value}/releases/${release.id}`);
      await loadReleases();
    } catch (err) {
      console.error("Failed to delete release:", err);
    }
  }
}

function formatDate(date: string) {
  return new Date(date).toLocaleDateString();
}

function getStatusSeverity(status: string) {
  const severities: any = {
    Planned: "info",
    Active: "success",
    Completed: "secondary",
  };
  return severities[status] || "info";
}

function getPrioritySeverity(priority: string) {
  const severities: any = {
    Critical: "danger",
    High: "warning",
    Medium: "info",
    Low: "secondary",
  };
  return severities[priority] || "info";
}
</script>

<style scoped>
.backlog-page {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.backlog-top {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.backlog-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #0f172a;
}

.backlog-actions {
  display: flex;
  gap: 0.5rem;
}

/* Filters */
.filters-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  padding: 1.25rem;
}

.filter-group {
  flex: 1;
  min-width: 10rem;
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.filter-label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #334155;
}

.loading-state,
.error-state {
  display: flex;
  justify-content: center;
  padding: 3rem 0;
}

.backlog-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* Section cards (sprint / backlog) */
.section-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  padding: 1.25rem;
}

.sprint-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.sprint-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.sprint-name {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #0f172a;
}

.sprint-dates {
  font-size: 0.875rem;
  color: #64748b;
}

.sprint-btns {
  display: flex;
  gap: 0.25rem;
}

.sprint-goal {
  margin: 0 0 1rem;
  padding: 0.625rem 0.75rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  color: #334155;
}

.backlog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.section-heading {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #0f172a;
}

.item-count {
  font-size: 0.875rem;
  color: #64748b;
}

.item-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #1e293b;
}

/* Dialog styles */
.dialog-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.dialog-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.dialog-subtitle {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
}

.dialog-desc {
  margin: 0;
  font-size: 0.875rem;
  color: #64748b;
}

.planning-section {
  margin-bottom: 1rem;
}

.planning-heading {
  margin: 0 0 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1e293b;
}

.team-planning {
  margin-bottom: 0.75rem;
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
</style>
