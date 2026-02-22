namespace StoryFirst.Api.Models;

public class SuccessMetric
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? TargetValue { get; set; }
    public string? CurrentValue { get; set; }
    public string? Unit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Outcome
    public int OutcomeId { get; set; }
    public Outcome? Outcome { get; set; }
}
