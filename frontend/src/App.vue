<script setup lang="ts">
import { RouterLink, RouterView, useRoute } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { computed, ref } from "vue";
import { useRouter } from "vue-router";
import Button from "primevue/button";
import Avatar from "primevue/avatar";
import Menu from "primevue/menu";
import Toast from "primevue/toast";
import ConfirmDialog from "primevue/confirmdialog";

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();

const isAuthenticated = computed(() => authStore.isAuthenticated);
const username = computed(() => authStore.username);
const userInitial = computed(() => username.value?.charAt(0).toUpperCase() || "?");
const mobileMenuOpen = ref(false);
const userMenu = ref();
const createMenu = ref();

const userMenuItems = computed(() => [
  {
    label: username.value || "User",
    items: [
      {
        label: "About StoryFirst",
        icon: "pi pi-info-circle",
        command: () => router.push("/about"),
      },
      {
        separator: true,
      },
      {
        label: "Logout",
        icon: "pi pi-sign-out",
        command: () => authStore.logout(),
      },
    ],
  },
]);

const createMenuItems = [
  {
    label: "Create",
    items: [
      {
        label: "New Project",
        icon: "pi pi-folder-plus",
        command: () => router.push("/projects/new"),
      },
    ],
  },
];

const toggleUserMenu = (event: Event) => {
  userMenu.value.toggle(event);
};

const toggleCreateMenu = (event: Event) => {
  createMenu.value.toggle(event);
};

const handleLogin = () => {
  authStore.login();
};

/* Active route helpers for nav highlighting */
const isHomeActive = computed(() => route.path === "/");
const isProjectsActive = computed(() => route.path.startsWith("/projects"));
</script>

<template>
  <div class="app-shell">
    <Toast />
    <ConfirmDialog />

    <!-- Top Navigation Bar -->
    <header class="app-header">
      <div class="header-inner">
        <!-- Left: Logo + Nav -->
        <div class="header-left">
          <RouterLink to="/" class="brand-link">
            <i class="pi pi-objects-column brand-icon"></i>
            <span class="brand-name">StoryFirst</span>
          </RouterLink>

          <!-- Desktop Nav Links -->
          <nav class="desktop-nav">
            <RouterLink to="/" class="nav-link" :class="{ 'nav-link--active': isHomeActive }">
              <i class="pi pi-th-large"></i>
              <span>Dashboard</span>
            </RouterLink>
            <RouterLink
              to="/projects"
              class="nav-link"
              :class="{ 'nav-link--active': isProjectsActive }"
            >
              <i class="pi pi-folder"></i>
              <span>Projects</span>
            </RouterLink>
          </nav>
        </div>

        <!-- Right: Actions + Auth -->
        <div class="header-right">
          <template v-if="isAuthenticated">
            <!-- Quick Create -->
            <Button
              icon="pi pi-plus"
              severity="primary"
              size="small"
              rounded
              aria-label="Create"
              @click="toggleCreateMenu"
              class="create-btn"
            />
            <Menu ref="createMenu" :model="createMenuItems" :popup="true" />

            <!-- User Avatar -->
            <div class="user-trigger" @click="toggleUserMenu">
              <Avatar :label="userInitial" shape="circle" class="user-avatar" />
              <span class="user-name">{{ username }}</span>
              <i class="pi pi-chevron-down user-chevron"></i>
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
            class="mobile-menu-btn"
            icon="pi pi-bars"
            text
            rounded
            @click="mobileMenuOpen = !mobileMenuOpen"
          />
        </div>
      </div>

      <!-- Mobile Nav -->
      <div v-if="mobileMenuOpen" class="mobile-nav">
        <RouterLink to="/" class="mobile-nav-link" @click="mobileMenuOpen = false">
          <i class="pi pi-th-large"></i> Dashboard
        </RouterLink>
        <RouterLink to="/projects" class="mobile-nav-link" @click="mobileMenuOpen = false">
          <i class="pi pi-folder"></i> Projects
        </RouterLink>
        <RouterLink
          v-if="isAuthenticated"
          to="/projects/new"
          class="mobile-nav-link"
          @click="mobileMenuOpen = false"
        >
          <i class="pi pi-plus"></i> New Project
        </RouterLink>
        <RouterLink to="/about" class="mobile-nav-link" @click="mobileMenuOpen = false">
          <i class="pi pi-info-circle"></i> About
        </RouterLink>
      </div>
    </header>

    <!-- Main Content -->
    <main class="app-main">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
