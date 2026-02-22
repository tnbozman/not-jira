namespace StoryFirst.Api.Models;

public enum Severity
{
    Low,
    Medium,
    High,
    Critical
}

public class Problem
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Severity Severity { get; set; } = Severity.Medium;
    public string? Context { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to ExternalEntity
    public int ExternalEntityId { get; set; }
    public ExternalEntity? ExternalEntity { get; set; }
    
    // Navigation properties
    public ICollection<Outcome> Outcomes { get; set; } = new List<Outcome>();
    public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
}
