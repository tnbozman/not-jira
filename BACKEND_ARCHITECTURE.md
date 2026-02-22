# Backend Architecture

This document describes the architecture of the StoryFirst backend API.

## Overview

The backend has been refactored to follow ASP.NET Core best practices with:
- **Areas-based organization** for logical separation of features
- **Repository pattern** for data access abstraction
- **Global exception handling** for consistent error responses

## Areas Structure

The application is organized into five functional areas:

### 1. ProjectManagement
Handles project and project member management.

**Controllers:**
- `ProjectsController` - CRUD operations for projects and project members

**Models:** Project, ProjectMember

### 2. UserStoryMapping
Manages the user story mapping hierarchy (Theme → Epic → Story/Spike/Task).

**Controllers:**
- `ThemesController` - CRUD operations for themes
- `EpicsController` - CRUD operations for epics
- `StoriesController` - CRUD operations for user stories
- `SpikesController` - CRUD operations for spikes
- `TasksController` - CRUD operations for tasks

**Models:** Theme, Epic, Story, Spike, TaskItem

### 3. SprintPlanning
Handles sprint planning, backlog management, teams, and releases.

**Controllers:**
- `BacklogController` - Backlog views and filtering
- `SprintsController` - CRUD operations for sprints
- `TeamsController` - CRUD operations for teams
- `ReleasesController` - CRUD operations for releases

**Models:** Sprint, Team, Release, TeamPlanning

### 4. ProductDiscovery
Manages external entities, problems, interviews, and tagging.

**Controllers:**
- `ExternalEntitiesController` - CRUD operations for external entities
- `ProblemsController` - CRUD operations for problems
- `InterviewsController` - CRUD operations for interviews and notes
- `TagsController` - CRUD operations for tags

**Models:** ExternalEntity, Problem, Outcome, Interview, InterviewNote, Tag, SuccessMetric

### 5. Visualization
Provides graph visualization endpoints.

**Controllers:**
- `GraphController` - Graph data for knowledge graph visualization

## Repository Pattern

The repository pattern provides an abstraction layer between controllers and the database context.

### Base Repository

**Interface:** `IRepository<TEntity>`
- Generic CRUD operations
- Query methods (FindAsync, FirstOrDefaultAsync)
- Persistence methods (AddAsync, Update, Remove, SaveChangesAsync)

**Implementation:** `Repository<TEntity>`

### Specialized Repositories

Repositories with domain-specific query methods:

- **IProjectRepository** - GetByKeyAsync, GetWithMembersAsync
- **IThemeRepository** - GetByProjectIdAsync, GetWithDetailsAsync
- **IEpicRepository** - GetByThemeIdAsync, GetWithDetailsAsync
- **IStoryRepository** - GetByEpicIdAsync, GetWithDetailsAsync
- **ISpikeRepository** - GetByEpicIdAsync, GetWithDetailsAsync
- **IExternalEntityRepository** - GetByProjectIdAsync, GetWithDetailsAsync

### Repository Registration

All repositories are registered in `Program.cs` as scoped services:

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
// ... etc
```

## Global Exception Handling

A global exception handler middleware (`GlobalExceptionHandlerMiddleware`) catches all unhandled exceptions and returns consistent error responses:

- Maps exception types to HTTP status codes
- Returns detailed error information in development
- Returns generic error messages in production
- Logs all exceptions

## Base Controller

All controllers inherit from `BaseApiController` which provides:
- Common `[Authorize]` and `[ApiController]` attributes
- Helper methods for result handling
- Centralized exception handling support

## Common Folder Structure

```
StoryFirst.Api/
├── Areas/
│   ├── ProjectManagement/
│   │   ├── Controllers/
│   │   ├── Models/      (for area-specific models if needed)
│   │   └── Services/    (for area-specific services if needed)
│   ├── UserStoryMapping/
│   ├── SprintPlanning/
│   ├── ProductDiscovery/
│   └── Visualization/
├── Common/
│   └── Controllers/
│       └── BaseApiController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── Middleware/
│   └── GlobalExceptionHandlerMiddleware.cs
├── Models/              (common/shared models)
├── Repositories/        (repository interfaces and implementations)
└── Program.cs
```

## API Routes

All routes follow the pattern: `/api/[controller]/[action]`

Since controllers are now in areas, they maintain their original route patterns for backward compatibility. The `[Area]` attribute is for organizational purposes and doesn't change the URL structure.

Example routes:
- `/api/projects` - ProjectManagement area
- `/api/projects/{projectId}/themes` - UserStoryMapping area
- `/api/projects/{projectId}/backlog` - SprintPlanning area
- `/api/projects/{projectId}/external-entities` - ProductDiscovery area
- `/api/projects/{projectId}/graph` - Visualization area

## Benefits of This Architecture

1. **Separation of Concerns** - Each area focuses on a specific domain
2. **Testability** - Repository pattern enables easier unit testing
3. **Maintainability** - Clear structure makes code easier to navigate and modify
4. **Scalability** - Areas can grow independently
5. **Consistency** - Global exception handling and base controllers ensure consistent behavior
6. **Flexibility** - Repository pattern allows changing data access strategy without affecting controllers
