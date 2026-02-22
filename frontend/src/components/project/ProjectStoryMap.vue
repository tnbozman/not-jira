<template>
  <div class="usm-panel">
    <div class="usm-header">
      <h2 class="panel-title">User Story Map</h2>
      <div class="usm-actions">
        <Button label="Add Theme" icon="pi pi-plus" size="small" @click="showThemeDialog = true" />
        <Button
          label="Manage Sprints"
          icon="pi pi-calendar"
          outlined
          size="small"
          @click="showSprintsDialog = true"
        />
      </div>
    </div>

    <div v-if="loading" class="loading-state">
      <ProgressSpinner />
    </div>

    <div v-else-if="error" class="error-state">
      <Message severity="error" :closable="false">{{ error }}</Message>
    </div>

    <div v-else-if="themes.length === 0" class="empty-state">
      <Message severity="info" :closable="false">
        No themes yet. Click "Add Theme" to create your first outcome-focused theme.
      </Message>
    </div>

    <div v-else class="story-map-wrapper">
      <div class="story-map-grid" :style="{ gridTemplateColumns: gridColumns }">
        <!-- ── Row 0: Theme headers ─────────────────────────── -->
        <!-- Top-left corner (blank cell above release labels) -->
        <div class="grid-corner"></div>

        <template v-for="theme in themes" :key="'th-' + theme.id">
          <div class="theme-header" :style="{ gridColumn: 'span ' + (theme.epics?.length || 1) }">
            <div class="theme-header-inner">
              <h3 class="theme-name">{{ theme.name }}</h3>
              <div class="card-btns">
                <Button icon="pi pi-pencil" text size="small" @click="editTheme(theme)" />
                <Button
                  icon="pi pi-plus"
                  text
                  size="small"
                  @click="showAddEpicDialog(theme)"
                  v-tooltip="'Add Epic'"
                />
                <Button
                  icon="pi pi-trash"
                  text
                  size="small"
                  severity="danger"
                  @click="confirmDeleteTheme(theme)"
                />
              </div>
            </div>
            <p v-if="theme.description" class="theme-desc">{{ theme.description }}</p>
          </div>
        </template>

        <!-- ── Row 1: Epic backbone ─────────────────────────── -->
        <!-- Left label -->
        <div class="row-label row-label--backbone">Backbone</div>

        <template v-for="theme in themes" :key="'te-' + theme.id">
          <div
            v-for="epic in theme.epics && theme.epics.length ? theme.epics : [null]"
            :key="'ep-' + (epic ? epic.id : 'empty-' + theme.id)"
            class="epic-cell"
          >
            <template v-if="epic">
              <div class="epic-card">
                <div class="epic-top">
                  <h4 class="epic-name">{{ epic.name }}</h4>
                  <div class="card-btns">
                    <Button icon="pi pi-pencil" text size="small" @click="editEpic(theme, epic)" />
                    <Button
                      icon="pi pi-plus"
                      text
                      size="small"
                      @click="showAddStoryDialog(epic)"
                      v-tooltip="'Add Story'"
                    />
                    <Button
                      icon="pi pi-trash"
                      text
                      size="small"
                      severity="danger"
                      @click="confirmDeleteEpic(theme, epic)"
                    />
                  </div>
                </div>
                <p v-if="epic.description" class="epic-desc">{{ epic.description }}</p>
              </div>
            </template>
            <template v-else>
              <div class="epic-card epic-card--empty">
                <p class="empty-hint">No epics</p>
              </div>
            </template>
          </div>
        </template>

        <!-- ── Rows 2+: Release swim-lanes ──────────────────── -->
        <template v-for="rl in releaseRows" :key="'rl-' + rl.id">
          <!-- Left release label -->
          <div class="row-label row-label--release">
            <span class="release-name">{{ rl.name }}</span>
            <Tag v-if="rl.status" :value="rl.status" size="small" />
          </div>

          <!-- One cell per epic column -->
          <div
            v-for="col in epicColumns"
            :key="'cell-' + rl.id + '-' + col.epicId"
            class="story-cell"
          >
            <template
              v-for="item in getItemsForCell(col.epicId, rl.id)"
              :key="item.kind + '-' + item.data.id"
            >
              <div
                :class="[
                  'work-item',
                  item.kind === 'story' ? 'work-item--story' : 'work-item--spike',
                ]"
              >
                <div class="wi-header">
                  <i :class="item.kind === 'story' ? 'pi pi-bookmark' : 'pi pi-bolt'"></i>
                  <span>{{ item.data.title }}</span>
                </div>
                <div class="wi-meta">
                  <Tag :value="item.data.status" />
                  <Tag
                    v-if="item.data.storyPoints"
                    :value="item.data.storyPoints + ' pts'"
                    severity="info"
                  />
                  <Tag v-if="item.data.sprint" :value="item.data.sprint.name" severity="warning" />
                </div>
                <div class="wi-actions">
                  <Button
                    v-if="item.kind === 'story'"
                    icon="pi pi-pencil"
                    text
                    size="small"
                    @click="editStory(col.epic!, item.data as any)"
                  />
                  <Button
                    v-if="item.kind === 'spike'"
                    icon="pi pi-pencil"
                    text
                    size="small"
                    @click="editSpike(col.epic!, item.data as any)"
                  />
                  <Button
                    v-if="item.kind === 'story'"
                    icon="pi pi-trash"
                    text
                    size="small"
                    severity="danger"
                    @click="confirmDeleteStory(col.epic!, item.data as any)"
                  />
                  <Button
                    v-if="item.kind === 'spike'"
                    icon="pi pi-trash"
                    text
                    size="small"
                    severity="danger"
                    @click="confirmDeleteSpike(col.epic!, item.data as any)"
                  />
                </div>
              </div>
            </template>
          </div>
        </template>
      </div>
    </div>

    <!-- Theme Dialog -->
    <Dialog
      v-model:visible="showThemeDialog"
      :header="editingTheme ? 'Edit Theme' : 'Add Theme'"
      :modal="true"
      :style="{ width: '32rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="theme-name" class="field-label">Name *</label>
          <InputText
            id="theme-name"
            v-model="themeForm.name"
            placeholder="Enter theme name"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="theme-description" class="field-label">Description</label>
          <Textarea
            id="theme-description"
            v-model="themeForm.description"
            placeholder="Enter theme description"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="theme-order" class="field-label">Order</label>
          <InputNumber
            id="theme-order"
            v-model="themeForm.order"
            placeholder="Display order"
            class="w-full"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showThemeDialog = false" />
        <Button label="Save" @click="saveTheme" :loading="saving" />
      </template>
    </Dialog>

    <!-- Epic Dialog -->
    <Dialog
      v-model:visible="showEpicDialog"
      :header="editingEpic ? 'Edit Epic' : 'Add Epic'"
      :modal="true"
      :style="{ width: '32rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="epic-name" class="field-label">Name *</label>
          <InputText
            id="epic-name"
            v-model="epicForm.name"
            placeholder="Enter epic name"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="epic-description" class="field-label">Description</label>
          <Textarea
            id="epic-description"
            v-model="epicForm.description"
            placeholder="Enter epic description"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="epic-order" class="field-label">Order</label>
          <InputNumber
            id="epic-order"
            v-model="epicForm.order"
            placeholder="Display order"
            class="w-full"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showEpicDialog = false" />
        <Button label="Save" @click="saveEpic" :loading="saving" />
      </template>
    </Dialog>

    <!-- Story Dialog -->
    <Dialog
      v-model:visible="showStoryDialog"
      :header="editingStory ? 'Edit Story' : 'Add Story'"
      :modal="true"
      :style="{ width: '32rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="story-title" class="field-label">Title *</label>
          <InputText
            id="story-title"
            v-model="storyForm.title"
            placeholder="Enter story title"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="story-description" class="field-label">Description</label>
          <Textarea
            id="story-description"
            v-model="storyForm.description"
            placeholder="Enter story description"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="story-solution" class="field-label">Solution Description</label>
          <Textarea
            id="story-solution"
            v-model="storyForm.solutionDescription"
            placeholder="Describe the solution"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="story-acceptance" class="field-label">Acceptance Criteria</label>
          <Textarea
            id="story-acceptance"
            v-model="storyForm.acceptanceCriteria"
            placeholder="Define acceptance criteria"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="story-status" class="field-label">Status</label>
          <Dropdown
            id="story-status"
            v-model="storyForm.status"
            :options="statusOptions"
            placeholder="Select status"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="story-points" class="field-label">Story Points</label>
          <InputNumber
            id="story-points"
            v-model="storyForm.storyPoints"
            placeholder="Story points"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="story-release" class="field-label">Release</label>
          <Dropdown
            id="story-release"
            v-model="storyForm.releaseId"
            :options="releases"
            optionLabel="name"
            optionValue="id"
            placeholder="Select release"
            showClear
            class="w-full"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showStoryDialog = false" />
        <Button label="Save" @click="saveStory" :loading="saving" />
      </template>
    </Dialog>

    <!-- Spike Dialog -->
    <Dialog
      v-model:visible="showSpikeDialog"
      :header="editingSpike ? 'Edit Spike' : 'Add Spike'"
      :modal="true"
      :style="{ width: '32rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="spike-title" class="field-label">Title *</label>
          <InputText
            id="spike-title"
            v-model="spikeForm.title"
            placeholder="Enter spike title"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="spike-description" class="field-label">Description</label>
          <Textarea
            id="spike-description"
            v-model="spikeForm.description"
            placeholder="Enter spike description"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="spike-goal" class="field-label">Investigation Goal</label>
          <Textarea
            id="spike-goal"
            v-model="spikeForm.investigationGoal"
            placeholder="What are we investigating?"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="spike-findings" class="field-label">Findings</label>
          <Textarea
            id="spike-findings"
            v-model="spikeForm.findings"
            placeholder="Document findings here"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="spike-status" class="field-label">Status</label>
          <Dropdown
            id="spike-status"
            v-model="spikeForm.status"
            :options="statusOptions"
            placeholder="Select status"
            class="w-full"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showSpikeDialog = false" />
        <Button label="Save" @click="saveSpike" :loading="saving" />
      </template>
    </Dialog>

    <!-- Sprints Dialog -->
    <Dialog
      v-model:visible="showSprintsDialog"
      header="Manage Sprints"
      :modal="true"
      :style="{ width: '48rem' }"
    >
      <div class="sprints-section">
        <Button
          label="Add Sprint"
          icon="pi pi-plus"
          @click="showSprintDialog = true"
          style="margin-bottom: 1rem"
        />
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
              <Button icon="pi pi-pencil" text size="small" @click="editSprint(slotProps.data)" />
              <Button
                icon="pi pi-trash"
                text
                size="small"
                severity="danger"
                @click="confirmDeleteSprint(slotProps.data)"
              />
            </template>
          </Column>
        </DataTable>
      </div>
    </Dialog>

    <!-- Sprint Dialog -->
    <Dialog
      v-model:visible="showSprintDialog"
      :header="editingSprint ? 'Edit Sprint' : 'Add Sprint'"
      :modal="true"
      :style="{ width: '32rem' }"
    >
      <div class="dialog-form">
        <div class="form-field">
          <label for="sprint-name" class="field-label">Name *</label>
          <InputText
            id="sprint-name"
            v-model="sprintForm.name"
            placeholder="Enter sprint name"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="sprint-goal" class="field-label">Goal</label>
          <Textarea
            id="sprint-goal"
            v-model="sprintForm.goal"
            placeholder="Sprint goal"
            rows="3"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="sprint-start" class="field-label">Start Date *</label>
          <Calendar
            id="sprint-start"
            v-model="sprintFormStartDate"
            showIcon
            dateFormat="yy-mm-dd"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="sprint-end" class="field-label">End Date *</label>
          <Calendar
            id="sprint-end"
            v-model="sprintFormEndDate"
            showIcon
            dateFormat="yy-mm-dd"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label for="sprint-status" class="field-label">Status</label>
          <Dropdown
            id="sprint-status"
            v-model="sprintForm.status"
            :options="sprintStatusOptions"
            placeholder="Select status"
            class="w-full"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" text @click="showSprintDialog = false" />
        <Button label="Save" @click="saveSprint" :loading="saving" />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import {
  userStoryMapService,
  type Theme,
  type Epic,
  type Story,
  type Spike,
  type Sprint,
  Priority,
} from "@/services/userStoryMapService";
import { backlogService, type ReleaseDto } from "@/services/backlogService";
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Message from "primevue/message";
import Tag from "primevue/tag";
import InputText from "primevue/inputtext";
import InputNumber from "primevue/inputnumber";
import Textarea from "primevue/textarea";
import Dropdown from "primevue/dropdown";
import Dialog from "primevue/dialog";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Calendar from "primevue/calendar";

