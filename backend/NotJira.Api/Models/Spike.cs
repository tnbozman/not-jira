namespace NotJira.Api.Models;

public class Spike
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Investigation/research details
    public string? InvestigationGoal { get; set; }
    public string? Findings { get; set; }
    
    public int Order { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public string Status { get; set; } = "Backlog";
    public int? StoryPoints { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Epic
    public int EpicId { get; set; }
    public Epic? Epic { get; set; }
    
    // Foreign key to Sprint (optional - spikes can be in backlog)
    public int? SprintId { get; set; }
    public Sprint? Sprint { get; set; }
    
    // Foreign key to Team (optional)
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
    
    // Foreign key to Release (optional)
    public int? ReleaseId { get; set; }
    public Release? Release { get; set; }
    
    // Assignee (using UserId from Keycloak)
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    
    // Foreign key to Outcome (outcome-focused spike)
    public int? OutcomeId { get; set; }
    public Outcome? Outcome { get; set; }
}
