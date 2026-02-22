using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.UserStoryMapping.Services;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/[controller]")]
public class ThemesController : BaseApiController
{
    private readonly IThemeService _themeService;

    public ThemesController(IThemeService themeService)
    {
        _themeService = themeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetThemes(int projectId)
    {
        var themes = await _themeService.GetAllByProjectAsync(projectId);
        return Ok(themes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTheme(int projectId, int id)
    {
        var theme = await _themeService.GetByIdAsync(projectId, id);

        if (theme == null)
        {
            return NotFound();
        }

        return Ok(theme);
    }

    [HttpPost]
    public async Task<ActionResult<Theme>> CreateTheme(int projectId, Theme theme)
    {
        try
        {
            var result = await _themeService.CreateAsync(projectId, theme);
            return CreatedAtAction(nameof(GetTheme), new { projectId, id = result.Id }, result);
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
    public async Task<IActionResult> UpdateTheme(int projectId, int id, Theme theme)
    {
        try
        {
            await _themeService.UpdateAsync(projectId, id, theme);
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
    public async Task<IActionResult> DeleteTheme(int projectId, int id)
    {
        try
        {
            await _themeService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
