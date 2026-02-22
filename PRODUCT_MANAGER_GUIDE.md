# Product Manager Guide: External Entities & Knowledge Graph

## Overview

The External Entities feature helps product managers track stakeholders (people and clients), their problems, desired outcomes, and success metrics. It supports conducting product discovery interviews and visualizing relationships through an interactive knowledge graph.

## Key Concepts

### External Entities
External entities are people or organizations outside your development team who have problems you're trying to solve. They can be:
- **People**: Individual stakeholders, users, or customers
- **Clients**: Organizations or companies

### Problems
Issues, pain points, or challenges that external entities face. Each problem has:
- Description
- Severity (Low, Medium, High, Critical)
- Context (additional details)
- Tags for categorization

### Outcomes
Desired results or goals that address problems. Each outcome has:
- Description
- Priority (Low, Medium, High, Critical)
- Context
- Success Metrics
- Tags for categorization

### Success Metrics
Measurable criteria that indicate whether an outcome has been achieved. Each metric includes:
- Description
- Target value
- Current value
- Unit of measurement

### Interviews
Structured sessions for gathering information. Three types:
- **Discovery**: Initial exploration of problems and needs
- **Feedback**: Gathering feedback on solutions or features
- **Clarification**: Follow-up sessions to clarify requirements

### Tags
Keywords that help categorize and deduplicate similar concepts across entities, problems, and outcomes.

## Getting Started

### 1. Access External Entities

1. Navigate to your project
2. Click on the "External Entities" tab
3. Click "Add Entity" to create your first external entity

### 2. Create an External Entity

Fill in the entity details:
- **Type**: Choose Person or Client
- **Name**: Full name or organization name
- **Email**: Contact email
- **Organization**: Company or organization (for people)
- **Phone**: Contact number (optional)
- **Notes**: Any additional information

**Example:**
```
Type: Person
Name: Sarah Johnson
Email: sarah@techcorp.com
Organization: TechCorp Inc.
Notes: Head of Product, primary stakeholder for analytics features
```

### 3. Record Problems

Problems can be added directly from the entity detail view or from the Problems section.

1. View an entity
2. Click "Add Problem"
3. Enter:
   - Description (clear, specific problem statement)
   - Severity level
   - Context (background information)
   - Tags (e.g., "reporting", "mobile", "performance")

**Example:**
```
Description: Cannot export analytics reports to Excel
Severity: High
Context: Users need to share data with stakeholders who don't have system access
Tags: reporting, export, analytics
```

### 4. Conduct Interviews

Record discovery sessions, feedback interviews, and clarifications:

1. Navigate to the Interviews section
2. Click "New Interview"
3. Fill in:
   - Type (Discovery/Feedback/Clarification)
   - Date and time
   - Interviewer name
   - Summary

4. Add notes during or after the interview:
   - Each note can be linked to a specific problem or outcome
   - This creates connections in the knowledge graph

**Interview Note Example:**
```
Content: "Sarah mentioned that monthly reports take 2 hours to compile manually. 
This affects her team's productivity significantly."
Related Problem: Cannot export analytics reports to Excel
```

### 5. Define Outcomes

Based on identified problems, create desired outcomes:

1. Add an outcome related to a problem
2. Define success metrics
3. Tag for easy discovery

**Example:**
```
Outcome: Automated analytics report generation
Priority: High
Context: Enable one-click export of analytics data

Success Metrics:
- Report generation time < 5 seconds
- Export to Excel, PDF, CSV formats
- 90% user satisfaction score
```

## Using the Knowledge Graph

The knowledge graph visualizes relationships between entities, problems, outcomes, and interviews.

### Accessing the Graph

1. Navigate to your project
2. Click on the "Knowledge Graph" tab
3. The graph will load automatically

### Understanding the Visualization

**Node Colors:**
- **Blue**: External Entities (People/Clients)
- **Red**: Problems
- **Green**: Outcomes
- **Purple**: Success Metrics
- **Amber**: Interviews

**Edge Labels:**
- "has problem": Entity → Problem
- "leads to": Problem → Outcome
- "measured by": Outcome → Success Metric
- "participated in": Entity → Interview

### Interacting with the Graph

- **Click** nodes to see details
- **Drag** nodes to rearrange the layout
- **Zoom** using mouse wheel or pinch gesture
- **Pan** by dragging the background

### Finding Patterns

Use the graph to:
1. **Identify clusters** of related problems
2. **Find common themes** across multiple entities
3. **Discover dependencies** between outcomes
4. **Track interview coverage** of different problems

## Preventing Duplication with Tags

