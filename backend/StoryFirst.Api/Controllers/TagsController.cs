using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace StoryFirst.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId}/tags")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/projects/{projectId}/tags
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        return await _context.Tags
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    // GET: api/projects/{projectId}/tags/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Tag>> GetTag(int projectId, int id)
    {
        var tag = await _context.Tags
            .Where(t => t.ProjectId == projectId && t.Id == id)
            .Include(t => t.EntityTags)
            .Include(t => t.ProblemTags)
            .Include(t => t.OutcomeTags)
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            return NotFound();
        }

        return tag;
    }

    // POST: api/projects/{projectId}/tags
    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(int projectId, Tag tag)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        // Check for duplicate tag name in this project
        if (await _context.Tags.AnyAsync(t => t.ProjectId == projectId && t.Name == tag.Name))
        {
            return Conflict("A tag with this name already exists in this project");
        }

        tag.ProjectId = projectId;
        tag.CreatedAt = DateTime.UtcNow;

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTag), new { projectId, id = tag.Id }, tag);
    }

    // DELETE: api/projects/{projectId}/tags/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int projectId, int id)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (tag == null)
        {
            return NotFound();
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/projects/{projectId}/tags/search?query={query}
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Tag>>> SearchTags(int projectId, [FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required");
        }

        var tags = await _context.Tags
            .Where(t => t.ProjectId == projectId && 
                   (t.Name.Contains(query) || (t.Description != null && t.Description.Contains(query))))
            .OrderBy(t => t.Name)
            .Take(20)
            .ToListAsync();

        return tags;
    }
}
