# External Entities System Architecture

## Data Model Relationships

```
┌─────────────────────┐
│      Project        │
└──────────┬──────────┘
           │
           │ 1:N
           │
           ▼
┌─────────────────────┐         ┌─────────────────────┐
│  External Entity    │────────▶│      Problem        │
│  (Person/Client)    │  1:N    │  (with Severity)    │
└──────────┬──────────┘         └──────────┬──────────┘
           │                                │
           │ 1:N                            │ N:M
           │                                │
           ▼                                ▼
┌─────────────────────┐         ┌─────────────────────┐
│     Interview       │         │      Outcome        │
│ (Discovery/Feedback)│         │  (with Priority)    │
└──────────┬──────────┘         └──────────┬──────────┘
           │                                │
           │ 1:N                            │ 1:N
           │                                │
           ▼                                ▼
┌─────────────────────┐         ┌─────────────────────┐
│   InterviewNote     │         │   Success Metric    │
│  (linked to P/O)    │         │  (Target/Current)   │
└─────────────────────┘         └─────────────────────┘

┌─────────────────────┐
│        Tag          │◀────────┐
│  (Deduplication)    │         │
└─────────────────────┘         │
           ▲                    │ N:M
           │                    │
           └────────────────────┘
     (via EntityTag, ProblemTag, OutcomeTag)
```

## API Structure

```
/api/projects/{projectId}
    │
    ├── /external-entities
    │   ├── GET     - List all entities
    │   ├── POST    - Create entity
    │   ├── /{id}
    │       ├── GET    - Get entity details
    │       ├── PUT    - Update entity
    │       └── DELETE - Delete entity
    │
    ├── /problems
    │   ├── GET     - List all problems
    │   ├── POST    - Create problem
    │   ├── /{id}
    │       ├── GET    - Get problem details
    │       ├── PUT    - Update problem
    │       └── DELETE - Delete problem
    │
    ├── /interviews
    │   ├── GET     - List all interviews
    │   ├── POST    - Create interview
    │   ├── /{id}
    │       ├── GET    - Get interview details
    │       ├── PUT    - Update interview
    │       ├── DELETE - Delete interview
    │       └── /notes
    │           └── POST - Add interview note
    │
    ├── /tags
    │   ├── GET     - List all tags
    │   ├── POST    - Create tag
    │   ├── /search?query={q} - Search tags
    │   └── /{id}
    │       ├── GET    - Get tag details
    │       └── DELETE - Delete tag
    │
    └── /graph
        └── GET     - Get complete graph data
                     (nodes + edges + stats)
```

## Frontend Navigation

```
Project Detail View
    │
    ├── Tab: Overview
    │   └── Basic project information
    │
    ├── Tab: External Entities
    │   └── Quick nav to → /projects/{id}/entities
    │       │
    │       └── External Entities View
    │           ├── DataTable of entities
    │           ├── Create/Edit Dialog
    │           └── View Details Dialog
    │               ├── Problems list
    │               └── Interviews list
    │
    ├── Tab: Knowledge Graph
    │   └── Quick nav to → /projects/{id}/graph
    │       │
    │       └── Graph View
    │           ├── Cytoscape.js visualization
    │           ├── Node legend
    │           ├── Stats bar
    │           └── Interactive controls
    │
    └── Tab: Members
        └── Project member management
```

## Knowledge Graph Visualization

```
Node Color Scheme:
┌─────────────────────────────────────────┐
│ 🔵 Blue    - External Entity            │
│ 🔴 Red     - Problem                    │
│ 🟢 Green   - Outcome                    │
│ 🟣 Purple  - Success Metric             │
│ 🟠 Amber   - Interview                  │
└─────────────────────────────────────────┘

Edge Relationships:
┌─────────────────────────────────────────┐
│ Entity ──has problem──▶ Problem         │
│ Problem ──leads to──▶ Outcome           │
│ Outcome ──measured by──▶ Metric         │
│ Entity ──participated in──▶ Interview   │
└─────────────────────────────────────────┘

Example Graph:
        [John Doe]
        🔵 Entity
            │
            │ has problem
            ▼
    [Export Issues]
      🔴 Problem
            │
            │ leads to
            ▼
   [Automated Reports]
      🟢 Outcome
            │
            │ measured by
            ├──▶ [< 5 sec]
            │    🟣 Metric
            │
            └──▶ [90% satisfaction]
                 🟣 Metric
```