const props = defineProps<{
  projectId: number;
}>();

const confirm = useConfirm();
const toast = useToast();

const projectIdValue = computed(() => props.projectId);

const loading = ref(true);
const loadingSprints = ref(false);
const saving = ref(false);
const error = ref("");

const themes = ref<Theme[]>([]);
const sprints = ref<Sprint[]>([]);
const releases = ref<ReleaseDto[]>([]);

const showThemeDialog = ref(false);
const showEpicDialog = ref(false);
const showStoryDialog = ref(false);
const showSpikeDialog = ref(false);
const showSprintsDialog = ref(false);
const showSprintDialog = ref(false);

const editingTheme = ref(false);
const editingEpic = ref(false);
const editingStory = ref(false);
const editingSpike = ref(false);
const editingSprint = ref(false);

const currentTheme = ref<Theme | null>(null);
const currentEpic = ref<Epic | null>(null);

const themeForm = ref<Theme>({ name: "", description: "", order: 0 });
const epicForm = ref<Epic>({ name: "", description: "", order: 0 });
const storyForm = ref<Story>({
  title: "",
  description: "",
  order: 0,
  priority: Priority.Medium,
  status: "Backlog",
});
const spikeForm = ref<Spike>({
  title: "",
  description: "",
  order: 0,
  priority: Priority.Medium,
  status: "Backlog",
});
const sprintForm = ref<Sprint>({
  name: "",
  goal: "",
  startDate: "",
  endDate: "",
  status: "Planned",
});
const sprintFormStartDate = ref<Date | null>(null);
const sprintFormEndDate = ref<Date | null>(null);

