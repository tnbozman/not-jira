import Keycloak from 'keycloak-js'

const keycloakConfig = {
  url: import.meta.env.VITE_KEYCLOAK_URL || 'http://localhost:8180',
  realm: 'notjira',
  clientId: 'notjira-frontend',
}

const keycloak = new Keycloak(keycloakConfig)

export interface KeycloakUser {
  username?: string
  email?: string
  firstName?: string
  lastName?: string
  roles?: string[]
  groups?: string[]
}

interface KeycloakTokenParsed {
  preferred_username?: string
  email?: string
  given_name?: string
  family_name?: string
  roles?: string[]
  groups?: string[]
}

class KeycloakService {
  private keycloak: Keycloak

  constructor() {
    this.keycloak = keycloak
  }

  async init(): Promise<boolean> {
    try {
      const authenticated = await this.keycloak.init({
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: window.location.origin + '/silent-check-sso.html',
        pkceMethod: 'S256',
        checkLoginIframe: false,
      })

      if (authenticated) {
        // Refresh token periodically
        setInterval(() => {
          this.keycloak
            .updateToken(70)
            .then((refreshed) => {
              if (refreshed) {
                console.log('Token refreshed')
              }
            })
            .catch(() => {
              console.log('Failed to refresh token')
            })
        }, 60000) // Check every minute
      }

      return authenticated
    } catch (error) {
      console.error('Keycloak init failed:', error)
      return false
    }
  }

  login(): void {
    this.keycloak.login()
  }

  logout(): void {
    this.keycloak.logout({ redirectUri: window.location.origin })
  }

  isAuthenticated(): boolean {
    return this.keycloak.authenticated ?? false
  }

  getToken(): string | undefined {
    return this.keycloak.token
  }

  getUser(): KeycloakUser | null {
    if (!this.keycloak.tokenParsed) {
      return null
    }

    const token = this.keycloak.tokenParsed as KeycloakTokenParsed

    return {
      username: token.preferred_username,
      email: token.email,
      firstName: token.given_name,
      lastName: token.family_name,
      roles: token.roles || [],
      groups: token.groups || [],
    }
  }

  hasRole(role: string): boolean {
    const user = this.getUser()
    return user?.roles?.includes(role) ?? false
  }

  hasGroup(group: string): boolean {
    const user = this.getUser()
    return user?.groups?.includes(group) ?? false
  }

  updateToken(): Promise<boolean> {
    return this.keycloak.updateToken(30)
  }
}

export const keycloakService = new KeycloakService()
export default keycloakService
