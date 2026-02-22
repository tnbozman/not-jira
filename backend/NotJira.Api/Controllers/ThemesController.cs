using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
public class ThemesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThemesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Theme>>> GetThemes(int projectId)
    {
        var themes = await _context.Themes
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Outcome)
            .Include(t => t.Epics)
            .OrderBy(t => t.Order)
            .ToListAsync();
            
        return Ok(themes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Theme>> GetTheme(int projectId, int id)
    {
        var theme = await _context.Themes
            .Where(t => t.ProjectId == projectId && t.Id == id)
            .Include(t => t.Outcome)
            .Include(t => t.Epics)
            .FirstOrDefaultAsync();

        if (theme == null)
        {
            return NotFound();
        }

        return Ok(theme);
    }

    [HttpPost]
    public async Task<ActionResult<Theme>> CreateTheme(int projectId, Theme theme)
    {
        theme.ProjectId = projectId;
        theme.CreatedAt = DateTime.UtcNow;
        theme.UpdatedAt = DateTime.UtcNow;

        _context.Themes.Add(theme);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTheme), new { projectId, id = theme.Id }, theme);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTheme(int projectId, int id, Theme theme)
    {
        if (id != theme.Id)
        {
            return BadRequest();
        }

        var existingTheme = await _context.Themes
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (existingTheme == null)
        {
            return NotFound();
        }

        existingTheme.Name = theme.Name;
        existingTheme.Description = theme.Description;
        existingTheme.Order = theme.Order;
        existingTheme.OutcomeId = theme.OutcomeId;
        existingTheme.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTheme(int projectId, int id)
    {
        var theme = await _context.Themes
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (theme == null)
        {
            return NotFound();
        }

        _context.Themes.Remove(theme);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
