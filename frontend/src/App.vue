<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { computed } from 'vue'

const authStore = useAuthStore()

const isAuthenticated = computed(() => authStore.isAuthenticated)
const username = computed(() => authStore.username)

const handleLogout = () => {
  authStore.logout()
}

const handleLogin = () => {
  authStore.login()
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 py-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between">
          <h1 class="text-3xl font-bold text-gray-900">Not JIRA</h1>
          <nav class="flex gap-4 items-center">
            <RouterLink 
              to="/" 
              class="px-4 py-2 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors router-link"
            >
              Home
            </RouterLink>
            <RouterLink 
              to="/about" 
              class="px-4 py-2 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors router-link"
            >
              About
            </RouterLink>
            
            <div v-if="isAuthenticated" class="flex items-center gap-4 ml-4 border-l pl-4">
              <span class="text-sm text-gray-600">
                <i class="pi pi-user mr-1"></i>
                {{ username }}
              </span>
              <button
                @click="handleLogout"
                class="px-4 py-2 rounded-md text-sm font-medium text-white bg-red-600 hover:bg-red-700 transition-colors"
              >
                <i class="pi pi-sign-out mr-1"></i>
                Logout
              </button>
            </div>
            
            <div v-else class="flex items-center gap-4 ml-4 border-l pl-4">
              <button
                @click="handleLogin"
                class="px-4 py-2 rounded-md text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 transition-colors"
              >
                <i class="pi pi-sign-in mr-1"></i>
                Login
              </button>
            </div>
          </nav>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.router-link.router-link-exact-active {
  background-color: #dbeafe;
  color: #1d4ed8;
}
</style>
