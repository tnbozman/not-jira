namespace NotJira.Api.Models;

public class TeamPlanning
{
    public int Id { get; set; }
    public string? PlanningTwoNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Sprint
    public int SprintId { get; set; }
    public Sprint? Sprint { get; set; }
    
    // Foreign key to Team
    public int TeamId { get; set; }
    public Team? Team { get; set; }
}
