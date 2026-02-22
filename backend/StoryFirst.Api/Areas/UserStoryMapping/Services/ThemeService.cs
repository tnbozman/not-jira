using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public class ThemeService : IThemeService
{
    private readonly IThemeRepository _themeRepository;
    private readonly IEpicRepository _epicRepository;

    public ThemeService(
        IThemeRepository themeRepository,
        IEpicRepository epicRepository)
    {
        _themeRepository = themeRepository;
        _epicRepository = epicRepository;
    }

    public async Task<IEnumerable<object>> GetAllByProjectAsync(int projectId)
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

        return result;
    }

    public async Task<object?> GetByIdAsync(int projectId, int id)
    {
        var theme = await _themeRepository.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == id);

        if (theme == null)
        {
            return null;
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

        return result;
    }

    public async Task<Theme> CreateAsync(int projectId, Theme theme)
    {
        theme.ProjectId = projectId;
        theme.CreatedAt = DateTime.UtcNow;
        theme.UpdatedAt = DateTime.UtcNow;

        await _themeRepository.AddAsync(theme);
        await _themeRepository.SaveChangesAsync();

        return theme;
    }

    public async Task UpdateAsync(int projectId, int id, Theme theme)
    {
        if (id != theme.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingTheme = await _themeRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (existingTheme == null)
        {
            throw new KeyNotFoundException("Theme not found");
        }

        existingTheme.Name = theme.Name;
        existingTheme.Description = theme.Description;
        existingTheme.Order = theme.Order;
        existingTheme.OutcomeId = theme.OutcomeId;
        existingTheme.UpdatedAt = DateTime.UtcNow;

        _themeRepository.Update(existingTheme);
        await _themeRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var theme = await _themeRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (theme == null)
        {
            throw new KeyNotFoundException("Theme not found");
        }

        _themeRepository.Remove(theme);
        await _themeRepository.SaveChangesAsync();
    }
}
