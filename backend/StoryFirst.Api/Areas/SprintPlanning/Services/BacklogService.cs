using StoryFirst.Api.Areas.SprintPlanning.Models;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public class BacklogService : IBacklogService
{
    private readonly IRepository<Sprint> _sprintRepository;
    private readonly IStoryRepository _storyRepository;
    private readonly ISpikeRepository _spikeRepository;

    public BacklogService(
        IRepository<Sprint> sprintRepository,
        IStoryRepository storyRepository,
        ISpikeRepository spikeRepository)
    {
        _sprintRepository = sprintRepository;
        _storyRepository = storyRepository;
        _spikeRepository = spikeRepository;
    }

    public async Task<BacklogResponse> GetBacklogAsync(
        int projectId,
        int? teamId = null,
        string? assigneeId = null,
        int? releaseId = null,
        int? epicId = null)
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

        return new BacklogResponse
        {
            Sprints = sprintGroups,
            BacklogItems = backlogItems
        };
    }
}