const statusOptions = ["Backlog", "To Do", "In Progress", "In Review", "Done"];
const sprintStatusOptions = ["Planned", "Active", "Completed"];

// ── Computed: flatten all epics into columns ─────────────────
interface EpicColumn {
  epicId: number;
  epic: Epic;
  themeId: number;
}

const epicColumns = computed<EpicColumn[]>(() => {
  const cols: EpicColumn[] = [];
  for (const theme of themes.value) {
    if (theme.epics && theme.epics.length) {
      for (const epic of theme.epics) {
        cols.push({ epicId: epic.id!, epic, themeId: theme.id! });
      }
    }
  }
  return cols;
});

// ── Computed: build release rows (+ "Unassigned" catch-all) ──
interface ReleaseRow {
  id: number | null; // null = unassigned
  name: string;
  status?: string;
}

const releaseRows = computed<ReleaseRow[]>(() => {
  const rows: ReleaseRow[] = [];
  // Add each release
  for (const r of releases.value) {
    rows.push({ id: r.id, name: r.name, status: r.status });
  }
  // Always add an "Unassigned" row at the end
  rows.push({ id: null, name: "Unassigned" });
  return rows;
});

// ── Computed: CSS grid columns ───────────────────────────────
const gridColumns = computed(() => {
  const totalEpicCols = epicColumns.value.length || 1;
  // First column is the release label, rest are epic columns
  return `8rem repeat(${totalEpicCols}, minmax(10rem, 1fr))`;
});