Tags help you avoid creating duplicate problems or outcomes with different wording.

### Best Practices for Tagging

1. **Search before creating**: Use tag search to find existing related tags
2. **Use consistent terminology**: Establish a tagging convention with your team
3. **Be specific but reusable**: Tags like "mobile-performance" are better than "issue-123"
4. **Review regularly**: Periodically review and merge similar tags

### Tag Search

Before creating a new problem or outcome:
1. Go to Tags section
2. Search for related keywords
3. Review existing tags and tagged items
4. Reuse existing tags when appropriate

**Example Workflow:**
```
1. Search tags for "export"
2. Find existing tag: "export-functionality"
3. See 3 existing problems with this tag
4. Review to avoid duplication
5. Use same tag for new problem if related
```

## Product Discovery Workflow

### Complete Discovery Process

1. **Identify Stakeholder**
   - Create external entity
   - Add contact information
   - Note their role and context

2. **Schedule Interview**
   - Create interview record
   - Set type to "Discovery"
   - Add interviewer name

3. **Conduct Interview**
   - Ask open-ended questions
   - Listen for problems and pain points
   - Note desired outcomes

4. **Document Findings**
   - Add problems as you discover them
   - Link interview notes to problems
   - Tag problems for categorization

5. **Define Outcomes**
   - Create outcomes based on problems
   - Link outcomes to related problems
   - Define success metrics

6. **Review Graph**
   - Visualize the knowledge graph
   - Identify patterns and themes
   - Find gaps in understanding

7. **Follow-up**
   - Schedule clarification interviews
   - Verify understanding
   - Refine outcomes and metrics

## Tips for Product Managers

### During Discovery Interviews

- **Focus on problems, not solutions**: Ask "What problem are you trying to solve?" not "What feature do you want?"
- **Capture context**: Note the environment, frequency, and impact of problems
- **Link everything**: Connect interview notes to problems and outcomes for better tracking
- **Tag immediately**: Add tags during the interview to capture themes while fresh

### Managing Your Knowledge Base

- **Regular reviews**: Review the knowledge graph weekly to spot trends
- **Update metrics**: Track current values for success metrics as you progress
- **Archive outdated items**: Remove or archive problems that have been solved
- **Share insights**: Use the graph view in stakeholder presentations

### Collaboration

- **Consistent tagging**: Establish team conventions for tags
- **Clear descriptions**: Write problem descriptions that others can understand
- **Document context**: Include enough detail for someone else to understand
- **Link related items**: Connect problems to outcomes and metrics

## Common Use Cases

### 1. New Product Feature Discovery

```
1. Create entities for 5 target users
2. Conduct discovery interviews with each
3. Record problems and tag by theme
4. Review graph to identify most common problems
5. Define outcomes addressing top problems
6. Create success metrics for each outcome
```

### 2. Feature Feedback Collection

```
1. Schedule feedback interviews with existing users
2. Record feedback as interview notes
3. Link notes to existing problems or create new ones
4. Update success metrics with current values
5. Use graph to see which outcomes are being achieved
```

### 3. Stakeholder Prioritization

```
1. Create entities for all stakeholders
2. Map their problems in the system
3. Use graph to visualize problem frequency
4. Tag problems by business impact
5. Prioritize outcomes based on problem severity and frequency
```

### 4. Theme Identification

```
1. Tag all problems as they're discovered
2. Use tag search to group related problems
3. Review graph to see problem clusters
4. Define themes based on problem groups
5. Create outcomes aligned with themes
```

## Troubleshooting

### "I can't find a problem I created"
- Check if you're in the correct project
- Use tag search to find related items
- Check the graph view for visual discovery

### "The graph looks cluttered"
- Filter by specific entity types
- Focus on high-priority items
- Review and archive solved problems

### "I'm creating duplicate problems"
- Always search tags before creating
- Review similar entities for existing problems
- Use the graph to find related items

### "Interview notes aren't showing up"
- Ensure you've linked notes to the interview
- Check that the interview belongs to the correct project
- Refresh the page

## Future Enhancements

The following features are planned for future releases:

- **Theme mapping**: Link problems to epics and themes
- **Outcome tracking**: Progress dashboard for success metrics
- **Export capabilities**: Export graph data for presentations
- **Advanced search**: Full-text search across all entities
- **Notifications**: Alerts for interview follow-ups

## Support

For questions or issues:
1. Check this guide
2. Review the API documentation
3. Contact your system administrator
4. Submit feedback through the project

---

**Last Updated**: February 22, 2026
**Version**: 1.0
