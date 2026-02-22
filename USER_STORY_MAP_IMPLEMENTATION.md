# User Story Map Implementation

## Overview

This implementation adds an outcome-focused user story map based on Jeff Patton's methodology to the Not-JIRA system. The user story map supports outcome-focused themes, epics, stories, and spikes, all linked to the existing Outcome system for full traceability from solutions back to entity problems.

## Features Implemented

### 1. Data Model (Backend)

#### Theme
- Represents the product backbone at the highest level
- Outcome-focused: Can be linked to an Outcome to trace the business value
- Contains multiple Epics
- Fields:
  - `Name`: Theme name
  - `Description`: Detailed description
  - `Order`: Display order in the story map
  - `OutcomeId`: Optional link to Outcome
  - `ProjectId`: Link to Project

#### Epic
- Decomposed from Themes
- Outcome-focused: Can be linked to an Outcome
- Contains multiple Stories and Spikes
- Fields:
  - `Name`: Epic name
  - `Description`: Detailed description
  - `Order`: Display order within theme
  - `ThemeId`: Parent theme
  - `OutcomeId`: Optional link to Outcome

#### Story
- User stories decomposed from Epics
- Outcome-focused: Can be linked to an Outcome
- Supports solution details and acceptance criteria
- Can be assigned to Sprints
- Fields:
  - `Title`: Story title
  - `Description`: Story description
  - `SolutionDescription`: Details of the solution approach
  - `AcceptanceCriteria`: Acceptance criteria for the story
  - `Order`: Display order within epic
  - `Priority`: Low, Medium, High, Critical
  - `Status`: Backlog, To Do, In Progress, In Review, Done
  - `StoryPoints`: Effort estimation
  - `EpicId`: Parent epic
  - `SprintId`: Optional sprint assignment
  - `OutcomeId`: Optional link to Outcome

#### Spike
- Research/investigation tasks decomposed from Epics
- Outcome-focused: Can be linked to an Outcome
- Supports investigation goals and findings
- Can be assigned to Sprints
- Fields:
  - `Title`: Spike title
  - `Description`: Spike description
  - `InvestigationGoal`: What needs to be investigated
  - `Findings`: Results of the investigation
  - `Order`: Display order within epic
  - `Priority`: Low, Medium, High, Critical
  - `Status`: Backlog, To Do, In Progress, In Review, Done
  - `StoryPoints`: Effort estimation
  - `EpicId`: Parent epic
  - `SprintId`: Optional sprint assignment
  - `OutcomeId`: Optional link to Outcome

#### Sprint
- Supports backlog management and sprint planning
- Stories and Spikes can be assigned to sprints
- Fields:
  - `Name`: Sprint name
  - `Goal`: Sprint goal
  - `StartDate`: Sprint start date
  - `EndDate`: Sprint end date
  - `Status`: Planned, Active, Completed
  - `ProjectId`: Link to Project

### 2. API Endpoints

All endpoints are protected with JWT authentication via Keycloak.

#### Themes
- `GET /api/projects/{projectId}/themes` - List all themes for a project
- `GET /api/projects/{projectId}/themes/{id}` - Get a specific theme
- `POST /api/projects/{projectId}/themes` - Create a new theme
- `PUT /api/projects/{projectId}/themes/{id}` - Update a theme
- `DELETE /api/projects/{projectId}/themes/{id}` - Delete a theme

#### Epics
- `GET /api/projects/{projectId}/themes/{themeId}/epics` - List all epics for a theme
- `GET /api/projects/{projectId}/themes/{themeId}/epics/{id}` - Get a specific epic
- `POST /api/projects/{projectId}/themes/{themeId}/epics` - Create a new epic
- `PUT /api/projects/{projectId}/themes/{themeId}/epics/{id}` - Update an epic
- `DELETE /api/projects/{projectId}/themes/{themeId}/epics/{id}` - Delete an epic

#### Stories
- `GET /api/projects/{projectId}/epics/{epicId}/stories` - List all stories for an epic
- `GET /api/projects/{projectId}/epics/{epicId}/stories/{id}` - Get a specific story
- `POST /api/projects/{projectId}/epics/{epicId}/stories` - Create a new story
- `PUT /api/projects/{projectId}/epics/{epicId}/stories/{id}` - Update a story
- `DELETE /api/projects/{projectId}/epics/{epicId}/stories/{id}` - Delete a story