// ── Build a lookup for items by epicId + releaseId ───────────
type WorkItem = { kind: "story"; data: Story } | { kind: "spike"; data: Spike };

function getItemsForCell(epicId: number, releaseId: number | null): WorkItem[] {
  const items: WorkItem[] = [];
  for (const theme of themes.value) {
    for (const epic of theme.epics || []) {
      if (epic.id !== epicId) continue;
      for (const story of epic.stories || []) {
        const storyRelId = story.releaseId ?? null;
        if (storyRelId === releaseId) {
          items.push({ kind: "story", data: story });
        }
      }
      for (const spike of epic.spikes || []) {
        const spikeRelId = spike.releaseId ?? null;
        if (spikeRelId === releaseId) {
          items.push({ kind: "spike", data: spike });
        }
      }
    }
  }
  return items;
}

onMounted(async () => {
  await Promise.all([loadThemes(), loadSprints(), loadReleases()]);
  loading.value = false;
});

async function loadThemes() {
  try {
    themes.value = await userStoryMapService.getThemes(projectIdValue.value);
  } catch (err) {
    error.value = "Failed to load themes";
    console.error(err);
  }
}

async function loadReleases() {
  try {
    releases.value = await backlogService.getReleases(projectIdValue.value);
  } catch (err) {
    console.error("Failed to load releases:", err);
  }
}

