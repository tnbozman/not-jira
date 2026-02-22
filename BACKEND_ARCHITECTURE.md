# Backend Architecture

This document describes the architecture of the StoryFirst backend API.

## Overview

The backend follows ASP.NET Core best practices with a three-layer architecture:
- **Controllers** - Handle HTTP requests/responses and route to services
- **Services** - Contain business logic and validation
- **Repositories** - Abstract data access layer

Additional architectural patterns:
- **Areas-based organization** for logical separation of features
- **Global exception handling** for consistent error responses

## Three-Layer Architecture

### Controllers (Presentation Layer)
Controllers are thin and focused on HTTP concerns:
- Accept HTTP requests
- Validate route parameters
- Delegate to services for business logic
- Handle exceptions from services and return appropriate HTTP status codes
- Format responses

**Exception Handling Pattern:**
```csharp
try
{
    var result = await _service.OperationAsync(params);
    return Ok(result);
}
catch (ArgumentException ex)
{
    return BadRequest(ex.Message);
}
catch (KeyNotFoundException)
{
    return NotFound();
}
catch (InvalidOperationException ex)
{
    return Conflict(ex.Message);
}
```

### Services (Business Logic Layer)
Services contain all business logic and validation:
- Validate input data
- Enforce business rules
- Coordinate between multiple repositories
- Manage timestamps (CreatedAt, UpdatedAt)
- Throw typed exceptions for error conditions

**Service Responsibilities:**
- All data storage calls via repositories
- All validation logic (format, existence, uniqueness, etc.)
- Business rule enforcement
- Data transformation and aggregation
- Exception throwing (not HTTP responses)

**Exception Pattern:**
```csharp
if (string.IsNullOrWhiteSpace(entity.Property))
{
    throw new ArgumentException("Property is required");
}

var existing = await _repository.GetByIdAsync(id);
if (existing == null)
{
    throw new KeyNotFoundException("Entity not found");
}
```

### Repositories (Data Access Layer)
Repositories abstract database operations:
- CRUD operations
- Query methods
- Persistence methods
- No business logic

## Areas Structure

The application is organized into five functional areas:

### 1. ProjectManagement
Handles project and project member management.

**Controllers:**
- `ProjectsController` - CRUD operations for projects and project members

**Services:**
- `IProjectService` / `ProjectService` - Project and member business logic

**Models:** Project, ProjectMember

### 2. UserStoryMapping
Manages the user story mapping hierarchy (Theme → Epic → Story/Spike/Task).

**Controllers:**
- `ThemesController` - CRUD operations for themes
- `EpicsController` - CRUD operations for epics
- `StoriesController` - CRUD operations for user stories
- `SpikesController` - CRUD operations for spikes
- `TasksController` - CRUD operations for tasks

**Services:**
- `IThemeService` / `ThemeService` - Theme business logic and hierarchical data retrieval
- `IEpicService` / `EpicService` - Epic business logic
- `IStoryService` / `StoryService` - User story business logic
- `ISpikeService` / `SpikeService` - Spike business logic
- `ITaskService` / `TaskService` - Task business logic

**Models:** Theme, Epic, Story, Spike, TaskItem

### 3. SprintPlanning
Handles sprint planning, backlog management, teams, and releases.

**Controllers:**
- `BacklogController` - Backlog views and filtering
- `SprintsController` - CRUD operations for sprints
- `TeamsController` - CRUD operations for teams
- `ReleasesController` - CRUD operations for releases

**Services:**
- `IBacklogService` / `BacklogService` - Backlog aggregation and filtering
- `ISprintService` / `SprintService` - Sprint and team planning business logic
- `ITeamService` / `TeamService` - Team business logic
- `IReleaseService` / `ReleaseService` - Release business logic

**Models:** Sprint, Team, Release, TeamPlanning, BacklogItemDto, BacklogResponse, SprintGroup

### 4. ProductDiscovery
Manages external entities, problems, interviews, and tagging.

**Controllers:**
- `ExternalEntitiesController` - CRUD operations for external entities
- `ProblemsController` - CRUD operations for problems
- `InterviewsController` - CRUD operations for interviews and notes
- `TagsController` - CRUD operations for tags

**Services:**
- `IExternalEntityService` / `ExternalEntityService` - External entity business logic
- `IProblemService` / `ProblemService` - Problem business logic and entity validation
- `IInterviewService` / `InterviewService` - Interview and notes business logic
- `ITagService` / `TagService` - Tag business logic with duplicate checking

**Models:** ExternalEntity, Problem, Outcome, Interview, InterviewNote, Tag, SuccessMetric

### 5. Visualization
Provides graph visualization endpoints.

**Controllers:**
- `GraphController` - Graph data for knowledge graph visualization

**Services:**
- `IGraphService` / `GraphService` - Graph data aggregation and transformation

