using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
public class ReleasesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReleasesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Release>>> GetReleases(int projectId)
    {
        var releases = await _context.Releases
            .Where(r => r.ProjectId == projectId)
            .OrderBy(r => r.ReleaseDate)
            .ToListAsync();
            
        return Ok(releases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Release>> GetRelease(int projectId, int id)
    {
        var release = await _context.Releases
            .Where(r => r.ProjectId == projectId && r.Id == id)
            .FirstOrDefaultAsync();

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

        _context.Releases.Add(release);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRelease), new { projectId, id = release.Id }, release);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRelease(int projectId, int id, Release release)
    {
        if (id != release.Id)
        {
            return BadRequest();
        }

        var existingRelease = await _context.Releases
            .FirstOrDefaultAsync(r => r.Id == id && r.ProjectId == projectId);

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

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRelease(int projectId, int id)
    {
        var release = await _context.Releases
            .FirstOrDefaultAsync(r => r.Id == id && r.ProjectId == projectId);

        if (release == null)
        {
            return NotFound();
        }

        _context.Releases.Remove(release);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
