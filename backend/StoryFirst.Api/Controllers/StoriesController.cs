using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/epics/{epicId}/[controller]")]
public class StoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public StoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Story>>> GetStories(int projectId, int epicId)
    {
        var stories = await _context.Stories
            .Where(s => s.EpicId == epicId)
            .Include(s => s.Outcome)
            .Include(s => s.Sprint)
            .OrderBy(s => s.Order)
            .ToListAsync();
            
        return Ok(stories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Story>> GetStory(int projectId, int epicId, int id)
    {
        var story = await _context.Stories
            .Where(s => s.EpicId == epicId && s.Id == id)
            .Include(s => s.Outcome)
            .Include(s => s.Sprint)
            .FirstOrDefaultAsync();

        if (story == null)
        {
            return NotFound();
        }

        return Ok(story);
    }

    [HttpPost]
    public async Task<ActionResult<Story>> CreateStory(int projectId, int epicId, Story story)
    {
        story.EpicId = epicId;
        story.CreatedAt = DateTime.UtcNow;
        story.UpdatedAt = DateTime.UtcNow;

        _context.Stories.Add(story);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStory), new { projectId, epicId, id = story.Id }, story);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStory(int projectId, int epicId, int id, Story story)
    {
        if (id != story.Id)
        {
            return BadRequest();
        }

        var existingStory = await _context.Stories
            .FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (existingStory == null)
        {
            return NotFound();
        }

        existingStory.Title = story.Title;
        existingStory.Description = story.Description;
        existingStory.SolutionDescription = story.SolutionDescription;
        existingStory.AcceptanceCriteria = story.AcceptanceCriteria;
        existingStory.Order = story.Order;
        existingStory.Priority = story.Priority;
        existingStory.Status = story.Status;
        existingStory.StoryPoints = story.StoryPoints;
        existingStory.SprintId = story.SprintId;
        existingStory.OutcomeId = story.OutcomeId;
        existingStory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStory(int projectId, int epicId, int id)
    {
        var story = await _context.Stories
            .FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (story == null)
        {
            return NotFound();
        }

        _context.Stories.Remove(story);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
