import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import keycloakService, { type KeycloakUser } from '@/services/keycloakService'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<KeycloakUser | null>(null)
  const isAuthenticated = ref(false)
  const isInitialized = ref(false)

  const username = computed(() => user.value?.username)
  const email = computed(() => user.value?.email)
  const roles = computed(() => user.value?.roles || [])
  const groups = computed(() => user.value?.groups || [])

  async function initialize() {
    try {
      const authenticated = await keycloakService.init()
      isAuthenticated.value = authenticated

      if (authenticated) {
        user.value = keycloakService.getUser()
      }

      isInitialized.value = true
      return authenticated
    } catch (error) {
      console.error('Failed to initialize auth:', error)
      isInitialized.value = true
      return false
    }
  }

  function login() {
    keycloakService.login()
  }

  function logout() {
    keycloakService.logout()
    user.value = null
    isAuthenticated.value = false
  }

  function hasRole(role: string): boolean {
    return keycloakService.hasRole(role)
  }

  function hasGroup(group: string): boolean {
    return keycloakService.hasGroup(group)
  }

  return {
    user,
    isAuthenticated,
    isInitialized,
    username,
    email,
    roles,
    groups,
    initialize,
    login,
    logout,
    hasRole,
    hasGroup,
  }
})
