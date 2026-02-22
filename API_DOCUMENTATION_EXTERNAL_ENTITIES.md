# External Entities API Documentation

This document describes the API endpoints for managing external entities, problems, outcomes, interviews, and knowledge graph visualization.

## Base URL

```
http://localhost:8080/api
```

All endpoints require JWT authentication via the `Authorization: Bearer <token>` header.

## Endpoints

### External Entities

#### List External Entities
```http
GET /api/projects/{projectId}/external-entities
```

Returns all external entities for a project.

**Response:**
```json
[
  {
    "id": 1,
    "type": "Person",
    "name": "John Doe",
    "email": "john@example.com",
    "organization": "Acme Corp",
    "phone": "+1234567890",
    "notes": "Key stakeholder",
    "projectId": 1,
    "problems": [...],
    "interviews": [...],
    "entityTags": [...],
    "createdAt": "2024-02-22T00:00:00Z",
    "updatedAt": "2024-02-22T00:00:00Z"
  }
]
```

#### Get External Entity
```http
GET /api/projects/{projectId}/external-entities/{id}
```

Returns detailed information about a specific external entity including related problems and interviews.

#### Create External Entity
```http
POST /api/projects/{projectId}/external-entities
```

**Request Body:**
```json
{
  "type": "Person",
  "name": "John Doe",
  "email": "john@example.com",
  "organization": "Acme Corp",
  "phone": "+1234567890",
  "notes": "Key stakeholder"
}
```

**Response:** Created entity object

#### Update External Entity
```http
PUT /api/projects/{projectId}/external-entities/{id}
```

**Request Body:** Same as create

#### Delete External Entity
```http
DELETE /api/projects/{projectId}/external-entities/{id}
```

### Problems

#### List Problems
```http
GET /api/projects/{projectId}/problems
```

Returns all problems for a project with related entities and outcomes.

**Response:**
```json
[
  {
    "id": 1,
    "description": "Difficulty tracking customer feedback",
    "severity": "High",
    "context": "Customers complain about lack of visibility",
    "externalEntityId": 1,
    "externalEntity": {...},
    "outcomes": [...],
    "problemTags": [...],
    "createdAt": "2024-02-22T00:00:00Z",
    "updatedAt": "2024-02-22T00:00:00Z"
  }
]
```

#### Get Problem
```http
GET /api/projects/{projectId}/problems/{id}
```

#### Create Problem
```http
POST /api/projects/{projectId}/problems
```

**Request Body:**
```json
{
  "description": "Difficulty tracking customer feedback",
  "severity": "High",
  "context": "Customers complain about lack of visibility",
  "externalEntityId": 1
}
```

**Severity levels:** `Low`, `Medium`, `High`, `Critical`

#### Update Problem
```http
PUT /api/projects/{projectId}/problems/{id}
```

#### Delete Problem
```http
DELETE /api/projects/{projectId}/problems/{id}
```

### Interviews

#### List Interviews
```http
GET /api/projects/{projectId}/interviews
```

Returns all interviews for a project, ordered by date descending.

**Response:**
```json
[
  {
    "id": 1,
    "type": "Discovery",
    "interviewDate": "2024-02-22T10:00:00Z",
    "interviewer": "Jane Smith",
    "summary": "Initial discovery session",
    "externalEntityId": 1,
    "projectId": 1,
    "externalEntity": {...},
    "notes": [...],
    "createdAt": "2024-02-22T00:00:00Z",
    "updatedAt": "2024-02-22T00:00:00Z"
  }
]
```

#### Get Interview
```http
GET /api/projects/{projectId}/interviews/{id}
```

Returns interview with all notes and related problems/outcomes.

#### Create Interview
```http
POST /api/projects/{projectId}/interviews
```

**Request Body:**
```json
{
  "type": "Discovery",
  "interviewDate": "2024-02-22T10:00:00Z",
  "interviewer": "Jane Smith",
  "summary": "Initial discovery session",
  "externalEntityId": 1
}
```

**Interview types:** `Discovery`, `Feedback`, `Clarification`

#### Update Interview
```http
PUT /api/projects/{projectId}/interviews/{id}
```

#### Delete Interview
```http
DELETE /api/projects/{projectId}/interviews/{id}
```

#### Add Interview Note
```http
POST /api/projects/{projectId}/interviews/{id}/notes
```

**Request Body:**
```json
{
  "content": "Customer mentioned issues with mobile app",
  "relatedProblemId": 1,
  "relatedOutcomeId": null
}
```

### Tags

#### List Tags
```http
GET /api/projects/{projectId}/tags
```

Returns all tags for a project.

**Response:**
```json
[
  {
    "id": 1,
    "name": "mobile",
    "description": "Mobile-related issues",
    "projectId": 1,
    "createdAt": "2024-02-22T00:00:00Z"
  }
]
```

#### Get Tag
```http
GET /api/projects/{projectId}/tags/{id}
```

#### Create Tag
```http
POST /api/projects/{projectId}/tags
```