async function loadSprints() {
  try {
    loadingSprints.value = true;
    sprints.value = await userStoryMapService.getSprints(projectIdValue.value);
  } catch (err) {
    console.error("Failed to load sprints:", err);
  } finally {
    loadingSprints.value = false;
  }
}

function showAddEpicDialog(theme: Theme) {
  currentTheme.value = theme;
  epicForm.value = {
    name: "",
    description: "",
    order: theme.epics?.length || 0,
    themeId: theme.id,
  };
  editingEpic.value = false;
  showEpicDialog.value = true;
}

function showAddStoryDialog(epic: Epic) {
  currentEpic.value = epic;
  storyForm.value = {
    title: "",
    description: "",
    order: epic.stories?.length || 0,
    priority: Priority.Medium,
    status: "Backlog",
    epicId: epic.id,
  };
  editingStory.value = false;
  showStoryDialog.value = true;
}

function editTheme(theme: Theme) {
  themeForm.value = { ...theme };
  editingTheme.value = true;
  showThemeDialog.value = true;
}

function editEpic(theme: Theme, epic: Epic) {
  currentTheme.value = theme;
  epicForm.value = { ...epic };
  editingEpic.value = true;
  showEpicDialog.value = true;
}

function editStory(epic: Epic, story: Story) {
  currentEpic.value = epic;
  storyForm.value = { ...story };
  editingStory.value = true;
  showStoryDialog.value = true;
}

function editSpike(epic: Epic, spike: Spike) {
  currentEpic.value = epic;
  spikeForm.value = { ...spike };
  editingSpike.value = true;
  showSpikeDialog.value = true;
}

function editSprint(sprint: Sprint) {
  sprintForm.value = { ...sprint };
  sprintFormStartDate.value = new Date(sprint.startDate);
  sprintFormEndDate.value = new Date(sprint.endDate);
  editingSprint.value = true;
  showSprintDialog.value = true;
}

