# LeSS (Large Scale Scrum) Developer Capabilities - Implementation Summary

## Overview
This implementation adds developer-focused capabilities to support Large Scale Scrum (LeSS) methodology, enabling teams to manage sprints, backlogs, and collaborative planning sessions.

## Backend Changes

### New Database Models

#### Team Model (`backend/NotJira.Api/Models/Team.cs`)
- **Purpose**: Represents development teams within a project
- **Fields**:
  - Id, Name, Description
  - ProjectId (foreign key)
  - CreatedAt, UpdatedAt
- **Relationships**: Has many Stories and Spikes

#### Release Model (`backend/NotJira.Api/Models/Release.cs`)
- **Purpose**: Represents product releases for grouping work items
- **Fields**:
  - Id, Name, Description
  - StartDate, ReleaseDate
  - Status (Planned, InProgress, Released)
  - ProjectId (foreign key)
  - CreatedAt, UpdatedAt
- **Relationships**: Has many Stories and Spikes

#### TeamPlanning Model (`backend/NotJira.Api/Models/TeamPlanning.cs`)
- **Purpose**: Stores Planning 2 notes per team for each sprint
- **Fields**:
  - Id, PlanningTwoNotes
  - SprintId, TeamId (foreign keys)
  - CreatedAt, UpdatedAt
- **Relationships**: Belongs to Sprint and Team

### Enhanced Models

#### Sprint Model Updates
- Added **PlanningOneNotes**: Overall planning session notes
- Added **ReviewNotes**: Combined sprint review notes
- Added **RetroNotes**: Combined retrospective notes
- Added **TeamPlannings** collection: Planning 2 notes per team

#### Story and Spike Model Updates
- Added **TeamId**: Optional team assignment
- Added **ReleaseId**: Optional release assignment
- Added **AssigneeId**: User ID from Keycloak
- Added **AssigneeName**: Cached user name for display

### New API Controllers

#### TeamsController (`backend/NotJira.Api/Controllers/TeamsController.cs`)
- **Route**: `/api/projects/{projectId}/teams`
- **Endpoints**:
  - `GET /` - List all teams in project
  - `GET /{id}` - Get team details
  - `POST /` - Create new team
  - `PUT /{id}` - Update team
  - `DELETE /{id}` - Delete team

#### ReleasesController (`backend/NotJira.Api/Controllers/ReleasesController.cs`)
- **Route**: `/api/projects/{projectId}/releases`
- **Endpoints**:
  - `GET /` - List all releases in project
  - `GET /{id}` - Get release details
  - `POST /` - Create new release
  - `PUT /{id}` - Update release
  - `DELETE /{id}` - Delete release

#### BacklogController (`backend/NotJira.Api/Controllers/BacklogController.cs`)
- **Route**: `/api/projects/{projectId}/backlog`
- **Endpoint**: `GET /` with query parameters
- **Features**:
  - Returns sprints (oldest to newest) with their items
  - Returns backlog items (not in any sprint) at the bottom
  - **Filtering support**:
    - `teamId`: Filter by team
    - `assigneeId`: Filter by assignee
    - `releaseId`: Filter by release
    - `epicId`: Filter by epic
  - Includes both Stories and Spikes
  - Each item includes epic, team, release, and assignee information

### Enhanced SprintsController
- **New Endpoints**:
  - `GET /{id}/planning` - Get sprint planning data
  - `PUT /{id}/planning` - Update Planning 1 and Planning 2 (per team)
  - `PUT /{id}/review` - Update sprint review notes
  - `PUT /{id}/retro` - Update sprint retrospective notes

### Database Migration
- **Migration**: `20260222023815_AddTeamReleaseAndLeSS`
- **Changes**:
  - Created Teams table
  - Created Releases table
  - Created TeamPlannings table
  - Added TeamId, ReleaseId, AssigneeId, AssigneeName to Stories and Spikes
  - Added PlanningOneNotes, ReviewNotes, RetroNotes to Sprints

## Frontend Changes

### BacklogView Component (`frontend/src/views/BacklogView.vue`)

#### Main Features

1. **Backlog Display**
   - **Sprint sections** (oldest to newest):
     - Sprint header with name, status, and date range
     - Sprint goal display
     - Quick access to Planning, Review, and Retro dialogs
     - DataTable showing all stories and spikes in the sprint
   - **Backlog section** at the bottom:
     - All items not assigned to any sprint
     - Same table format as sprint sections

2. **Filtering System**
   - Team filter (dropdown)
   - Assignee filter (dropdown from project members)
   - Release filter (dropdown)
   - Epic filter (dropdown)
   - Real-time filtering as selections change

