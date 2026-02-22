using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.UserStoryMapping.Services;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/themes/{themeId}/[controller]")]
public class EpicsController : BaseApiController
{
    private readonly IEpicService _epicService;

    public EpicsController(IEpicService epicService)
    {
        _epicService = epicService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Epic>>> GetEpics(int projectId, int themeId)
    {
        var epics = await _epicService.GetByThemeIdAsync(themeId);
        return Ok(epics);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Epic>> GetEpic(int projectId, int themeId, int id)
    {
        var epic = await _epicService.GetByIdAsync(themeId, id);

        if (epic == null)
        {
            return NotFound();
        }

        return Ok(epic);
    }

    [HttpPost]
    public async Task<ActionResult<Epic>> CreateEpic(int projectId, int themeId, Epic epic)
    {
        try
        {
            var result = await _epicService.CreateAsync(themeId, epic);
            return CreatedAtAction(nameof(GetEpic), new { projectId, themeId, id = result.Id }, result);
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
    public async Task<IActionResult> UpdateEpic(int projectId, int themeId, int id, Epic epic)
    {
        try
        {
            await _epicService.UpdateAsync(themeId, id, epic);
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
    public async Task<IActionResult> DeleteEpic(int projectId, int themeId, int id)
    {
        try
        {
            await _epicService.DeleteAsync(themeId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
