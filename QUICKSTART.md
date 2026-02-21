# Quick Start Guide

This guide will help you get the Not JIRA application up and running quickly.

## Option 1: Docker Compose (Recommended for Production)

The easiest way to run the entire application with all services.

### Prerequisites
- Docker Desktop or Docker Engine with Docker Compose installed

### Steps

1. **Build and start all services:**
   ```bash
   docker-compose up --build
   ```

2. **Access the application:**
   - Frontend: http://localhost
   - Backend API: http://localhost:8080
   - PostgreSQL: localhost:5432

3. **Stop the application:**
   ```bash
   docker-compose down
   ```

4. **Stop and remove all data:**
   ```bash
   docker-compose down -v
   ```

## Option 2: DevContainer (Recommended for Development)

Best for development with VS Code.

### Prerequisites
- Visual Studio Code
- Docker Desktop
- Remote - Containers extension for VS Code

### Steps

1. **Open in DevContainer:**
   - Open the project in VS Code
   - Click "Reopen in Container" when prompted
   - Or use Command Palette: `Remote-Containers: Reopen in Container`

2. **Run the frontend (in DevContainer terminal):**
   ```bash
   cd frontend
   npm run dev
   ```
   Access at: http://localhost:5173

3. **Run the backend (in DevContainer terminal):**
   ```bash
   cd backend/NotJira.Api
   dotnet run
   ```
   Access at: http://localhost:8080

The PostgreSQL database is automatically started as part of the DevContainer.

## Option 3: Local Development

Run services locally without Docker.

### Prerequisites
- Node.js 20.x or later
- .NET 10 SDK
- PostgreSQL 17

### Steps

1. **Start PostgreSQL:**
   Ensure PostgreSQL is running on localhost:5432 with:
   - Username: postgres
   - Password: postgres
   - Database: notjira

2. **Run the backend:**
   ```bash
   cd backend/NotJira.Api
   dotnet restore
   dotnet ef database update  # Apply migrations
   dotnet run
   ```

3. **Run the frontend:**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

4. **Access the application:**
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:5000 or https://localhost:5001

## Testing the API

### Using curl

**Get all tasks:**
```bash
curl http://localhost:8080/api/tasks
```

**Create a task:**
```bash
curl -X POST http://localhost:8080/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My First Task",
    "description": "This is a test task",
    "status": "To Do",
    "priority": "High"
  }'
```

**Get weather forecast (sample endpoint):**
```bash
curl http://localhost:8080/api/weatherforecast
```

## Troubleshooting

### Docker Compose Issues

**Port already in use:**
```bash
# Stop any services using ports 80, 8080, or 5432
# Or change the ports in docker-compose.yml
```

**Database connection errors:**
```bash
# Ensure the database service is healthy
docker-compose ps

# Check backend logs
docker-compose logs backend
```

### Frontend Build Issues

**Node modules issues:**
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
```

### Backend Build Issues

**NuGet package issues:**
```bash
cd backend/NotJira.Api
dotnet clean
dotnet restore
dotnet build
```

**Database migration issues:**
```bash
cd backend/NotJira.Api
dotnet ef database drop  # WARNING: This deletes all data
dotnet ef database update
```

## Next Steps

- Read the main [README.md](README.md) for detailed documentation
- Explore the API endpoints in the Swagger/OpenAPI documentation at http://localhost:8080/openapi/v1.json
- Customize the application to fit your needs

## Support

If you encounter any issues, please check the logs:

```bash
# Docker Compose logs
docker-compose logs -f

# Specific service logs
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f db
```
