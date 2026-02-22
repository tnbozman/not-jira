# Technical Decision: External Entities & Graph Visualization

## Problem Statement
We need to add external entities (people, clients) to projects who have problems, outcomes, and success metrics. Product managers need to conduct interviews for product discovery, track feedback, and visualize relationships between concepts.

## Key Requirements
1. Register external entities (people, clients)
2. Record problems, outcomes, and success metrics
3. Conduct interviews and store feedback
4. Follow-up clarifications
5. Graph visualization to view correlations
6. Deduplication system to prevent similar concepts

## Technology Decision: PostgreSQL + Cytoscape.js

### Decision
Use **PostgreSQL** for data storage with proper relational modeling and **Cytoscape.js** for graph visualization in the frontend.

### Rationale

#### Why PostgreSQL (vs RDF/Fuseki):
1. **Already in use** - Leverage existing infrastructure and team knowledge
2. **Proven stack** - Team is familiar with EF Core and SQL
3. **Simpler to implement** - No need to introduce new triple-store technology
4. **Good enough** - Can model relationships effectively with foreign keys and junction tables
5. **Performance** - Excellent for transactional queries and standard CRUD operations
6. **Extensibility** - PostgreSQL supports graph extensions (Apache AGE) if needed later
7. **Tooling** - Rich ecosystem of ORMs, admin tools, and visualization connectors

#### Why Cytoscape.js (for visualization):
1. **Mature & well-maintained** - Active development and large community
2. **Performant** - Handles 100,000+ nodes/edges
3. **Feature-rich** - Multiple layout algorithms, filtering, styling
4. **Easy integration** - Works well with Vue.js and JSON data from REST APIs
5. **Open source** - No licensing costs

### Data Model

#### Core Entities
- **ExternalEntity**: Base entity for people and clients
  - Type: Person | Client
  - Name, email, organization
  - Tags for deduplication

- **Problem**: Issues identified by external entities
  - Description, severity
  - Link to external entity
  - Tags for deduplication

- **Outcome**: Desired outcomes
  - Description, priority
  - Link to external entity and problems

- **SuccessMetric**: Measurable success criteria
  - Description, target value, current value
  - Link to outcomes

- **Interview**: Product discovery sessions
  - Date, interviewer, interviewee
  - Type: Discovery | Feedback | Clarification
  - Notes

- **InterviewNote**: Detailed notes from interviews
  - Link to interview
  - Related to problems, outcomes, or entities

- **Tag/Keyword**: For deduplication and categorization
  - Name, type
  - Many-to-many with entities, problems, outcomes

### Graph Relationships
- External Entity → has many → Problems
- Problems → lead to → Outcomes
- Outcomes → measured by → Success Metrics
- Interviews → discover → Problems/Outcomes
- Tags → categorize → all entities

### Migration Path to RDF (Future)
If we later need:
- Complex semantic inference
- Cross-system data integration
- More advanced ontologies

We can export PostgreSQL data to RDF/Fuseki without major refactoring of the application logic.

## Implementation Approach

### Phase 1: Backend Models & API
1. Create entity models in C#
2. Add EF Core migrations
3. Create REST API controllers
4. Add endpoints for graph data retrieval

### Phase 2: Frontend Views & Services
1. Create TypeScript services for API calls
2. Build entity management views
3. Create interview recording interface
4. Add tag/keyword management

### Phase 3: Graph Visualization
1. Integrate Cytoscape.js
2. Create graph view component
3. Fetch and transform data for visualization
4. Add interactive filtering and exploration

### Phase 4: Deduplication
1. Tag-based categorization
2. Search functionality to find similar entities
3. Manual merging tools for product managers

## Conclusion
This approach provides a pragmatic solution that:
- Leverages existing technology stack
- Delivers required functionality
- Maintains simplicity and performance
- Allows future evolution if needed

Date: February 22, 2026
