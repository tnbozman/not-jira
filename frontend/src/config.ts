/**
 * Runtime configuration module.
 *
 * In production (Docker), values are injected at container startup via /env.js
 * which sets window.__ENV__. In development, values fall back to Vite's
 * import.meta.env (VITE_* variables) or sensible defaults.
 */

interface RuntimeConfig {
  API_BASE_URL: string;
  KEYCLOAK_URL: string;
  KEYCLOAK_REALM: string;
  KEYCLOAK_CLIENT_ID: string;
}

declare global {
  interface Window {
    __ENV__?: Partial<RuntimeConfig>;
  }
}

function getEnv(key: keyof RuntimeConfig, fallback: string): string {
  // 1. Runtime env.js (production Docker)
  if (window.__ENV__?.[key]) {
    return window.__ENV__[key];
  }

  // 2. Vite build-time env (development)
  const viteKey = `VITE_${key}`;
  const viteVal = (import.meta.env as Record<string, string | undefined>)[viteKey];
  if (viteVal) {
    return viteVal;
  }

  // 3. Fallback default
  return fallback;
}

const config: RuntimeConfig = {
  API_BASE_URL: getEnv("API_BASE_URL", "/api"),
  KEYCLOAK_URL: getEnv("KEYCLOAK_URL", "http://localhost:8180"),
  KEYCLOAK_REALM: getEnv("KEYCLOAK_REALM", "notjira"),
  KEYCLOAK_CLIENT_ID: getEnv("KEYCLOAK_CLIENT_ID", "notjira-frontend"),
};

export default config;
