using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class ReleasesController : BaseApiController
{
    private readonly IRepository<Release> _releaseRepository;

    public ReleasesController(IRepository<Release> releaseRepository)
    {
        _releaseRepository = releaseRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Release>>> GetReleases(int projectId)
    {
        var releases = (await _releaseRepository.FindAsync(r => r.ProjectId == projectId))
            .OrderBy(r => r.ReleaseDate)
            .ToList();
            
        return Ok(releases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Release>> GetRelease(int projectId, int id)
    {
        var release = await _releaseRepository.FirstOrDefaultAsync(r => r.ProjectId == projectId && r.Id == id);

        if (release == null)
        {
            return NotFound();
        }

        return Ok(release);
    }

    [HttpPost]
    public async Task<ActionResult<Release>> CreateRelease(int projectId, Release release)
    {
        release.ProjectId = projectId;
        release.CreatedAt = DateTime.UtcNow;
        release.UpdatedAt = DateTime.UtcNow;

        await _releaseRepository.AddAsync(release);
        await _releaseRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRelease), new { projectId, id = release.Id }, release);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRelease(int projectId, int id, Release release)
    {
        if (id != release.Id)
        {
            return BadRequest();
        }

        var existingRelease = await _releaseRepository.FirstOrDefaultAsync(r => r.Id == id && r.ProjectId == projectId);

        if (existingRelease == null)
        {
            return NotFound();
        }

        existingRelease.Name = release.Name;
        existingRelease.Description = release.Description;
        existingRelease.StartDate = release.StartDate;
        existingRelease.ReleaseDate = release.ReleaseDate;
        existingRelease.Status = release.Status;
        existingRelease.UpdatedAt = DateTime.UtcNow;

        _releaseRepository.Update(existingRelease);
        await _releaseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRelease(int projectId, int id)
    {
        var release = await _releaseRepository.FirstOrDefaultAsync(r => r.Id == id && r.ProjectId == projectId);

        if (release == null)
        {
            return NotFound();
        }

        _releaseRepository.Remove(release);
        await _releaseRepository.SaveChangesAsync();

        return NoContent();
    }
}
