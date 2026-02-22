using StoryFirst.Api.Areas.Visualization.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.Visualization.Services;

public class GraphService : IGraphService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IExternalEntityRepository _entityRepository;
    private readonly IRepository<StoryFirst.Api.Models.Problem> _problemRepository;
    private readonly IRepository<StoryFirst.Api.Models.Outcome> _outcomeRepository;
    private readonly IRepository<StoryFirst.Api.Models.Interview> _interviewRepository;

    public GraphService(
        IProjectRepository projectRepository,
        IExternalEntityRepository entityRepository,
        IRepository<StoryFirst.Api.Models.Problem> problemRepository,
        IRepository<StoryFirst.Api.Models.Outcome> outcomeRepository,
        IRepository<StoryFirst.Api.Models.Interview> interviewRepository)
    {
        _projectRepository = projectRepository;
        _entityRepository = entityRepository;
        _problemRepository = problemRepository;
        _outcomeRepository = outcomeRepository;
        _interviewRepository = interviewRepository;
    }

    public async Task<GraphData> GetGraphDataAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        var entities = await _entityRepository.GetByProjectIdAsync(projectId);
        var problems = await _problemRepository.FindAsync(p => p.ExternalEntity!.ProjectId == projectId);
        var outcomes = await _outcomeRepository.FindAsync(o => o.ExternalEntity!.ProjectId == projectId);
        var interviews = await _interviewRepository.FindAsync(i => i.ProjectId == projectId);

        var nodes = new List<GraphNode>();
        var edges = new List<GraphEdge>();

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

            edges.Add(new GraphEdge
            {
                Id = $"entity-{problem.ExternalEntityId}-problem-{problem.Id}",
                Source = $"entity-{problem.ExternalEntityId}",
                Target = $"problem-{problem.Id}",
                Label = "has problem"
            });

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
                EntityCount = entities.Count(),
                ProblemCount = problems.Count(),
                OutcomeCount = outcomes.Count(),
                InterviewCount = interviews.Count()
            }
        };
    }

    private static string TruncateText(string? text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength) + "...";
    }
}
