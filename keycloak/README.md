# Keycloak Configuration

This directory contains the Keycloak realm configuration for the Not JIRA application.

## Overview

The `realm-export.json` file defines:
- **Realm**: `notjira`
- **Clients**: 
  - `notjira-api` (Backend API - bearer only)
  - `notjira-frontend` (Frontend SPA - public client with PKCE)
- **Users**: Three test users with different roles
- **Groups**: `users`, `admins`, `managers`
- **Roles**: `user`, `admin`, `manager`

## Default Users

The realm comes pre-configured with three test users:

| Username   | Password     | Email                    | Role    | Group     |
|------------|--------------|--------------------------|---------|-----------|
| admin      | admin123     | admin@notjira.local      | admin   | /admins   |
| manager    | manager123   | manager@notjira.local    | manager | /managers |
| testuser   | testuser123  | testuser@notjira.local   | user    | /users    |

**Note**: These are test credentials for development only. In production, you should:
1. Remove or disable these users
2. Use strong passwords
3. Enable user self-registration or integrate with your identity provider
4. Configure proper password policies

## Accessing Keycloak Admin Console

When running the application with Docker Compose:

1. **URL**: http://localhost:8180
2. **Admin Username**: admin
3. **Admin Password**: admin

From the admin console, you can:
- Manage users, groups, and roles
- Configure client settings
- View authentication logs
- Customize login themes
- Configure identity providers

## Client Configuration

### Frontend Client (`notjira-frontend`)

- **Client ID**: `notjira-frontend`
- **Access Type**: Public
- **Authentication Flow**: Authorization Code with PKCE (S256)
- **Valid Redirect URIs**:
  - `http://localhost/*`
  - `http://localhost:5173/*` (Vite dev server)
  - `http://localhost:80/*` (Production)
- **Web Origins**: Same as redirect URIs

### Backend Client (`notjira-api`)

- **Client ID**: `notjira-api`
- **Access Type**: Bearer-only
- **Purpose**: Validates JWT tokens from frontend

## Token Claims

The following claims are included in JWT tokens:

- `preferred_username`: User's username
- `email`: User's email address
- `given_name`: First name
- `family_name`: Last name
- `roles`: Array of realm roles assigned to the user
- `groups`: Array of groups the user belongs to (without full path)

## Security Considerations

### Development Setup

The current configuration is designed for local development:
- HTTP is enabled (not HTTPS)
- SSL requirement is set to `external`
- Hostname strict mode is disabled

### Production Setup

For production environments, you should:

1. **Enable HTTPS**:
   - Set `KC_HOSTNAME_STRICT=true`
   - Configure proper SSL certificates
   - Update `KC_HOSTNAME` to your domain

2. **Use a Dedicated Database**:
   - The current setup uses the same PostgreSQL instance as the application
   - Consider using a separate database for Keycloak in production

3. **Update Client Redirect URIs**:
   - Remove localhost URIs
   - Add production domain URIs

4. **Enable Additional Security Features**:
   - Configure brute force protection settings
   - Enable email verification
   - Set up two-factor authentication
   - Configure session timeouts

5. **Environment Variables**:
   - Use secrets management for `KEYCLOAK_ADMIN_PASSWORD`
   - Use environment-specific realm configurations

## Modifying the Configuration

To modify the realm configuration:

1. Make changes through the Keycloak Admin Console
2. Export the realm:
   ```bash
   docker exec -it notjira-keycloak /opt/keycloak/bin/kc.sh export --dir /tmp --realm notjira
   docker cp notjira-keycloak:/tmp/notjira-realm.json ./keycloak/realm-export.json
   ```
3. Commit the updated `realm-export.json` file

## Troubleshooting

### Keycloak not starting

Check the Keycloak container logs:
```bash
docker-compose logs keycloak
```

Common issues:
- Database connection failed (ensure PostgreSQL is healthy)
- Port 8180 already in use
- Invalid realm configuration

### Authentication failures

1. Check Keycloak is accessible at http://localhost:8180
2. Verify the realm exists and is enabled
3. Check client configurations match the application settings
4. Review Keycloak logs for authentication errors

### Token validation errors

1. Ensure backend configuration matches Keycloak realm settings
2. Verify the audience claim in the token
3. Check token expiration time
4. Ensure network connectivity between backend and Keycloak

## Resources

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [Keycloak Server Administration Guide](https://www.keycloak.org/docs/latest/server_admin/)
- [Securing Applications](https://www.keycloak.org/docs/latest/securing_apps/)
