<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import { projectService, type Project } from "@/services/projectService";

const router = useRouter();
const authStore = useAuthStore();

const projects = ref<Project[]>([]);
const loading = ref(true);
const greeting = computed(() => {
  const hour = new Date().getHours();
  if (hour < 12) return "Good morning";
  if (hour < 18) return "Good afternoon";
  return "Good evening";
});
const displayName = computed(() => authStore.username || "there");

const recentProjects = computed(() => projects.value.slice(0, 6));
const projectCount = computed(() => projects.value.length);
const totalMembers = computed(() => {
  const unique = new Set<string>();
  projects.value.forEach((p) =>
    p.members?.forEach((m) => unique.add(m.userId || m.userName || "")),
  );
  return unique.size;
});

const loadProjects = async () => {
  try {
    loading.value = true;
    projects.value = await projectService.getAllProjects();
  } catch {
    // silently fail on dashboard
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  if (authStore.isAuthenticated) {
    loadProjects();
  } else {
    loading.value = false;
  }
});
</script>

<template>
  <div class="dashboard">
    <!-- Compact greeting row -->
    <div class="greeting-row">
      <div>
        <h1 class="greeting-title">{{ greeting }}, {{ displayName }}</h1>
        <p class="greeting-sub">Here's what's happening across your projects.</p>
      </div>
      <Button
        label="New Project"
        icon="pi pi-plus"
        severity="primary"
        size="small"
        @click="router.push('/projects/new')"
      />
    </div>

    <!-- Stats bar -->
    <div class="stats-bar">
      <div class="stat-item">
        <div class="stat-icon stat-icon--blue">
          <i class="pi pi-folder"></i>
        </div>
        <div>
          <span class="stat-value">{{ projectCount }}</span>
          <span class="stat-label">Projects</span>
        </div>
      </div>
      <div class="stat-item">
        <div class="stat-icon stat-icon--teal">
          <i class="pi pi-users"></i>
        </div>
        <div>
          <span class="stat-value">{{ totalMembers }}</span>
          <span class="stat-label">Members</span>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <ProgressSpinner />
    </div>

    <!-- Projects list -->
    <div v-else-if="recentProjects.length > 0" class="projects-section">
      <div class="section-header">
        <h2 class="section-title">Your Projects</h2>
        <Button
          v-if="projectCount > 6"
          label="View All"
          icon="pi pi-arrow-right"
          iconPos="right"
          text
          size="small"
          @click="router.push('/projects')"
        />
      </div>

      <div class="project-grid">
        <div
          v-for="project in recentProjects"
          :key="project.id"
          class="project-card"
          @click="router.push(`/projects/${project.id}`)"
        >
          <div class="project-card-top">
            <span class="project-key">{{ project.key }}</span>
            <span class="project-members">
              <i class="pi pi-users"></i>
              {{ project.members?.length || 0 }}
            </span>
          </div>
          <h3 class="project-name">{{ project.name }}</h3>
          <p class="project-desc">
            {{ project.description || "No description" }}
          </p>
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else-if="!loading" class="empty-state">
      <div class="empty-icon-wrap">
        <i class="pi pi-folder-open empty-icon"></i>
      </div>
      <h3 class="empty-title">No projects yet</h3>
      <p class="empty-desc">Create your first project to get started.</p>
      <Button label="Create Project" icon="pi pi-plus" @click="router.push('/projects/new')" />
    </div>

    <!-- Quick links -->
    <div v-if="recentProjects.length > 0" class="quick-links">
      <h2 class="section-title">Quick Links</h2>
      <div class="quick-link-grid">
        <div class="quick-link" @click="router.push('/projects/new')">
          <i class="pi pi-folder-plus ql-icon ql-icon--blue"></i>
          <span class="ql-text">New Project</span>
        </div>
        <div class="quick-link" @click="router.push('/projects')">
          <i class="pi pi-list ql-icon ql-icon--teal"></i>
          <span class="ql-text">All Projects</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* ── Dashboard layout ────────────────────────────────── */
.dashboard {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* ── Greeting row ────────────────────────────────────── */
.greeting-row {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
}

.greeting-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1.3;
}

.greeting-sub {
  font-size: 0.875rem;
  color: #64748b;
  margin-top: 0.125rem;
}

/* ── Stats bar ───────────────────────────────────────── */
.stats-bar {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  padding: 0.875rem 1.25rem;
  min-width: 160px;
}

.stat-icon {
  width: 2.25rem;
  height: 2.25rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  flex-shrink: 0;
}

.stat-icon--blue {
  background-color: #dbeafe;
  color: #2563eb;
}

.stat-icon--teal {
  background-color: #ccfbf1;
  color: #0d9488;
}

.stat-value {
  display: block;
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1.2;
}

.stat-label {
  display: block;
  font-size: 0.75rem;
  color: #64748b;
}

/* ── Loading ─────────────────────────────────────────── */
.loading-state {
  display: flex;
  justify-content: center;
  padding: 4rem 0;
}

/* ── Projects section ────────────────────────────────── */
.projects-section {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #0f172a;
}

.project-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.75rem;
}

@media (min-width: 640px) {
  .project-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1024px) {
  .project-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

.project-card {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  padding: 1rem 1.25rem;
  cursor: pointer;
  transition:
    box-shadow 0.15s ease,
    border-color 0.15s ease;
}

.project-card:hover {
  box-shadow: 0 4px 12px rgb(0 0 0 / 0.06);
  border-color: #cbd5e1;
}

.project-card-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.project-key {
  display: inline-flex;
  align-items: center;
  padding: 0.125rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.6875rem;
  font-weight: 700;
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
  background-color: #dbeafe;
  color: #1e40af;
  letter-spacing: 0.02em;
}

.project-members {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: #94a3b8;
}

.project-members i {
  font-size: 0.6875rem;
}

.project-name {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #0f172a;
  line-height: 1.4;
  margin-bottom: 0.25rem;
}

.project-desc {
  font-size: 0.8125rem;
  color: #64748b;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* ── Empty state ─────────────────────────────────────── */
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
}

.empty-icon-wrap {
  width: 4rem;
  height: 4rem;
  border-radius: 50%;
  background-color: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
}

.empty-icon {
  font-size: 1.5rem;
  color: #94a3b8;
}

.empty-title {
  font-size: 1.0625rem;
  font-weight: 600;
  color: #334155;
  margin-bottom: 0.25rem;
}

.empty-desc {
  font-size: 0.875rem;
  color: #64748b;
  margin-bottom: 1.25rem;
}

/* ── Quick links ─────────────────────────────────────── */
.quick-links {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.quick-link-grid {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.quick-link {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  padding: 0.75rem 1.25rem;
  cursor: pointer;
  transition:
    box-shadow 0.15s ease,
    border-color 0.15s ease;
}

.quick-link:hover {
  box-shadow: 0 2px 8px rgb(0 0 0 / 0.05);
  border-color: #cbd5e1;
}

.ql-icon {
  font-size: 0.9375rem;
}

.ql-icon--blue {
  color: #2563eb;
}

.ql-icon--teal {
  color: #0d9488;
}

.ql-text {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #334155;
}
</style>
