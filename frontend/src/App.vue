<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { computed, ref } from "vue";
import Button from "primevue/button";
import Avatar from "primevue/avatar";
import Menu from "primevue/menu";
import Toast from "primevue/toast";
import ConfirmDialog from "primevue/confirmdialog";

const authStore = useAuthStore();

const isAuthenticated = computed(() => authStore.isAuthenticated);
const username = computed(() => authStore.username);
const userInitial = computed(() => username.value?.charAt(0).toUpperCase() || "?");
const mobileMenuOpen = ref(false);
const userMenu = ref();

const userMenuItems = computed(() => [
  {
    label: username.value || "User",
    items: [
      {
        label: "Logout",
        icon: "pi pi-sign-out",
        command: () => authStore.logout(),
      },
    ],
  },
]);

const toggleUserMenu = (event: Event) => {
  userMenu.value.toggle(event);
};

const handleLogin = () => {
  authStore.login();
};
</script>

<template>
  <div class="min-h-screen bg-surface-50">
    <Toast />
    <ConfirmDialog />

    <!-- Top Navigation Bar -->
    <header class="bg-white border-b border-surface-200 sticky top-0 z-50 shadow-sm">
      <div class="max-w-screen-2xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <!-- Logo & Brand -->
          <div class="flex items-center gap-2">
            <RouterLink to="/" class="flex items-center gap-2 no-underline">
              <i class="pi pi-objects-column text-primary text-xl"></i>
              <span class="text-xl font-bold text-surface-900 tracking-tight">Not JIRA</span>
            </RouterLink>
          </div>

          <!-- Desktop Nav Links -->
          <nav class="hidden md:flex items-center gap-1">
            <RouterLink to="/" class="nav-link">
              <i class="pi pi-home text-sm"></i>
              <span>Dashboard</span>
            </RouterLink>
            <RouterLink to="/projects" class="nav-link">
              <i class="pi pi-folder text-sm"></i>
              <span>Projects</span>
            </RouterLink>
            <RouterLink to="/about" class="nav-link">
              <i class="pi pi-info-circle text-sm"></i>
              <span>About</span>
            </RouterLink>
          </nav>

          <!-- Right Section: Auth -->
          <div class="flex items-center gap-3">
            <template v-if="isAuthenticated">
              <div
                class="flex items-center gap-2 cursor-pointer hover:bg-surface-100 rounded-full py-1 px-2 transition-colors"
                @click="toggleUserMenu"
              >
                <Avatar :label="userInitial" shape="circle" class="bg-primary text-white" />
                <span class="hidden sm:inline text-sm font-medium text-surface-700">{{
                  username
                }}</span>
                <i class="pi pi-chevron-down text-xs text-surface-400"></i>
              </div>
              <Menu ref="userMenu" :model="userMenuItems" :popup="true" />
            </template>

            <Button
              v-else
              label="Sign In"
              icon="pi pi-sign-in"
              severity="primary"
              size="small"
              @click="handleLogin"
            />

            <!-- Mobile hamburger -->
            <Button
              class="md:hidden"
              icon="pi pi-bars"
              text
              rounded
              @click="mobileMenuOpen = !mobileMenuOpen"
            />
          </div>
        </div>
      </div>

      <!-- Mobile Nav -->
      <div
        v-if="mobileMenuOpen"
        class="md:hidden border-t border-surface-200 bg-white px-4 pb-4 pt-2 space-y-1"
      >
        <RouterLink to="/" class="mobile-nav-link" @click="mobileMenuOpen = false">
          <i class="pi pi-home"></i> Dashboard
        </RouterLink>
        <RouterLink to="/projects" class="mobile-nav-link" @click="mobileMenuOpen = false">
          <i class="pi pi-folder"></i> Projects
        </RouterLink>
        <RouterLink to="/about" class="mobile-nav-link" @click="mobileMenuOpen = false">
          <i class="pi pi-info-circle"></i> About
        </RouterLink>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-screen-2xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.nav-link {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 0.875rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--p-surface-600);
  text-decoration: none;
  transition: all 0.15s ease;
}

.nav-link:hover {
  background-color: var(--p-surface-100);
  color: var(--p-surface-900);
}

.nav-link.router-link-exact-active {
  background-color: var(--p-primary-50);
  color: var(--p-primary-600);
}

.mobile-nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 0.75rem;
  border-radius: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 500;
  color: var(--p-surface-700);
  text-decoration: none;
  transition: background-color 0.15s ease;
}

.mobile-nav-link:hover,
.mobile-nav-link.router-link-exact-active {
  background-color: var(--p-primary-50);
  color: var(--p-primary-600);
}
</style>