/* ── Shell layout ────────────────────────────────────── */
.app-shell {
  min-height: 100vh;
  background-color: #f8fafc; /* slate-50 – a reliable neutral */
}

.app-main {
  max-width: 1600px;
  margin: 0 auto;
  padding: 1rem 0.5rem;
}

@media (min-width: 640px) {
  .app-main {
    padding: 1rem 1rem;
  }
}

@media (min-width: 1024px) {
  .app-main {
    padding: 1rem 1.5rem;
  }
}

/* ── Header ──────────────────────────────────────────── */
.app-header {
  position: sticky;
  top: 0;
  z-index: 50;
  background-color: #ffffff;
  border-bottom: 1px solid #e2e8f0; /* slate-200 */
  box-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.04);
}

.header-inner {
  max-width: 1440px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 3.5rem;
  padding: 0 1rem;
}

@media (min-width: 640px) {
  .header-inner {
    padding: 0 1.5rem;
  }
}

@media (min-width: 1024px) {
  .header-inner {
    padding: 0 2rem;
  }
}

/* ── Left side: Brand + Nav ──────────────────────────── */
.header-left {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.brand-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  flex-shrink: 0;
}

.brand-icon {
  font-size: 1.25rem;
  color: #2563eb; /* blue-600 */
}

.brand-name {
  font-size: 1.125rem;
  font-weight: 700;
  color: #0f172a; /* slate-900 */
  letter-spacing: -0.02em;
}

.desktop-nav {
  display: none;
  align-items: center;
  gap: 0.25rem;
}

@media (min-width: 768px) {
  .desktop-nav {
    display: flex;
  }
}

/* ── Nav links ───────────────────────────────────────── */
.nav-link {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.4rem 0.75rem;
  border-radius: 0.375rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #64748b; /* slate-500 */
  text-decoration: none;
  transition:
    color 0.15s ease,
    background-color 0.15s ease;
}

.nav-link i {
  font-size: 0.8125rem;
}

.nav-link:hover {
  background-color: #f1f5f9; /* slate-100 */
  color: #0f172a; /* slate-900 */
}

.nav-link--active {
  background-color: #eff6ff; /* blue-50 */
  color: #2563eb; /* blue-600 */
}

.nav-link--active:hover {
  background-color: #dbeafe; /* blue-100 */
  color: #1d4ed8; /* blue-700 */
}

/* ── Right side ──────────────────────────────────────── */
.header-right {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.create-btn {
  width: 2rem !important;
  height: 2rem !important;
}

.user-trigger {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: 9999px;
  transition: background-color 0.15s ease;
}

.user-trigger:hover {
  background-color: #f1f5f9; /* slate-100 */
}

.user-avatar {
  width: 1.75rem !important;
  height: 1.75rem !important;
  font-size: 0.75rem !important;
  background-color: #2563eb !important; /* blue-600 */
  color: #ffffff !important;
}

.user-name {
  display: none;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #334155; /* slate-700 */
}

@media (min-width: 640px) {
  .user-name {
    display: inline;
  }
}

.user-chevron {
  font-size: 0.625rem;
  color: #94a3b8; /* slate-400 */
}

.mobile-menu-btn {
  display: inline-flex;
}

@media (min-width: 768px) {
  .mobile-menu-btn {
    display: none;
  }
}

/* ── Mobile nav ──────────────────────────────────────── */
.mobile-nav {
  display: block;
  border-top: 1px solid #e2e8f0; /* slate-200 */
  background-color: #ffffff;
  padding: 0.5rem 1rem 1rem;
}

@media (min-width: 768px) {
  .mobile-nav {
    display: none;
  }
}

.mobile-nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 0.75rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #334155; /* slate-700 */
  text-decoration: none;
  transition: background-color 0.15s ease;
}

.mobile-nav-link:hover,
.mobile-nav-link.router-link-exact-active {
  background-color: #eff6ff; /* blue-50 */
  color: #2563eb; /* blue-600 */
}
</style>
