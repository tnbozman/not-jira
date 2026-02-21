# Not JIRA

A modern task management application built with a monorepo architecture.

## Tech Stack

### Frontend
- **Vue 3** - Progressive JavaScript framework
- **PrimeVue** - Rich UI component library
- **Tailwind CSS** - Utility-first CSS framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool

### Backend
- **.NET 10** - Modern web API framework
- **Entity Framework Core** - ORM for database access
- **PostgreSQL** - Robust relational database

### Infrastructure
- **Docker & Docker Compose** - Containerization and orchestration
- **DevContainer** - Consistent development environment

## Project Structure

```
not-jira/
├── frontend/              # Vue 3 frontend application
│   ├── src/
│   ├── Dockerfile
│   └── nginx.conf
├── backend/              # .NET 10 backend API
│   └── NotJira.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       └── Dockerfile
├── .devcontainer/        # Development container configuration
│   ├── devcontainer.json
│   ├── Dockerfile
│   └── docker-compose.dev.yml
└── docker-compose.yml    # Production deployment configuration
```

## Getting Started

### Prerequisites
- Docker Desktop or Docker Engine with Docker Compose
- (Optional) Visual Studio Code with Remote - Containers extension

### Running with Docker Compose

1. Clone the repository:
```bash
git clone https://github.com/tnbozman/not-jira.git
cd not-jira
```

2. Start all services:
```bash
docker-compose up --build
```

3. Access the application:
- Frontend: http://localhost
- Backend API: http://localhost:8080
- OpenAPI/Swagger: http://localhost:8080/openapi/v1.json

### Development with DevContainer

1. Open the project in Visual Studio Code
2. When prompted, click "Reopen in Container" (or use Command Palette: "Remote-Containers: Reopen in Container")
3. Wait for the container to build and start
4. The development environment will be ready with .NET and Node.js pre-installed

#### Running Frontend in DevContainer
```bash
cd frontend
npm run dev
```

#### Running Backend in DevContainer
```bash
cd backend/NotJira.Api
dotnet run
```

### Local Development (Without Docker)

#### Frontend Setup
```bash
cd frontend
npm install
npm run dev
```

#### Backend Setup
```bash
cd backend/NotJira.Api
dotnet restore
dotnet run
```

**Note:** You'll need to set up PostgreSQL locally and update the connection string in `appsettings.json`.

## API Endpoints

### Tasks
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get a specific task
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update a task
- `DELETE /api/tasks/{id}` - Delete a task

### Weather (Sample)
- `GET /api/weatherforecast` - Get weather forecast

## Database Migrations

To create and apply database migrations:

```bash
# Navigate to the API project
cd backend/NotJira.Api

# Create a new migration
dotnet ef migrations add InitialCreate

# Apply migrations to the database
dotnet ef database update
```

## Environment Variables

### Backend
- `ASPNETCORE_ENVIRONMENT` - Environment name (Development/Production)
- `ASPNETCORE_URLS` - URL bindings for the application
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

### Database
- `POSTGRES_USER` - PostgreSQL username
- `POSTGRES_PASSWORD` - PostgreSQL password
- `POSTGRES_DB` - Database name

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.
