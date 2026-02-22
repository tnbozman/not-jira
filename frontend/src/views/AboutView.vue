<script setup lang="ts">
import Card from 'primevue/card'
</script>

<template>
  <div class="max-w-3xl mx-auto space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-surface-900 mb-2">About Not JIRA</h1>
      <p class="text-surface-500">A modern project management tool built with modern technologies.</p>
    </div>

    <Card class="border border-surface-200">
      <template #content>
        <div class="space-y-6">
          <div>
            <h2 class="text-lg font-semibold text-surface-800 mb-3">Tech Stack</h2>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <div class="flex items-center gap-3 p-3 bg-surface-50 rounded-lg">
                <i class="pi pi-desktop text-blue-500 text-lg"></i>
                <div>
                  <p class="font-medium text-surface-800 text-sm">Frontend</p>
                  <p class="text-surface-500 text-xs">Vue 3 · PrimeVue · Tailwind CSS</p>
                </div>
              </div>
              <div class="flex items-center gap-3 p-3 bg-surface-50 rounded-lg">
                <i class="pi pi-server text-emerald-500 text-lg"></i>
                <div>
                  <p class="font-medium text-surface-800 text-sm">Backend</p>
                  <p class="text-surface-500 text-xs">.NET 10 · Entity Framework Core</p>
                </div>
              </div>
              <div class="flex items-center gap-3 p-3 bg-surface-50 rounded-lg">
                <i class="pi pi-database text-amber-500 text-lg"></i>
                <div>
                  <p class="font-medium text-surface-800 text-sm">Database</p>
                  <p class="text-surface-500 text-xs">PostgreSQL 17</p>
                </div>
              </div>
              <div class="flex items-center gap-3 p-3 bg-surface-50 rounded-lg">
                <i class="pi pi-lock text-purple-500 text-lg"></i>
                <div>
                  <p class="font-medium text-surface-800 text-sm">Auth</p>
                  <p class="text-surface-500 text-xs">Keycloak SSO · OIDC</p>
                </div>
              </div>
            </div>
          </div>

          <div>
            <h2 class="text-lg font-semibold text-surface-800 mb-3">Features</h2>
            <ul class="space-y-2">
              <li class="flex items-start gap-2 text-sm text-surface-600">
                <i class="pi pi-check-circle text-emerald-500 mt-0.5"></i>
                Project management with teams and members
              </li>
              <li class="flex items-start gap-2 text-sm text-surface-600">
                <i class="pi pi-check-circle text-emerald-500 mt-0.5"></i>
                External entity tracking with interviews and problems
              </li>
              <li class="flex items-start gap-2 text-sm text-surface-600">
                <i class="pi pi-check-circle text-emerald-500 mt-0.5"></i>
                Interactive knowledge graph visualization
              </li>
              <li class="flex items-start gap-2 text-sm text-surface-600">
                <i class="pi pi-check-circle text-emerald-500 mt-0.5"></i>
                User story mapping with themes, epics, stories, and spikes
              </li>
              <li class="flex items-start gap-2 text-sm text-surface-600">
                <i class="pi pi-check-circle text-emerald-500 mt-0.5"></i>
                Sprint planning, reviews, and retrospectives
              </li>
              <li class="flex items-start gap-2 text-sm text-surface-600">
                <i class="pi pi-check-circle text-emerald-500 mt-0.5"></i>
                Product backlog with team, release, and epic filtering
              </li>
            </ul>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>
