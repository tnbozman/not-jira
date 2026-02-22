<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import Button from "primevue/button";
import Card from "primevue/card";
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
  <div class="space-y-8">
    <!-- Welcome Banner -->
    <div class="bg-gradient-to-r from-blue-600 to-indigo-700 rounded-2xl p-8 text-white shadow-lg">
      <h1 class="text-3xl font-bold mb-2">{{ greeting }}, {{ displayName }}!</h1>
      <p class="text-blue-100 text-lg">Welcome to Not JIRA — your project management workspace.</p>
    </div>

    <!-- Quick Actions -->
    <div>
      <h2 class="text-xl font-semibold text-surface-800 mb-4">Quick Actions</h2>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card
          class="cursor-pointer hover:shadow-md transition-shadow border border-surface-200"
          @click="router.push('/projects/new')"
        >
          <template #content>
            <div class="flex flex-col items-center text-center gap-3 py-2">
              <div class="w-12 h-12 rounded-full bg-blue-100 flex items-center justify-center">
                <i class="pi pi-plus text-blue-600 text-xl"></i>
              </div>
              <div>
                <p class="font-semibold text-surface-800">New Project</p>
                <p class="text-sm text-surface-500">Start a new project</p>
              </div>
            </div>
          </template>
        </Card>

        <Card
          class="cursor-pointer hover:shadow-md transition-shadow border border-surface-200"
          @click="router.push('/projects')"
        >
          <template #content>
            <div class="flex flex-col items-center text-center gap-3 py-2">
              <div class="w-12 h-12 rounded-full bg-emerald-100 flex items-center justify-center">
                <i class="pi pi-folder-open text-emerald-600 text-xl"></i>
              </div>
              <div>
                <p class="font-semibold text-surface-800">View Projects</p>
                <p class="text-sm text-surface-500">
                  {{ projects.length }} project{{ projects.length !== 1 ? "s" : "" }}
                </p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="border border-surface-200">
          <template #content>
            <div class="flex flex-col items-center text-center gap-3 py-2">
              <div class="w-12 h-12 rounded-full bg-amber-100 flex items-center justify-center">
                <i class="pi pi-users text-amber-600 text-xl"></i>
              </div>
              <div>
                <p class="font-semibold text-surface-800">Entities</p>
                <p class="text-sm text-surface-500">Manage stakeholders</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="border border-surface-200">
          <template #content>
            <div class="flex flex-col items-center text-center gap-3 py-2">
              <div class="w-12 h-12 rounded-full bg-purple-100 flex items-center justify-center">
                <i class="pi pi-sitemap text-purple-600 text-xl"></i>
              </div>
              <div>
                <p class="font-semibold text-surface-800">Story Maps</p>
                <p class="text-sm text-surface-500">Plan your work</p>
              </div>
            </div>
          </template>
        </Card>
      </div>
    </div>

    <!-- Recent Projects -->
    <div v-if="projects.length > 0">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-xl font-semibold text-surface-800">Recent Projects</h2>
        <Button
          label="View All"
          icon="pi pi-arrow-right"
          iconPos="right"
          text
          size="small"
          @click="router.push('/projects')"
        />
      </div>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <Card
          v-for="project in projects.slice(0, 6)"
          :key="project.id"
          class="cursor-pointer hover:shadow-md transition-shadow border border-surface-200"
          @click="router.push(`/projects/${project.id}`)"
        >
          <template #content>
            <div class="space-y-3">
              <div class="flex items-center gap-2">
                <span
                  class="inline-flex items-center px-2.5 py-0.5 rounded text-xs font-mono font-semibold bg-blue-100 text-blue-700"
                >
                  {{ project.key }}
                </span>
              </div>
              <h3 class="font-semibold text-surface-800 text-lg">{{ project.name }}</h3>
              <p class="text-sm text-surface-500 line-clamp-2">
                {{ project.description || "No description" }}
              </p>
              <div class="flex items-center gap-3 text-sm text-surface-400 pt-1">
                <span class="flex items-center gap-1">
                  <i class="pi pi-users text-xs"></i>
                  {{ project.members?.length || 0 }} members
                </span>
              </div>
            </div>
          </template>
        </Card>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="!loading" class="text-center py-12">
      <div
        class="w-20 h-20 rounded-full bg-surface-100 flex items-center justify-center mx-auto mb-4"
      >
        <i class="pi pi-folder-open text-surface-400 text-3xl"></i>
      </div>
      <h3 class="text-lg font-semibold text-surface-700 mb-2">No projects yet</h3>
      <p class="text-surface-500 mb-6">Create your first project to get started.</p>
      <Button label="Create Project" icon="pi pi-plus" @click="router.push('/projects/new')" />
    </div>
  </div>
</template>
