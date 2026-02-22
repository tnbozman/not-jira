using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using Microsoft.AspNetCore.Authorization;

namespace StoryFirst.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId}/graph")]
[Authorize]
public class GraphController : ControllerBase
{
    private readonly AppDbContext _context;

    public GraphController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/projects/{projectId}/graph
    [HttpGet]
    public async Task<ActionResult<GraphData>> GetGraphData(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        // Fetch all entities
        var entities = await _context.ExternalEntities
            .Where(e => e.ProjectId == projectId)
            .Include(e => e.EntityTags)
                .ThenInclude(et => et.Tag)
            .ToListAsync();

        // Fetch all problems
        var problems = await _context.Problems
            .Where(p => p.ExternalEntity!.ProjectId == projectId)
            .Include(p => p.Outcomes)
            .Include(p => p.ProblemTags)
                .ThenInclude(pt => pt.Tag)
            .ToListAsync();

        // Fetch all outcomes
        var outcomes = await _context.Outcomes
            .Where(o => o.ExternalEntity!.ProjectId == projectId)
            .Include(o => o.SuccessMetrics)
            .Include(o => o.OutcomeTags)
                .ThenInclude(ot => ot.Tag)
            .ToListAsync();

        // Fetch all interviews
        var interviews = await _context.Interviews
            .Where(i => i.ProjectId == projectId)
            .ToListAsync();

        // Build nodes
        var nodes = new List<GraphNode>();
        var edges = new List<GraphEdge>();

        // Add entity nodes
        foreach (var entity in entities)
        {
            nodes.Add(new GraphNode
            {
                Id = $"entity-{entity.Id}",
                Label = entity.Name,
                Type = "entity",
                Data = new
                {
                    entity.Id,
                    entity.Type,
                    entity.Email,
                    entity.Organization,
                    Tags = entity.EntityTags.Select(et => et.Tag?.Name).ToList()
                }
            });
        }

        // Add problem nodes and edges
        foreach (var problem in problems)
        {
            nodes.Add(new GraphNode
            {
                Id = $"problem-{problem.Id}",
                Label = TruncateText(problem.Description, 50),
                Type = "problem",
                Data = new
                {
                    problem.Id,
                    problem.Description,
                    problem.Severity,
                    Tags = problem.ProblemTags.Select(pt => pt.Tag?.Name).ToList()
                }
            });

            // Edge from entity to problem
            edges.Add(new GraphEdge
            {
                Id = $"entity-{problem.ExternalEntityId}-problem-{problem.Id}",
                Source = $"entity-{problem.ExternalEntityId}",
                Target = $"problem-{problem.Id}",
                Label = "has problem"
            });

            // Edges from problem to outcomes
            foreach (var outcome in problem.Outcomes)
            {
                edges.Add(new GraphEdge
                {
                    Id = $"problem-{problem.Id}-outcome-{outcome.Id}",
                    Source = $"problem-{problem.Id}",
                    Target = $"outcome-{outcome.Id}",
                    Label = "leads to"
                });
            }
        }

        // Add outcome nodes
        foreach (var outcome in outcomes)
        {
            nodes.Add(new GraphNode
            {
                Id = $"outcome-{outcome.Id}",
                Label = TruncateText(outcome.Description, 50),
                Type = "outcome",
                Data = new
                {
                    outcome.Id,
                    outcome.Description,
                    outcome.Priority,
                    SuccessMetrics = outcome.SuccessMetrics.Count,
                    Tags = outcome.OutcomeTags.Select(ot => ot.Tag?.Name).ToList()
                }
            });

            // Add success metric nodes
            foreach (var metric in outcome.SuccessMetrics)
            {
                nodes.Add(new GraphNode
                {
                    Id = $"metric-{metric.Id}",
                    Label = TruncateText(metric.Description, 40),
                    Type = "metric",
                    Data = new
                    {
                        metric.Id,
                        metric.Description,
                        metric.TargetValue,
                        metric.CurrentValue,
                        metric.Unit
                    }
                });

                edges.Add(new GraphEdge
                {
                    Id = $"outcome-{outcome.Id}-metric-{metric.Id}",
                    Source = $"outcome-{outcome.Id}",
                    Target = $"metric-{metric.Id}",
                    Label = "measured by"
                });
            }
        }

        // Add interview connections
        foreach (var interview in interviews)
        {
            nodes.Add(new GraphNode
            {
                Id = $"interview-{interview.Id}",
                Label = $"Interview: {interview.InterviewDate:yyyy-MM-dd}",
                Type = "interview",
                Data = new
                {
                    interview.Id,
                    interview.Type,
                    interview.InterviewDate,
                    interview.Interviewer
                }
            });

            edges.Add(new GraphEdge
            {
                Id = $"entity-{interview.ExternalEntityId}-interview-{interview.Id}",
                Source = $"entity-{interview.ExternalEntityId}",
                Target = $"interview-{interview.Id}",
                Label = "participated in"
            });
        }

        return new GraphData
        {
            Nodes = nodes,
            Edges = edges,
            Stats = new GraphStats
            {
                EntityCount = entities.Count,
                ProblemCount = problems.Count,
                OutcomeCount = outcomes.Count,
                InterviewCount = interviews.Count
            }
        };
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength) + "...";
    }
}

// DTOs for graph data
public class GraphData
{
    public List<GraphNode> Nodes { get; set; } = new();
    public List<GraphEdge> Edges { get; set; } = new();
    public GraphStats? Stats { get; set; }
}

public class GraphNode
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class GraphEdge
{
    public string Id { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class GraphStats
{
    public int EntityCount { get; set; }
    public int ProblemCount { get; set; }
    public int OutcomeCount { get; set; }
    public int InterviewCount { get; set; }
}