async function saveTheme() {
  try {
    saving.value = true;
    if (editingTheme.value && themeForm.value.id) {
      await userStoryMapService.updateTheme(
        projectIdValue.value,
        themeForm.value.id,
        themeForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Theme updated", life: 3000 });
    } else {
      await userStoryMapService.createTheme(projectIdValue.value, themeForm.value);
      toast.add({ severity: "success", summary: "Success", detail: "Theme created", life: 3000 });
    }
    showThemeDialog.value = false;
    await loadThemes();
  } catch (err) {
    toast.add({ severity: "error", summary: "Error", detail: "Failed to save theme", life: 3000 });
    console.error(err);
  } finally {
    saving.value = false;
  }
}

async function saveEpic() {
  try {
    saving.value = true;
    if (editingEpic.value && epicForm.value.id) {
      await userStoryMapService.updateEpic(
        projectIdValue.value,
        currentTheme.value!.id!,
        epicForm.value.id,
        epicForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Epic updated", life: 3000 });
    } else {
      await userStoryMapService.createEpic(
        projectIdValue.value,
        currentTheme.value!.id!,
        epicForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Epic created", life: 3000 });
    }
    showEpicDialog.value = false;
    await loadThemes();
  } catch (err) {
    toast.add({ severity: "error", summary: "Error", detail: "Failed to save epic", life: 3000 });
    console.error(err);
  } finally {
    saving.value = false;
  }
}

async function saveStory() {
  try {
    saving.value = true;
    if (editingStory.value && storyForm.value.id) {
      await userStoryMapService.updateStory(
        projectIdValue.value,
        currentEpic.value!.id!,
        storyForm.value.id,
        storyForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Story updated", life: 3000 });
    } else {
      await userStoryMapService.createStory(
        projectIdValue.value,
        currentEpic.value!.id!,
        storyForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Story created", life: 3000 });
    }
    showStoryDialog.value = false;
    await loadThemes();
  } catch (err) {
    toast.add({ severity: "error", summary: "Error", detail: "Failed to save story", life: 3000 });
    console.error(err);
  } finally {
    saving.value = false;
  }
}

async function saveSpike() {
  try {
    saving.value = true;
    if (editingSpike.value && spikeForm.value.id) {
      await userStoryMapService.updateSpike(
        projectIdValue.value,
        currentEpic.value!.id!,
        spikeForm.value.id,
        spikeForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Spike updated", life: 3000 });
    } else {
      await userStoryMapService.createSpike(
        projectIdValue.value,
        currentEpic.value!.id!,
        spikeForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Spike created", life: 3000 });
    }
    showSpikeDialog.value = false;
    await loadThemes();
  } catch (err) {
    toast.add({ severity: "error", summary: "Error", detail: "Failed to save spike", life: 3000 });
    console.error(err);
  } finally {
    saving.value = false;
  }
}

async function saveSprint() {
  try {
    saving.value = true;
    if (sprintFormStartDate.value) {
      sprintForm.value.startDate = sprintFormStartDate.value.toISOString();
    }
    if (sprintFormEndDate.value) {
      sprintForm.value.endDate = sprintFormEndDate.value.toISOString();
    }

    if (editingSprint.value && sprintForm.value.id) {
      await userStoryMapService.updateSprint(
        projectIdValue.value,
        sprintForm.value.id,
        sprintForm.value,
      );
      toast.add({ severity: "success", summary: "Success", detail: "Sprint updated", life: 3000 });
    } else {
      await userStoryMapService.createSprint(projectIdValue.value, sprintForm.value);
      toast.add({ severity: "success", summary: "Success", detail: "Sprint created", life: 3000 });
    }
    showSprintDialog.value = false;
    await loadSprints();
  } catch (err) {
    toast.add({ severity: "error", summary: "Error", detail: "Failed to save sprint", life: 3000 });
    console.error(err);
  } finally {
    saving.value = false;
  }
}

function confirmDeleteTheme(theme: Theme) {
  confirm.require({
    message: `Are you sure you want to delete theme "${theme.name}"? This will also delete all epics, stories, and spikes under this theme.`,
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: async () => {
      try {
        await userStoryMapService.deleteTheme(projectIdValue.value, theme.id!);
        toast.add({ severity: "success", summary: "Success", detail: "Theme deleted", life: 3000 });
        await loadThemes();
      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to delete theme",
          life: 3000,
        });
        console.error(err);
      }
    },
  });
}

function confirmDeleteEpic(theme: Theme, epic: Epic) {
  confirm.require({
    message: `Are you sure you want to delete epic "${epic.name}"? This will also delete all stories and spikes under this epic.`,
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: async () => {
      try {
        await userStoryMapService.deleteEpic(projectIdValue.value, theme.id!, epic.id!);
        toast.add({ severity: "success", summary: "Success", detail: "Epic deleted", life: 3000 });
        await loadThemes();
      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to delete epic",
          life: 3000,
        });
        console.error(err);
      }
    },
  });
}

function confirmDeleteStory(epic: Epic, story: Story) {
  confirm.require({
    message: `Are you sure you want to delete story "${story.title}"?`,
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: async () => {
      try {
        await userStoryMapService.deleteStory(projectIdValue.value, epic.id!, story.id!);
        toast.add({ severity: "success", summary: "Success", detail: "Story deleted", life: 3000 });
        await loadThemes();
      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to delete story",
          life: 3000,
        });
        console.error(err);
      }
    },
  });
}

function confirmDeleteSpike(epic: Epic, spike: Spike) {
  confirm.require({
    message: `Are you sure you want to delete spike "${spike.title}"?`,
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: async () => {
      try {
        await userStoryMapService.deleteSpike(projectIdValue.value, epic.id!, spike.id!);
        toast.add({ severity: "success", summary: "Success", detail: "Spike deleted", life: 3000 });
        await loadThemes();
      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to delete spike",
          life: 3000,
        });
        console.error(err);
      }
    },
  });
}

