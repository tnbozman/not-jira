using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.SprintPlanning.Models;

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
