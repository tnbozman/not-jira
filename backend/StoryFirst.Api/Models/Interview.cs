namespace StoryFirst.Api.Models;

public enum InterviewType
{
    Discovery,
    Feedback,
    Clarification
}

public class Interview
{
    public int Id { get; set; }
    public InterviewType Type { get; set; }
    public DateTime InterviewDate { get; set; }
    public string Interviewer { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to ExternalEntity
    public int ExternalEntityId { get; set; }
    public ExternalEntity? ExternalEntity { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Navigation properties
    public ICollection<InterviewNote> Notes { get; set; } = new List<InterviewNote>();
}