3. **Sprint Management**
   - **Create Sprint dialog**:
     - Name, Goal, Start Date, End Date
     - Status (Planned, Active, Completed)
   
4. **LeSS Planning Dialogs**

   **Sprint Planning Dialog**:
   - **Planning 1 (Overall)**: Combined planning session notes
   - **Planning 2 (Per Team)**: Individual text areas for each team's planning
   - Saves all planning data in one operation
   
   **Sprint Review Dialog**:
   - Single combined review for all teams
   - Large text area for comprehensive notes
   
   **Sprint Retro Dialog**:
   - Single combined retrospective for all teams
   - Large text area for retrospective notes

5. **Team Management Dialog**
   - List all teams
   - Add new teams (name, description)
   - Delete teams
   - Accessed via "Manage Teams" button

6. **Release Management Dialog**
   - List all releases
   - Add new releases (name, description, dates, status)
   - Delete releases
   - Accessed via "Manage Releases" button

#### UI Components Used
- PrimeVue DataTable for displaying work items
- Dialog components for all management functions
- Dropdown filters for multi-dimensional filtering
- Tag components for status, priority, and type display
- DatePicker for date selection
- Textarea for notes and descriptions

### Router Updates
- Added route: `/projects/:id/backlog` → BacklogView

### ProjectDetailView Updates
- Added "Backlog" tab with description and navigation button
- Icon: `pi pi-list` (purple color)
- Description mentions filtering and LeSS capabilities

## LeSS Methodology Support

### Planning
1. **Planning 1**: Overall planning session with all teams
   - Stored in `Sprint.PlanningOneNotes`
   - Combined session for setting sprint goal and coordinating teams

2. **Planning 2**: Per-team detailed planning
   - Stored in `TeamPlanning.PlanningTwoNotes` (one per team)
   - Each team plans their own work in detail

### Review
- **Combined Review**: All teams present together
- Stored in `Sprint.ReviewNotes`
- Promotes knowledge sharing across teams

### Retrospective
- **Combined Retro**: All teams reflect together
- Stored in `Sprint.RetroNotes`
- Enables cross-team learning and improvement

## Data Flow

### Backlog Loading
1. Frontend calls `GET /api/projects/{projectId}/backlog?filters`
2. Backend queries Stories and Spikes with all related data (Epic, Team, Release, Sprint)
3. Backend groups items by Sprint and separates unassigned items
4. Frontend displays sprints in order, then backlog items at bottom

### Sprint Planning Workflow
1. User clicks "Planning" on a sprint
2. Frontend loads existing planning data via `GET /sprints/{id}/planning`
3. User edits Planning 1 notes and each team's Planning 2 notes
4. User saves via `PUT /sprints/{id}/planning`
5. Backend updates Sprint and creates/updates TeamPlanning records

### Filtering
- All filters are optional and cumulative (AND logic)
- Filters are applied server-side for efficiency
- Results update immediately when filters change

## Key Features Summary

✅ **Team Management**: Create and manage development teams
✅ **Release Planning**: Organize work into releases
✅ **Sprint Backlog**: View all work grouped by sprints
✅ **Backlog Items**: See unassigned items at the bottom
✅ **Multi-dimensional Filtering**: Team, Assignee, Release, Epic
✅ **LeSS Planning 1**: Combined planning session notes
✅ **LeSS Planning 2**: Per-team detailed planning notes
✅ **Combined Review**: Shared sprint review across teams
✅ **Combined Retro**: Shared retrospective across teams
✅ **Assignee Tracking**: Track who is working on what
✅ **Sprint Ordering**: Oldest to newest display

## Technical Stack

### Backend
- .NET 10 with ASP.NET Core
- Entity Framework Core
- PostgreSQL database
- RESTful API with authorization

### Frontend
- Vue 3 with Composition API
- TypeScript
- PrimeVue UI components
- Axios for API calls
- Vue Router for navigation

## Testing

### Verified
- ✅ Backend builds successfully
- ✅ Frontend builds successfully
- ✅ Database migration applied
- ✅ API endpoints respond (with auth required)
- ✅ Docker containers run successfully

### Recommended Testing
- Create teams and releases via the UI
- Assign stories/spikes to teams, assignees, and releases
- Create sprints and add items to them
- Test filtering combinations
- Test sprint planning, review, and retro workflows
- Verify data persistence across sessions

## Future Enhancements

Potential improvements for future iterations:
- Drag-and-drop for moving items between sprints
- Sprint velocity tracking
- Burndown charts
- Team capacity planning
- Sprint commitment tracking
- Work item dependencies
- Board view (Kanban-style)
- Time tracking integration
