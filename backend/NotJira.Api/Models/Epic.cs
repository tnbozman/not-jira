namespace NotJira.Api.Models;

public class Epic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Theme
    public int ThemeId { get; set; }
    public Theme? Theme { get; set; }
    
    // Foreign key to Outcome (outcome-focused epic)
    public int? OutcomeId { get; set; }
    public Outcome? Outcome { get; set; }
    
    // Navigation properties
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Spike> Spikes { get; set; } = new List<Spike>();
}
