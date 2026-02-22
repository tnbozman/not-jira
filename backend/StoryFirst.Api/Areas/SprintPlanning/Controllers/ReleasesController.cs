using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.SprintPlanning.Services;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class ReleasesController : BaseApiController
{
    private readonly IReleaseService _releaseService;

    public ReleasesController(IReleaseService releaseService)
    {
        _releaseService = releaseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Release>>> GetReleases(int projectId)
    {
        var releases = await _releaseService.GetAllByProjectAsync(projectId);
        return Ok(releases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Release>> GetRelease(int projectId, int id)
    {
        var release = await _releaseService.GetByIdAsync(projectId, id);

        if (release == null)
        {
            return NotFound();
        }

        return Ok(release);
    }

    [HttpPost]
    public async Task<ActionResult<Release>> CreateRelease(int projectId, Release release)
    {
        var createdRelease = await _releaseService.CreateAsync(projectId, release);
        return CreatedAtAction(nameof(GetRelease), new { projectId, id = createdRelease.Id }, createdRelease);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRelease(int projectId, int id, Release release)
    {
        if (id != release.Id)
        {
            return BadRequest();
        }

        try
        {
            await _releaseService.UpdateAsync(projectId, id, release);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRelease(int projectId, int id)
    {
        try
        {
            await _releaseService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