function confirmDeleteSprint(sprint: Sprint) {
  confirm.require({
    message: `Are you sure you want to delete sprint "${sprint.name}"?`,
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: async () => {
      try {
        await userStoryMapService.deleteSprint(projectIdValue.value, sprint.id!);
        toast.add({
          severity: "success",
          summary: "Success",
          detail: "Sprint deleted",
          life: 3000,
        });
        await loadSprints();
      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "Failed to delete sprint",
          life: 3000,
        });
        console.error(err);
      }
    },
  });
}
</script>

<style scoped>
.usm-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: calc(100vh - 12rem);
  min-height: 400px;
}

.usm-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.panel-title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #0f172a;
}

.usm-actions {
  display: flex;
  gap: 0.5rem;
}

.loading-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 300px;
}

.error-state {
  padding: 1rem 0;
}

.empty-state {
  text-align: center;
  padding: 3rem;
}

/* ── Story-map grid ────────────────────────────────────────── */
.story-map-wrapper {
  flex: 1;
  min-height: 0;
  overflow-x: auto;
  overflow-y: auto;
  padding-bottom: 0;
  max-width: 100%;
  scrollbar-width: thin;
  scrollbar-color: #94a3b8 #e2e8f0;
}

.story-map-wrapper::-webkit-scrollbar {
  height: 10px;
  width: 8px;
}

.story-map-wrapper::-webkit-scrollbar-track {
  background: #e2e8f0;
  border-radius: 4px;
}

.story-map-wrapper::-webkit-scrollbar-thumb {
  background: #94a3b8;
  border-radius: 4px;
}

.story-map-wrapper::-webkit-scrollbar-thumb:hover {
  background: #64748b;
}

.story-map-grid {
  display: grid;
  gap: 0;
  min-width: max-content;
}

