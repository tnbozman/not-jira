namespace StoryFirst.Api.Models;

public class Release
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Status { get; set; } = "Planned"; // Planned, InProgress, Released
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Navigation properties
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Spike> Spikes { get; set; } = new List<Spike>();
}