**Models:** GraphData, GraphNode, GraphEdge, GraphStats

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

### Service Registration

All services are registered in `Program.cs` as scoped services:

```csharp
// ProjectManagement
builder.Services.AddScoped<IProjectService, ProjectService>();

// UserStoryMapping
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IEpicService, EpicService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<ISpikeService, SpikeService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// SprintPlanning
builder.Services.AddScoped<IBacklogService, BacklogService>();
builder.Services.AddScoped<ISprintService, SprintService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IReleaseService, ReleaseService>();

// ProductDiscovery
builder.Services.AddScoped<IExternalEntityService, ExternalEntityService>();
builder.Services.AddScoped<IProblemService, ProblemService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<ITagService, TagService>();

// Visualization
builder.Services.AddScoped<IGraphService, GraphService>();
```

## Service Layer Pattern

### Service Method Naming Conventions

Services follow consistent naming conventions:
- **Retrieval:** `GetAllAsync()`, `GetByIdAsync(id)`, `GetByKeyAsync(key)`, `GetAllByProjectAsync(projectId)`
- **Mutations:** `CreateAsync(entity)`, `UpdateAsync(id, entity)`, `DeleteAsync(id)`
- **Specific Operations:** `AddMemberAsync()`, `RemoveMemberAsync()`, `GetBacklogAsync()`, etc.

### Service Design Principles

1. **Single Responsibility** - Each service manages one aggregate or domain concept
2. **Dependency Injection** - Services inject repositories (never inject other services to avoid circular dependencies)
3. **Exception-Based Error Handling** - Services throw typed exceptions, controllers catch and map to HTTP responses
4. **No HTTP Concerns** - Services don't know about HTTP, only business logic
5. **Stateless** - Services are stateless and scoped per request

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
│   │   └── Services/
│   │       ├── IProjectService.cs
│   │       └── ProjectService.cs
│   ├── UserStoryMapping/
│   │   ├── Controllers/
│   │   └── Services/
│   │       ├── IThemeService.cs
│   │       ├── ThemeService.cs
│   │       ├── IEpicService.cs
│   │       ├── EpicService.cs
│   │       ├── IStoryService.cs
│   │       ├── StoryService.cs
│   │       ├── ISpikeService.cs
│   │       ├── SpikeService.cs
│   │       ├── ITaskService.cs
│   │       └── TaskService.cs
│   ├── SprintPlanning/
│   │   ├── Controllers/
│   │   ├── Models/        (DTOs for backlog and sprint planning)
│   │   └── Services/
│   │       ├── IBacklogService.cs
│   │       ├── BacklogService.cs
│   │       ├── ISprintService.cs
│   │       ├── SprintService.cs
│   │       ├── ITeamService.cs
│   │       ├── TeamService.cs
│   │       ├── IReleaseService.cs
│   │       └── ReleaseService.cs
│   ├── ProductDiscovery/
│   │   ├── Controllers/
│   │   └── Services/
│   │       ├── IExternalEntityService.cs
│   │       ├── ExternalEntityService.cs
│   │       ├── IProblemService.cs
│   │       ├── ProblemService.cs
│   │       ├── IInterviewService.cs
│   │       ├── InterviewService.cs
│   │       ├── ITagService.cs
│   │       └── TagService.cs
│   └── Visualization/
│       ├── Controllers/
│       ├── Models/        (Graph DTOs)
│       └── Services/
│           ├── IGraphService.cs
│           └── GraphService.cs
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

1. **Separation of Concerns** 
   - Controllers handle HTTP concerns
   - Services handle business logic
   - Repositories handle data access
   
2. **Testability** 
   - Service layer can be unit tested independently
   - Repository pattern enables mocking for service tests
   - Controllers can be tested with mocked services

3. **Maintainability** 
   - Clear structure makes code easier to navigate
   - Business logic centralized in services
   - Changes to business rules require changes only in service layer

4. **Scalability** 
   - Areas can grow independently
   - Services can be extracted to microservices if needed
   - Repository pattern allows changing data access strategy

5. **Consistency** 
   - All controllers follow the same pattern
   - Exception handling is consistent across all areas
   - Service naming conventions ensure predictability

6. **Flexibility** 
   - Easy to add new validation rules in services
   - Controllers remain thin and focused
   - Business logic can be reused across multiple controllers

## Request Flow

```
HTTP Request
    ↓
Controller (validate route params, invoke service)
    ↓
Service (validate data, enforce business rules, call repository)
    ↓
Repository (query database)
    ↓
Database
```

**Response Flow:**
```
Database
    ↓
Repository (return entities)
    ↓
Service (transform/aggregate data, or throw exception)
    ↓
Controller (catch exceptions, format HTTP response)
    ↓
HTTP Response
```
