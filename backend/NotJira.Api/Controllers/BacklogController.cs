using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
public class BacklogController : ControllerBase
{
    private readonly AppDbContext _context;

    public BacklogController(AppDbContext context)
    {
        _context = context;
    }

    public class BacklogItemDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "Story" or "Spike"
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public Priority Priority { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? StoryPoints { get; set; }
        public int EpicId { get; set; }
        public string? EpicName { get; set; }
        public int? SprintId { get; set; }
        public string? SprintName { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public int? ReleaseId { get; set; }
        public string? ReleaseName { get; set; }
        public string? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
    }

    public class BacklogResponse
    {
        public List<SprintGroup> Sprints { get; set; } = new();
        public List<BacklogItemDto> BacklogItems { get; set; } = new();
    }

    public class SprintGroup
    {
        public int SprintId { get; set; }
        public string SprintName { get; set; } = string.Empty;
        public string? SprintGoal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<BacklogItemDto> Items { get; set; } = new();
    }

    [HttpGet]
    public async Task<ActionResult<BacklogResponse>> GetBacklog(
        int projectId,
        [FromQuery] int? teamId = null,
        [FromQuery] string? assigneeId = null,
        [FromQuery] int? releaseId = null,
        [FromQuery] int? epicId = null)
    {
        // Get all sprints for the project
        var sprints = await _context.Sprints
            .Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.StartDate)
            .ToListAsync();

        // Query stories
        var storiesQuery = _context.Stories
            .Include(s => s.Epic)
            .Include(s => s.Sprint)
            .Include(s => s.Team)
            .Include(s => s.Release)
            .Where(s => s.Epic.Theme.ProjectId == projectId);

        // Apply filters
        if (teamId.HasValue)
            storiesQuery = storiesQuery.Where(s => s.TeamId == teamId.Value);
        if (!string.IsNullOrEmpty(assigneeId))
            storiesQuery = storiesQuery.Where(s => s.AssigneeId == assigneeId);
        if (releaseId.HasValue)
            storiesQuery = storiesQuery.Where(s => s.ReleaseId == releaseId.Value);
        if (epicId.HasValue)
            storiesQuery = storiesQuery.Where(s => s.EpicId == epicId.Value);

        var stories = await storiesQuery.ToListAsync();

        // Query spikes
        var spikesQuery = _context.Spikes
            .Include(s => s.Epic)
            .Include(s => s.Sprint)
            .Include(s => s.Team)
            .Include(s => s.Release)
            .Where(s => s.Epic.Theme.ProjectId == projectId);

        // Apply filters
        if (teamId.HasValue)
            spikesQuery = spikesQuery.Where(s => s.TeamId == teamId.Value);
        if (!string.IsNullOrEmpty(assigneeId))
            spikesQuery = spikesQuery.Where(s => s.AssigneeId == assigneeId);
        if (releaseId.HasValue)
            spikesQuery = spikesQuery.Where(s => s.ReleaseId == releaseId.Value);
        if (epicId.HasValue)
            spikesQuery = spikesQuery.Where(s => s.EpicId == epicId.Value);

        var spikes = await spikesQuery.ToListAsync();

        // Convert to DTOs
        var storyDtos = stories.Select(s => new BacklogItemDto
        {
            Id = s.Id,
            Type = "Story",
            Title = s.Title,
            Description = s.Description,
            Order = s.Order,
            Priority = s.Priority,
            Status = s.Status,
            StoryPoints = s.StoryPoints,
            EpicId = s.EpicId,
            EpicName = s.Epic?.Name,
            SprintId = s.SprintId,
            SprintName = s.Sprint?.Name,
            TeamId = s.TeamId,
            TeamName = s.Team?.Name,
            ReleaseId = s.ReleaseId,
            ReleaseName = s.Release?.Name,
            AssigneeId = s.AssigneeId,
            AssigneeName = s.AssigneeName
        }).ToList();

        var spikeDtos = spikes.Select(s => new BacklogItemDto
        {
            Id = s.Id,
            Type = "Spike",
            Title = s.Title,
            Description = s.Description,
            Order = s.Order,
            Priority = s.Priority,
            Status = s.Status,
            StoryPoints = s.StoryPoints,
            EpicId = s.EpicId,
            EpicName = s.Epic?.Name,
            SprintId = s.SprintId,
            SprintName = s.Sprint?.Name,
            TeamId = s.TeamId,
            TeamName = s.Team?.Name,
            ReleaseId = s.ReleaseId,
            ReleaseName = s.Release?.Name,
            AssigneeId = s.AssigneeId,
            AssigneeName = s.AssigneeName
        }).ToList();

        var allItems = storyDtos.Concat(spikeDtos).ToList();

        // Group by sprints
        var sprintGroups = sprints.Select(sprint => new SprintGroup
        {
            SprintId = sprint.Id,
            SprintName = sprint.Name,
            SprintGoal = sprint.Goal,
            StartDate = sprint.StartDate,
            EndDate = sprint.EndDate,
            Status = sprint.Status,
            Items = allItems.Where(i => i.SprintId == sprint.Id).OrderBy(i => i.Order).ToList()
        }).ToList();

        // Get backlog items (no sprint assigned)
        var backlogItems = allItems.Where(i => !i.SprintId.HasValue).OrderBy(i => i.Order).ToList();

        return Ok(new BacklogResponse
        {
            Sprints = sprintGroups,
            BacklogItems = backlogItems
        });
    }
}
