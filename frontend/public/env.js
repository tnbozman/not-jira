// Default env.js – overwritten at container start by env.sh
// During local development this file is served as-is with safe defaults.
window.__ENV__ = {
  API_BASE_URL: "/api",
  KEYCLOAK_URL: "http://localhost:8180",
  KEYCLOAK_REALM: "storyfirst",
  KEYCLOAK_CLIENT_ID: "storyfirst-frontend",
};