**Request Body:**
```json
{
  "name": "mobile",
  "description": "Mobile-related issues"
}
```

#### Delete Tag
```http
DELETE /api/projects/{projectId}/tags/{id}
```

#### Search Tags
```http
GET /api/projects/{projectId}/tags/search?query=mobile
```

Returns up to 20 tags matching the search query.

### Knowledge Graph

#### Get Graph Data
```http
GET /api/projects/{projectId}/graph
```

Returns complete graph data for visualization including nodes, edges, and statistics.

**Response:**
```json
{
  "nodes": [
    {
      "id": "entity-1",
      "label": "John Doe",
      "type": "entity",
      "data": {
        "id": 1,
        "type": "Person",
        "email": "john@example.com",
        "organization": "Acme Corp",
        "tags": ["customer", "key-stakeholder"]
      }
    },
    {
      "id": "problem-1",
      "label": "Difficulty tracking customer feedback",
      "type": "problem",
      "data": {
        "id": 1,
        "description": "Difficulty tracking customer feedback",
        "severity": "High",
        "tags": ["feedback", "tracking"]
      }
    },
    {
      "id": "outcome-1",
      "label": "Improved feedback visibility",
      "type": "outcome",
      "data": {
        "id": 1,
        "description": "Improved feedback visibility",
        "priority": "High",
        "successMetrics": 2,
        "tags": ["visibility", "reporting"]
      }
    }
  ],
  "edges": [
    {
      "id": "entity-1-problem-1",
      "source": "entity-1",
      "target": "problem-1",
      "label": "has problem"
    },
    {
      "id": "problem-1-outcome-1",
      "source": "problem-1",
      "target": "outcome-1",
      "label": "leads to"
    }
  ],
  "stats": {
    "entityCount": 5,
    "problemCount": 12,
    "outcomeCount": 8,
    "interviewCount": 3
  }
}
```

**Node Types:**
- `entity`: External entity (Person or Client)
- `problem`: Problem identified
- `outcome`: Desired outcome
- `metric`: Success metric
- `interview`: Interview session

**Edge Labels:**
- `has problem`: Entity to Problem
- `leads to`: Problem to Outcome
- `measured by`: Outcome to Success Metric
- `participated in`: Entity to Interview

## Data Models

### EntityType
- `Person`: Individual stakeholder
- `Client`: Organization or client entity

### Severity
- `Low`: Minor issue
- `Medium`: Moderate issue
- `High`: Important issue
- `Critical`: Urgent issue requiring immediate attention

### Priority
- `Low`: Nice to have
- `Medium`: Should have
- `High`: Must have
- `Critical`: Critical business need

### InterviewType
- `Discovery`: Initial product discovery session
- `Feedback`: Feedback on existing features
- `Clarification`: Follow-up clarification session

## Error Responses

All endpoints may return the following error responses:

**401 Unauthorized**
```json
{
  "error": "Unauthorized",
  "message": "Authentication required"
}
```

**404 Not Found**
```json
{
  "error": "Not Found",
  "message": "Project not found"
}
```

**400 Bad Request**
```json
{
  "error": "Bad Request",
  "message": "Invalid input data"
}
```

**409 Conflict**
```json
{
  "error": "Conflict",
  "message": "A tag with this name already exists in this project"
}
```

## Usage Examples

### Creating a Complete Discovery Flow

1. **Create External Entity**
```bash
curl -X POST http://localhost:8080/api/projects/1/external-entities \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Person",
    "name": "Sarah Johnson",
    "email": "sarah@customer.com",
    "organization": "Customer Corp"
  }'
```

2. **Create a Problem**
```bash
curl -X POST http://localhost:8080/api/projects/1/problems \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Cannot export reports in desired format",
    "severity": "High",
    "externalEntityId": 1
  }'
```

3. **Record an Interview**
```bash
curl -X POST http://localhost:8080/api/projects/1/interviews \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Discovery",
    "interviewDate": "2024-02-22T14:00:00Z",
    "interviewer": "Product Manager",
    "summary": "Initial discovery about reporting needs",
    "externalEntityId": 1
  }'
```

4. **Add Tags for Deduplication**
```bash
curl -X POST http://localhost:8080/api/projects/1/tags \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "reporting",
    "description": "Reporting and export features"
  }'
```

5. **View Knowledge Graph**
```bash
curl http://localhost:8080/api/projects/1/graph \
  -H "Authorization: Bearer <token>"
```

## Best Practices

1. **Use Tags for Deduplication**: Before creating new problems or outcomes, search existing tags to ensure similar concepts aren't duplicated.

2. **Link Interview Notes**: Always link interview notes to related problems or outcomes for better graph visualization.

3. **Complete Entity Information**: Provide as much information as possible about external entities to enable better tracking and analysis.

4. **Regular Graph Reviews**: Use the knowledge graph endpoint regularly to identify patterns and correlations between problems and outcomes.

5. **Severity and Priority**: Use consistent severity and priority levels across the team for better prioritization.