#### Spikes
- `GET /api/projects/{projectId}/epics/{epicId}/spikes` - List all spikes for an epic
- `GET /api/projects/{projectId}/epics/{epicId}/spikes/{id}` - Get a specific spike
- `POST /api/projects/{projectId}/epics/{epicId}/spikes` - Create a new spike
- `PUT /api/projects/{projectId}/epics/{epicId}/spikes/{id}` - Update a spike
- `DELETE /api/projects/{projectId}/epics/{epicId}/spikes/{id}` - Delete a spike

#### Sprints
- `GET /api/projects/{projectId}/sprints` - List all sprints for a project
- `GET /api/projects/{projectId}/sprints/{id}` - Get a specific sprint
- `POST /api/projects/{projectId}/sprints` - Create a new sprint
- `PUT /api/projects/{projectId}/sprints/{id}` - Update a sprint
- `DELETE /api/projects/{projectId}/sprints/{id}` - Delete a sprint

### 3. Frontend Components

#### User Story Map View (`/projects/{id}/story-map`)
A comprehensive view that displays:
- Themes in a horizontal row at the top level
- Epics as cards within each theme
- Stories and Spikes as work items within each epic
- Visual indicators for:
  - Work item type (Story vs Spike)
  - Status
  - Story points
  - Sprint assignment
  - Linked outcomes

Features:
- Add/Edit/Delete Themes, Epics, Stories, and Spikes
- Inline forms with validation
- Sprint management dialog
- Visual differentiation between stories and spikes
- Outcome tags showing traceability

#### User Story Map Service
A TypeScript service providing:
- Full CRUD operations for all entities
- Type-safe interfaces
- JWT authentication integration
- Error handling

### 4. Outcome Traceability

The implementation supports full traceability:
1. **External Entities** have **Problems**
2. **Problems** link to **Outcomes**
3. **Outcomes** have **Success Metrics**
4. **Themes** can link to **Outcomes** (outcome-focused themes)
5. **Epics** can link to **Outcomes** (outcome-focused epics)
6. **Stories** can link to **Outcomes** AND have **SolutionDescription** (tracing solutions to outcomes)
7. **Spikes** can link to **Outcomes** AND have **InvestigationGoal** and **Findings**

This creates a complete chain:
```
External Entity → Problem → Outcome → Theme/Epic/Story/Spike → Solution
                           ↓
                    Success Metric
```

### 5. Sprint Planning Support

Stories and Spikes can be:
- Created in the Backlog (no sprint assignment)
- Assigned to specific sprints
- Moved between sprints by editing
- Tracked with story points
- Monitored with status updates

Sprints provide:
- Time-boxed iterations
- Sprint goals
- Start and end dates
- Status tracking (Planned, Active, Completed)

## Database Schema

The migration `20260222014304_AddUserStoryMap` creates the following tables:
- `Themes` - Product backbone themes
- `Epics` - Epics within themes
- `Stories` - User stories within epics
- `Spikes` - Investigation spikes within epics
- `Sprints` - Sprint planning

All tables include proper foreign keys, indexes, and cascade delete rules.

## Navigation

The User Story Map is accessible from:
1. Project Detail View → "User Story Map" tab
2. Direct URL: `/projects/{projectId}/story-map`

## Key Design Decisions

1. **Hierarchical Structure**: Themes → Epics → Stories/Spikes follows Jeff Patton's methodology
2. **Outcome Focus**: Every level can link to outcomes for traceability
3. **Solution Details**: Stories have dedicated fields for solution descriptions and acceptance criteria
4. **Sprint Integration**: Backlog items can be assigned to sprints for agile planning
5. **Priority & Status**: Stories and Spikes track priority and status independently
6. **Story Points**: Support for effort estimation at story and spike level

## Future Enhancements

Potential additions:
- Drag-and-drop reordering within the story map
- Bulk operations (move multiple stories to a sprint)
- Outcome visualization in the story map
- Release planning across sprints
- Velocity tracking per sprint
- Burndown charts
- Integration with the existing task system
- Story templates
- Epic progress visualization

## Testing

To test the implementation:

1. Start the application with Docker Compose
2. Log in with test credentials
3. Navigate to a project
4. Click "User Story Map" tab
5. Create themes, epics, stories, and spikes
6. Link them to existing outcomes (created via External Entities)
7. Create sprints and assign work items
8. Verify traceability from Entity → Problem → Outcome → Story/Spike → Solution

## Technical Stack

- **Backend**: .NET 10, Entity Framework Core, PostgreSQL
- **Frontend**: Vue 3, TypeScript, PrimeVue
- **Authentication**: Keycloak with JWT
- **API**: RESTful with route-based resource hierarchy
