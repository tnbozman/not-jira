using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.ProductDiscovery.Services;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/tags")]
public class TagsController : BaseApiController
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags(int projectId)
    {
        try
        {
            var tags = await _tagService.GetAllByProjectAsync(projectId);
            return Ok(tags);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tag>> GetTag(int projectId, int id)
    {
        var tag = await _tagService.GetByIdAsync(projectId, id);
        if (tag == null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(int projectId, Tag tag)
    {
        try
        {
            var createdTag = await _tagService.CreateAsync(projectId, tag);
            return CreatedAtAction(nameof(GetTag), new { projectId, id = createdTag.Id }, createdTag);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int projectId, int id)
    {
        try
        {
            await _tagService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Tag>>> SearchTags(int projectId, [FromQuery] string query)
    {
        try
        {
            var tags = await _tagService.SearchAsync(projectId, query);
            return Ok(tags);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
