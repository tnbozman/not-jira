using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class BacklogController : BaseApiController
{
    private readonly IRepository<Sprint> _sprintRepository;
    private readonly IStoryRepository _storyRepository;
    private readonly ISpikeRepository _spikeRepository;

    public BacklogController(
        IRepository<Sprint> sprintRepository,
        IStoryRepository storyRepository,
        ISpikeRepository spikeRepository)
    {
        _sprintRepository = sprintRepository;
        _storyRepository = storyRepository;
        _spikeRepository = spikeRepository;
    }

    public class BacklogItemDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
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
        var sprints = (await _sprintRepository.FindAsync(s => s.ProjectId == projectId))
            .OrderBy(s => s.StartDate)
            .ToList();

        var storiesQuery = await _storyRepository.FindAsync(s => s.Epic!.Theme!.ProjectId == projectId);
        var stories = storiesQuery.ToList();

        if (teamId.HasValue)
            stories = stories.Where(s => s.TeamId == teamId.Value).ToList();
        if (!string.IsNullOrEmpty(assigneeId))
            stories = stories.Where(s => s.AssigneeId == assigneeId).ToList();
        if (releaseId.HasValue)
            stories = stories.Where(s => s.ReleaseId == releaseId.Value).ToList();
        if (epicId.HasValue)
            stories = stories.Where(s => s.EpicId == epicId.Value).ToList();

        var spikesQuery = await _spikeRepository.FindAsync(s => s.Epic!.Theme!.ProjectId == projectId);
        var spikes = spikesQuery.ToList();

        if (teamId.HasValue)
            spikes = spikes.Where(s => s.TeamId == teamId.Value).ToList();
        if (!string.IsNullOrEmpty(assigneeId))
            spikes = spikes.Where(s => s.AssigneeId == assigneeId).ToList();
        if (releaseId.HasValue)
            spikes = spikes.Where(s => s.ReleaseId == releaseId.Value).ToList();
        if (epicId.HasValue)
            spikes = spikes.Where(s => s.EpicId == epicId.Value).ToList();

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

        var backlogItems = allItems.Where(i => !i.SprintId.HasValue).OrderBy(i => i.Order).ToList();

        return Ok(new BacklogResponse
        {
            Sprints = sprintGroups,
            BacklogItems = backlogItems
        });
    }
}
