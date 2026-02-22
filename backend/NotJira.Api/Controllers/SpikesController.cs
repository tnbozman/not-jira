using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/epics/{epicId}/[controller]")]
public class SpikesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SpikesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Spike>>> GetSpikes(int projectId, int epicId)
    {
        var spikes = await _context.Spikes
            .Where(s => s.EpicId == epicId)
            .Include(s => s.Outcome)
            .Include(s => s.Sprint)
            .OrderBy(s => s.Order)
            .ToListAsync();
            
        return Ok(spikes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Spike>> GetSpike(int projectId, int epicId, int id)
    {
        var spike = await _context.Spikes
            .Where(s => s.EpicId == epicId && s.Id == id)
            .Include(s => s.Outcome)
            .Include(s => s.Sprint)
            .FirstOrDefaultAsync();

        if (spike == null)
        {
            return NotFound();
        }

        return Ok(spike);
    }

    [HttpPost]
    public async Task<ActionResult<Spike>> CreateSpike(int projectId, int epicId, Spike spike)
    {
        spike.EpicId = epicId;
        spike.CreatedAt = DateTime.UtcNow;
        spike.UpdatedAt = DateTime.UtcNow;

        _context.Spikes.Add(spike);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSpike), new { projectId, epicId, id = spike.Id }, spike);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpike(int projectId, int epicId, int id, Spike spike)
    {
        if (id != spike.Id)
        {
            return BadRequest();
        }

        var existingSpike = await _context.Spikes
            .FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (existingSpike == null)
        {
            return NotFound();
        }

        existingSpike.Title = spike.Title;
        existingSpike.Description = spike.Description;
        existingSpike.InvestigationGoal = spike.InvestigationGoal;
        existingSpike.Findings = spike.Findings;
        existingSpike.Order = spike.Order;
        existingSpike.Priority = spike.Priority;
        existingSpike.Status = spike.Status;
        existingSpike.StoryPoints = spike.StoryPoints;
        existingSpike.SprintId = spike.SprintId;
        existingSpike.OutcomeId = spike.OutcomeId;
        existingSpike.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpike(int projectId, int epicId, int id)
    {
        var spike = await _context.Spikes
            .FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (spike == null)
        {
            return NotFound();
        }

        _context.Spikes.Remove(spike);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
