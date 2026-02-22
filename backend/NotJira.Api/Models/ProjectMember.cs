namespace NotJira.Api.Models;

public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string UserId { get; set; } = string.Empty; // Keycloak user ID
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public ProjectRole Role { get; set; }
    public DateTime AddedAt { get; set; }
    
    // Navigation property
    public Project Project { get; set; } = null!;
}

public enum ProjectRole
{
    Developer,
    ProductManager,
    ProjectSponsor
}
