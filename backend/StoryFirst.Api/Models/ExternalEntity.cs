namespace StoryFirst.Api.Models;

public enum EntityType
{
    Person,
    Client
}

public class ExternalEntity
{
    public int Id { get; set; }
    public EntityType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Organization { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Navigation properties
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<EntityTag> EntityTags { get; set; } = new List<EntityTag>();
}
