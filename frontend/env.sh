#!/bin/sh
# env.sh – Generates /usr/share/nginx/html/env.js from environment variables
# at container startup so the SPA can read runtime configuration.

cat <<EOF > /usr/share/nginx/html/env.js
window.__ENV__ = {
  API_BASE_URL: "${API_BASE_URL:-/api}",
  KEYCLOAK_URL: "${KEYCLOAK_URL:-http://localhost:8180}",
  KEYCLOAK_REALM: "${KEYCLOAK_REALM:-storyfirst}",
  KEYCLOAK_CLIENT_ID: "${KEYCLOAK_CLIENT_ID:-storyfirst-frontend}"
};
EOF

echo "env.js generated with:"
cat /usr/share/nginx/html/env.js

# Hand off to the original CMD
exec "$@"
