namespace StoryFirst.Api.Models;

public class Sprint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Planned"; // Planned, Active, Completed
    
    // LeSS Planning fields
    public string? PlanningOneNotes { get; set; }
    public string? ReviewNotes { get; set; }
    public string? RetroNotes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Navigation properties
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Spike> Spikes { get; set; } = new List<Spike>();
    public ICollection<TeamPlanning> TeamPlannings { get; set; } = new List<TeamPlanning>();
}
