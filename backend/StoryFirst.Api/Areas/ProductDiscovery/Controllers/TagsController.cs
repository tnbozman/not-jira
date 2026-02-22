using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/tags")]
public class TagsController : BaseApiController
{
    private readonly IRepository<Tag> _tagRepository;
    private readonly IProjectRepository _projectRepository;

    public TagsController(
        IRepository<Tag> tagRepository,
        IProjectRepository projectRepository)
    {
        _tagRepository = tagRepository;
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        var tags = (await _tagRepository.FindAsync(t => t.ProjectId == projectId))
            .OrderBy(t => t.Name)
            .ToList();

        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tag>> GetTag(int projectId, int id)
    {
        var tag = await _tagRepository.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == id);

        if (tag == null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(int projectId, Tag tag)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        if (await _tagRepository.AnyAsync(t => t.ProjectId == projectId && t.Name == tag.Name))
        {
            return Conflict("A tag with this name already exists in this project");
        }

        tag.ProjectId = projectId;
        tag.CreatedAt = DateTime.UtcNow;

        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTag), new { projectId, id = tag.Id }, tag);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int projectId, int id)
    {
        var tag = await _tagRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (tag == null)
        {
            return NotFound();
        }

        _tagRepository.Remove(tag);
        await _tagRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Tag>>> SearchTags(int projectId, [FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required");
        }

        var tags = (await _tagRepository.FindAsync(t => t.ProjectId == projectId && 
               (t.Name.Contains(query) || (t.Description != null && t.Description.Contains(query)))))
            .OrderBy(t => t.Name)
            .Take(20)
            .ToList();

        return Ok(tags);
    }
}
