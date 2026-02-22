namespace NotJira.Api.Models;

public class Story
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Solution details
    public string? SolutionDescription { get; set; }
    public string? AcceptanceCriteria { get; set; }
    
    public int Order { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public string Status { get; set; } = "Backlog";
    public int? StoryPoints { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Epic
    public int EpicId { get; set; }
    public Epic? Epic { get; set; }
    
    // Foreign key to Sprint (optional - stories can be in backlog)
    public int? SprintId { get; set; }
    public Sprint? Sprint { get; set; }
    
    // Foreign key to Outcome (outcome-focused story)
    public int? OutcomeId { get; set; }
    public Outcome? Outcome { get; set; }
}
