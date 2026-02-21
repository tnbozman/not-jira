namespace NotJira.Api.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "To Do";
    public string Priority { get; set; } = "Medium";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
