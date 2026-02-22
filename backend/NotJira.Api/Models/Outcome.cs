namespace NotJira.Api.Models;

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

public class Outcome
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public string? Context { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to ExternalEntity
    public int ExternalEntityId { get; set; }
    public ExternalEntity? ExternalEntity { get; set; }
    
    // Many-to-many with Problems
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    
    // Navigation properties
    public ICollection<SuccessMetric> SuccessMetrics { get; set; } = new List<SuccessMetric>();
    public ICollection<OutcomeTag> OutcomeTags { get; set; } = new List<OutcomeTag>();
}
