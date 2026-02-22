namespace StoryFirst.Api.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Navigation properties
    public ICollection<EntityTag> EntityTags { get; set; } = new List<EntityTag>();
    public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
    public ICollection<OutcomeTag> OutcomeTags { get; set; } = new List<OutcomeTag>();
}

// Junction tables for many-to-many relationships
public class EntityTag
{
    public int ExternalEntityId { get; set; }
    public ExternalEntity? ExternalEntity { get; set; }
    
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}

public class ProblemTag
{
    public int ProblemId { get; set; }
    public Problem? Problem { get; set; }
    
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}

public class OutcomeTag
{
    public int OutcomeId { get; set; }
    public Outcome? Outcome { get; set; }
    
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
