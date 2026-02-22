using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/[controller]")]
public class ThemesController : BaseApiController
{
    private readonly IThemeRepository _themeRepository;
    private readonly IEpicRepository _epicRepository;

    public ThemesController(
        IThemeRepository themeRepository,
        IEpicRepository epicRepository)
    {
        _themeRepository = themeRepository;
        _epicRepository = epicRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetThemes(int projectId)
    {
        var themes = await _themeRepository.FindAsync(t => t.ProjectId == projectId);
        var orderedThemes = themes.OrderBy(t => t.Order).ToList();
        
        var result = new List<object>();
        foreach (var theme in orderedThemes)
        {
            var themeDetails = await _themeRepository.GetWithDetailsAsync(theme.Id);
            var epics = await _epicRepository.FindAsync(e => e.ThemeId == theme.Id);
            
            result.Add(new
            {
                themeDetails!.Id,
                themeDetails.Name,
                themeDetails.Description,
                themeDetails.Order,
                themeDetails.ProjectId,
                themeDetails.OutcomeId,
                Outcome = themeDetails.Outcome == null ? null : new { themeDetails.Outcome.Id, themeDetails.Outcome.Description },
                themeDetails.CreatedAt,
                themeDetails.UpdatedAt,
                Epics = epics.OrderBy(e => e.Order).Select(e => new
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
            });
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTheme(int projectId, int id)
    {
        var theme = await _themeRepository.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == id);

        if (theme == null)
        {
            return NotFound();
        }

        var themeDetails = await _themeRepository.GetWithDetailsAsync(id);
        var epics = await _epicRepository.FindAsync(e => e.ThemeId == id);

        var result = new
        {
            themeDetails!.Id,
            themeDetails.Name,
            themeDetails.Description,
            themeDetails.Order,
            themeDetails.ProjectId,
            themeDetails.OutcomeId,
            Outcome = themeDetails.Outcome == null ? null : new { themeDetails.Outcome.Id, themeDetails.Outcome.Description },
            themeDetails.CreatedAt,
            themeDetails.UpdatedAt,
            Epics = epics.OrderBy(e => e.Order).Select(e => new
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
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Theme>> CreateTheme(int projectId, Theme theme)
    {
        theme.ProjectId = projectId;
        theme.CreatedAt = DateTime.UtcNow;
        theme.UpdatedAt = DateTime.UtcNow;

        await _themeRepository.AddAsync(theme);
        await _themeRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTheme), new { projectId, id = theme.Id }, theme);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTheme(int projectId, int id, Theme theme)
    {
        if (id != theme.Id)
        {
            return BadRequest();
        }

        var existingTheme = await _themeRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (existingTheme == null)
        {
            return NotFound();
        }

        existingTheme.Name = theme.Name;
        existingTheme.Description = theme.Description;
        existingTheme.Order = theme.Order;
        existingTheme.OutcomeId = theme.OutcomeId;
        existingTheme.UpdatedAt = DateTime.UtcNow;

        _themeRepository.Update(existingTheme);
        await _themeRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTheme(int projectId, int id)
    {
        var theme = await _themeRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (theme == null)
        {
            return NotFound();
        }

        _themeRepository.Remove(theme);
        await _themeRepository.SaveChangesAsync();

        return NoContent();
    }
}
