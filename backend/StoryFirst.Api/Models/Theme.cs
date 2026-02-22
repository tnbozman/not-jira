namespace StoryFirst.Api.Models;

public class Theme
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Foreign key to Outcome (outcome-focused theme)
    public int? OutcomeId { get; set; }
    public Outcome? Outcome { get; set; }
    
    // Navigation properties
    public ICollection<Epic> Epics { get; set; } = new List<Epic>();
}
