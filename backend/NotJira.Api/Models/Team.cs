namespace NotJira.Api.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    // Navigation properties
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Spike> Spikes { get; set; } = new List<Spike>();
}
