namespace StoryFirst.Api.Areas.SprintPlanning.Models;

public class SprintPlanningDto
{
    public string? PlanningOneNotes { get; set; }
    public List<TeamPlanningDto>? TeamPlannings { get; set; }
}

public class TeamPlanningDto
{
    public int TeamId { get; set; }
    public string? PlanningTwoNotes { get; set; }
}

public class SprintPlanningResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PlanningOneNotes { get; set; }
    public List<TeamPlanningResponse> TeamPlannings { get; set; } = new();
}

public class TeamPlanningResponse
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? PlanningTwoNotes { get; set; }
}
