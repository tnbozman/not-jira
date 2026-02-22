namespace StoryFirst.Api.Models;

public class Project
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property for project members
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}
