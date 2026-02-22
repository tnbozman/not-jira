using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Controllers;

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
    public async Task<IActionResult> GetThemes(int projectId)
    {
        var themes = await _context.Themes
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Order)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.Order,
                t.ProjectId,
                t.OutcomeId,
                Outcome = t.Outcome == null ? null : new { t.Outcome.Id, t.Outcome.Description },
                t.CreatedAt,
                t.UpdatedAt,
                Epics = t.Epics.OrderBy(e => e.Order).Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Description,
                    e.Order,
                    e.ThemeId,
                    e.OutcomeId,
                    Outcome = e.Outcome == null ? null : new { e.Outcome.Id, e.Outcome.Description },
                    e.CreatedAt,
                    e.UpdatedAt,
                    Stories = e.Stories.OrderBy(s => s.Order).Select(s => new
                    {
                        s.Id,
                        s.Title,
                        s.Description,
                        s.SolutionDescription,
                        s.AcceptanceCriteria,
                        s.Order,
                        s.Priority,
                        s.Status,
                        s.StoryPoints,
                        s.EpicId,
                        s.SprintId,
                        Sprint = s.Sprint == null ? null : new { s.Sprint.Id, s.Sprint.Name },
                        s.ReleaseId,
                        Release = s.Release == null ? null : new { s.Release.Id, s.Release.Name },
                        s.TeamId,
                        s.AssigneeId,
                        s.AssigneeName,
                        s.OutcomeId,
                        s.CreatedAt,
                        s.UpdatedAt
                    }).ToList(),
                    Spikes = e.Spikes.OrderBy(sp => sp.Order).Select(sp => new
                    {
                        sp.Id,
                        sp.Title,
                        sp.Description,
                        sp.InvestigationGoal,
                        sp.Findings,
                        sp.Order,
                        sp.Priority,
                        sp.Status,
                        sp.StoryPoints,
                        sp.EpicId,
                        sp.SprintId,
                        Sprint = sp.Sprint == null ? null : new { sp.Sprint.Id, sp.Sprint.Name },
                        sp.ReleaseId,
                        Release = sp.Release == null ? null : new { sp.Release.Id, sp.Release.Name },
                        sp.TeamId,
                        sp.AssigneeId,
                        sp.AssigneeName,
                        sp.OutcomeId,
                        sp.CreatedAt,
                        sp.UpdatedAt
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(themes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTheme(int projectId, int id)
    {
        var theme = await _context.Themes
            .Where(t => t.ProjectId == projectId && t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.Order,
                t.ProjectId,
                t.OutcomeId,
                Outcome = t.Outcome == null ? null : new { t.Outcome.Id, t.Outcome.Description },
                t.CreatedAt,
                t.UpdatedAt,
                Epics = t.Epics.OrderBy(e => e.Order).Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Description,
                    e.Order,
                    e.ThemeId,
                    e.OutcomeId,
                    Outcome = e.Outcome == null ? null : new { e.Outcome.Id, e.Outcome.Description },
                    e.CreatedAt,
                    e.UpdatedAt,
                    Stories = e.Stories.OrderBy(s => s.Order).Select(s => new
                    {
                        s.Id,
                        s.Title,
                        s.Description,
                        s.SolutionDescription,
                        s.AcceptanceCriteria,
                        s.Order,
                        s.Priority,
                        s.Status,
                        s.StoryPoints,
                        s.EpicId,
                        s.SprintId,
                        Sprint = s.Sprint == null ? null : new { s.Sprint.Id, s.Sprint.Name },
                        s.ReleaseId,
                        Release = s.Release == null ? null : new { s.Release.Id, s.Release.Name },
                        s.TeamId,
                        s.AssigneeId,
                        s.AssigneeName,
                        s.OutcomeId,
                        s.CreatedAt,
                        s.UpdatedAt
                    }).ToList(),
                    Spikes = e.Spikes.OrderBy(sp => sp.Order).Select(sp => new
                    {
                        sp.Id,
                        sp.Title,
                        sp.Description,
                        sp.InvestigationGoal,
                        sp.Findings,
                        sp.Order,
                        sp.Priority,
                        sp.Status,
                        sp.StoryPoints,
                        sp.EpicId,
                        sp.SprintId,
                        Sprint = sp.Sprint == null ? null : new { sp.Sprint.Id, sp.Sprint.Name },
                        sp.ReleaseId,
                        Release = sp.Release == null ? null : new { sp.Release.Id, sp.Release.Name },
                        sp.TeamId,
                        sp.AssigneeId,
                        sp.AssigneeName,
                        sp.OutcomeId,
                        sp.CreatedAt,
                        sp.UpdatedAt
                    }).ToList()
                }).ToList()
            })
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
