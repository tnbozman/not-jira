using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/themes/{themeId}/[controller]")]
public class EpicsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EpicsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Epic>>> GetEpics(int projectId, int themeId)
    {
        var epics = await _context.Epics
            .Where(e => e.ThemeId == themeId)
            .Include(e => e.Outcome)
            .Include(e => e.Stories)
            .Include(e => e.Spikes)
            .OrderBy(e => e.Order)
            .ToListAsync();
            
        return Ok(epics);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Epic>> GetEpic(int projectId, int themeId, int id)
    {
        var epic = await _context.Epics
            .Where(e => e.ThemeId == themeId && e.Id == id)
            .Include(e => e.Outcome)
            .Include(e => e.Stories)
            .Include(e => e.Spikes)
            .FirstOrDefaultAsync();

        if (epic == null)
        {
            return NotFound();
        }

        return Ok(epic);
    }

    [HttpPost]
    public async Task<ActionResult<Epic>> CreateEpic(int projectId, int themeId, Epic epic)
    {
        epic.ThemeId = themeId;
        epic.CreatedAt = DateTime.UtcNow;
        epic.UpdatedAt = DateTime.UtcNow;

        _context.Epics.Add(epic);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEpic), new { projectId, themeId, id = epic.Id }, epic);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEpic(int projectId, int themeId, int id, Epic epic)
    {
        if (id != epic.Id)
        {
            return BadRequest();
        }

        var existingEpic = await _context.Epics
            .FirstOrDefaultAsync(e => e.Id == id && e.ThemeId == themeId);

        if (existingEpic == null)
        {
            return NotFound();
        }

        existingEpic.Name = epic.Name;
        existingEpic.Description = epic.Description;
        existingEpic.Order = epic.Order;
        existingEpic.OutcomeId = epic.OutcomeId;
        existingEpic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEpic(int projectId, int themeId, int id)
    {
        var epic = await _context.Epics
            .FirstOrDefaultAsync(e => e.Id == id && e.ThemeId == themeId);

        if (epic == null)
        {
            return NotFound();
        }

        _context.Epics.Remove(epic);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