## Technology Stack

```
Frontend Layer
┌───────────────────────────────────────────┐
│  Vue 3 + TypeScript + PrimeVue           │
│  ├── ExternalEntitiesView.vue           │
│  ├── GraphView.vue (Cytoscape.js)       │
│  ├── externalEntityService.ts           │
│  └── graphService.ts                     │
└───────────────┬───────────────────────────┘
                │ REST API (JSON)
                │ JWT Auth
                ▼
Backend Layer
┌───────────────────────────────────────────┐
│  .NET 10 + EF Core                       │
│  ├── ExternalEntitiesController         │
│  ├── ProblemsController                  │
│  ├── InterviewsController                │
│  ├── TagsController                      │
│  └── GraphController                     │
└───────────────┬───────────────────────────┘
                │ EF Core ORM
                │
                ▼
Database Layer
┌───────────────────────────────────────────┐
│  PostgreSQL 17                            │
│  ├── ExternalEntities                    │
│  ├── Problems                             │
│  ├── Outcomes                             │
│  ├── SuccessMetrics                       │
│  ├── Interviews                           │
│  ├── InterviewNotes                       │
│  ├── Tags                                 │
│  └── Junction Tables (EntityTag, etc.)   │
└───────────────────────────────────────────┘
```

## User Workflow

```
Product Manager Journey
────────────────────────

1. Create Project
   └─▶ Navigate to External Entities tab

2. Add External Entity
   ├─▶ Type: Person or Client
   ├─▶ Contact details
   └─▶ Notes

3. Identify Problems
   ├─▶ Document problem description
   ├─▶ Set severity level
   └─▶ Add tags for categorization

4. Conduct Interview
   ├─▶ Record interview metadata
   ├─▶ Add detailed notes
   ├─▶ Link notes to problems/outcomes
   └─▶ Tag for discovery

5. Define Outcomes
   ├─▶ Create desired outcomes
   ├─▶ Link to related problems
   ├─▶ Define success metrics
   └─▶ Set priorities

6. Visualize Knowledge Graph
   ├─▶ View entity relationships
   ├─▶ Identify problem clusters
   ├─▶ Discover patterns
   └─▶ Generate insights

7. Search & Deduplicate
   ├─▶ Search existing tags
   ├─▶ Find similar concepts
   └─▶ Reuse or create tags
```

## Deduplication Strategy

```
Tag-Based System
────────────────

Before creating new problem:
    │
    ├─▶ Search tags: "export", "reporting"
    │   └─▶ Found: 3 existing problems with "export" tag
    │
    ├─▶ Review existing problems
    │   └─▶ Similar? → Add to existing
    │       Different? → Create new with tag
    │
    └─▶ Result: Reduced duplication

Tag Reuse:
    Problem A ──[reporting]──┐
    Problem B ──[reporting]──┼─▶ Shared Tag
    Outcome C ──[reporting]──┘

Benefits:
    ✓ Find related concepts
    ✓ Group by theme
    ✓ Prevent duplicates
    ✓ Enable pattern discovery
```

## Future Architecture (RDF Migration)

```
If semantic inference needed:

PostgreSQL                    Apache Jena Fuseki
────────────                 ───────────────────
  Entities                         RDF Triples
  Problems      ─export─▶      Subject-Predicate-Object
  Outcomes                         SPARQL Queries
  
Current graph model maps cleanly to RDF:
    Entity ──hasProblem──▶ Problem  (RDF triple)
    Problem ──leadsTo──▶ Outcome    (RDF triple)
    
Migration path preserved!
```

## Security Model

```
Authentication & Authorization
──────────────────────────────

Request Flow:
    Client
      │
      │ JWT Token
      ▼
   [Authorize] Attribute
      │
      │ Valid?
      ├─▶ Yes → Controller Action
      │            │
      │            ├─▶ Check projectId
      │            │   (user has access?)
      │            │
      │            └─▶ Return data
      │                (scoped to project)
      │
      └─▶ No  → 401 Unauthorized

All endpoints require:
    ✓ Valid JWT token
    ✓ Project-scoped access
    ✓ No data leakage across projects
```

---

This visual guide provides a comprehensive overview of the external entities system architecture, data flows, and user workflows.
