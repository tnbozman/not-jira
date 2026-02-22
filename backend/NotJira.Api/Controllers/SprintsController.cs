using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
public class SprintsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SprintsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sprint>>> GetSprints(int projectId)
    {
        var sprints = await _context.Sprints
            .Where(s => s.ProjectId == projectId)
            .Include(s => s.Stories)
            .Include(s => s.Spikes)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();
            
        return Ok(sprints);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sprint>> GetSprint(int projectId, int id)
    {
        var sprint = await _context.Sprints
            .Where(s => s.ProjectId == projectId && s.Id == id)
            .Include(s => s.Stories)
            .Include(s => s.Spikes)
            .FirstOrDefaultAsync();

        if (sprint == null)
        {
            return NotFound();
        }

        return Ok(sprint);
    }

    [HttpPost]
    public async Task<ActionResult<Sprint>> CreateSprint(int projectId, Sprint sprint)
    {
        sprint.ProjectId = projectId;
        sprint.CreatedAt = DateTime.UtcNow;
        sprint.UpdatedAt = DateTime.UtcNow;

        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSprint), new { projectId, id = sprint.Id }, sprint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSprint(int projectId, int id, Sprint sprint)
    {
        if (id != sprint.Id)
        {
            return BadRequest();
        }

        var existingSprint = await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (existingSprint == null)
        {
            return NotFound();
        }

        existingSprint.Name = sprint.Name;
        existingSprint.Goal = sprint.Goal;
        existingSprint.StartDate = sprint.StartDate;
        existingSprint.EndDate = sprint.EndDate;
        existingSprint.Status = sprint.Status;
        existingSprint.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSprint(int projectId, int id)
    {
        var sprint = await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        _context.Sprints.Remove(sprint);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
