namespace StoryFirst.Api.Models;

public class InterviewNote
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? RelatedProblemId { get; set; }
    public int? RelatedOutcomeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Interview
    public int InterviewId { get; set; }
    public Interview? Interview { get; set; }
    
    // Optional navigation properties
    public Problem? RelatedProblem { get; set; }
    public Outcome? RelatedOutcome { get; set; }
}
