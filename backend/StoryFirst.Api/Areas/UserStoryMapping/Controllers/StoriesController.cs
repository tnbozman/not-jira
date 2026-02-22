using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.UserStoryMapping.Services;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/epics/{epicId}/[controller]")]
public class StoriesController : BaseApiController
{
    private readonly IStoryService _storyService;

    public StoriesController(IStoryService storyService)
    {
        _storyService = storyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Story>>> GetStories(int projectId, int epicId)
    {
        var stories = await _storyService.GetByEpicIdAsync(epicId);
        return Ok(stories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Story>> GetStory(int projectId, int epicId, int id)
    {
        var story = await _storyService.GetByIdAsync(epicId, id);

        if (story == null)
        {
            return NotFound();
        }

        return Ok(story);
    }

    [HttpPost]
    public async Task<ActionResult<Story>> CreateStory(int projectId, int epicId, Story story)
    {
        try
        {
            var result = await _storyService.CreateAsync(epicId, story);
            return CreatedAtAction(nameof(GetStory), new { projectId, epicId, id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStory(int projectId, int epicId, int id, Story story)
    {
        try
        {
            await _storyService.UpdateAsync(epicId, id, story);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStory(int projectId, int epicId, int id)
    {
        try
        {
            await _storyService.DeleteAsync(epicId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