/* Top-left blank corner */
.grid-corner {
  background: transparent;
  border-bottom: 2px solid #e2e8f0;
  border-right: 2px solid #e2e8f0;
}

/* ── Theme header row ──────────────────────────────────────── */
.theme-header {
  background: #1e40af;
  color: #ffffff;
  padding: 0.5rem 0.75rem;
  border-bottom: 2px solid #e2e8f0;
  border-right: 1px solid #bfdbfe;
}

.theme-header:last-of-type {
  border-right: none;
}

.theme-header-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.5rem;
}

.theme-name {
  margin: 0;
  font-size: 0.875rem;
  font-weight: 700;
  color: #ffffff;
}

.theme-header .card-btns :deep(.p-button) {
  color: #bfdbfe !important;
}

.theme-header .card-btns :deep(.p-button:hover) {
  color: #ffffff !important;
  background: rgba(255, 255, 255, 0.15) !important;
}

.theme-desc {
  margin: 0.25rem 0 0;
  font-size: 0.8125rem;
  color: #bfdbfe;
}

/* ── Row labels (left column) ──────────────────────────────── */
.row-label {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 0.25rem;
  padding: 0.5rem;
  border-right: 2px solid #e2e8f0;
  border-bottom: 1px solid #e2e8f0;
  font-weight: 600;
  font-size: 0.75rem;
  color: #475569;
  background: #f8fafc;
  position: sticky;
  left: 0;
  z-index: 2;
}

.row-label--backbone {
  background: #eff6ff;
  color: #1e40af;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-bottom: 2px solid #e2e8f0;
}

.row-label--release {
  background: #f8fafc;
}

.release-name {
  color: #1e293b;
  font-size: 0.8125rem;
  font-weight: 600;
}

/* ── Epic cells (backbone row) ─────────────────────────────── */
.epic-cell {
  padding: 0.375rem;
  border-right: 1px solid #e2e8f0;
  border-bottom: 2px solid #e2e8f0;
  background: #eff6ff;
}

.epic-card {
  background: #ffffff;
  border: 1px solid #bfdbfe;
  border-radius: 0.5rem;
  padding: 0.5rem 0.625rem;
  height: 100%;
}

.epic-card--empty {
  display: flex;
  align-items: center;
  justify-content: center;
  border-style: dashed;
  border-color: #cbd5e1;
  background: #f8fafc;
}

.empty-hint {
  color: #94a3b8;
  font-size: 0.8125rem;
  font-style: italic;
  margin: 0;
}

.epic-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.25rem;
}

.epic-name {
  margin: 0;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #1e293b;
}

.epic-desc {
  margin: 0.25rem 0 0;
  font-size: 0.75rem;
  color: #64748b;
}

.card-btns {
  display: flex;
  gap: 0.125rem;
  flex-shrink: 0;
}

/* ── Story/Spike cells (body rows) ─────────────────────────── */
.story-cell {
  padding: 0.375rem;
  border-right: 1px solid #e2e8f0;
  border-bottom: 1px solid #e2e8f0;
  background: #ffffff;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  min-height: 3.5rem;
}

.work-item {
  padding: 0.5rem 0.625rem;
  border: 1px solid #e2e8f0;
  border-radius: 0.375rem;
  background: #ffffff;
}

.work-item--story {
  border-left: 3px solid #2563eb;
}

.work-item--spike {
  border-left: 3px solid #eab308;
}

.wi-header {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-weight: 600;
  font-size: 0.8125rem;
  color: #1e293b;
}

.wi-header i {
  font-size: 0.75rem;
  color: #64748b;
}

.wi-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
  margin-top: 0.25rem;
}

.wi-actions {
  display: flex;
  gap: 0.125rem;
  justify-content: flex-end;
  margin-top: 0.125rem;
}

/* ── Dialogs ───────────────────────────────────────────────── */
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

.sprints-section {
  padding: 0.5rem 0;
}
</style>
