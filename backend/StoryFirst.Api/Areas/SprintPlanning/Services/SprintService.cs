using StoryFirst.Api.Areas.SprintPlanning.Models;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public class SprintService : ISprintService
{
    private readonly IRepository<Sprint> _sprintRepository;
    private readonly IRepository<TeamPlanning> _teamPlanningRepository;

    public SprintService(
        IRepository<Sprint> sprintRepository,
        IRepository<TeamPlanning> teamPlanningRepository)
    {
        _sprintRepository = sprintRepository;
        _teamPlanningRepository = teamPlanningRepository;
    }

    public async Task<IEnumerable<Sprint>> GetAllByProjectAsync(int projectId)
    {
        var sprints = (await _sprintRepository.FindAsync(s => s.ProjectId == projectId))
            .OrderByDescending(s => s.StartDate)
            .ToList();

        return sprints;
    }

    public async Task<Sprint?> GetByIdAsync(int projectId, int id)
    {
        return await _sprintRepository.FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Id == id);
    }

    public async Task<Sprint> CreateAsync(int projectId, Sprint sprint)
    {
        sprint.ProjectId = projectId;
        sprint.CreatedAt = DateTime.UtcNow;
        sprint.UpdatedAt = DateTime.UtcNow;

        await _sprintRepository.AddAsync(sprint);
        await _sprintRepository.SaveChangesAsync();

        return sprint;
    }

    public async Task UpdateAsync(int projectId, int id, Sprint sprint)
    {
        if (id != sprint.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingSprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (existingSprint == null)
        {
            throw new KeyNotFoundException("Sprint not found");
        }

        existingSprint.Name = sprint.Name;
        existingSprint.Goal = sprint.Goal;
        existingSprint.StartDate = sprint.StartDate;
        existingSprint.EndDate = sprint.EndDate;
        existingSprint.Status = sprint.Status;
        existingSprint.PlanningOneNotes = sprint.PlanningOneNotes;
        existingSprint.ReviewNotes = sprint.ReviewNotes;
        existingSprint.RetroNotes = sprint.RetroNotes;
        existingSprint.UpdatedAt = DateTime.UtcNow;

        _sprintRepository.Update(existingSprint);
        await _sprintRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            throw new KeyNotFoundException("Sprint not found");
        }

        _sprintRepository.Remove(sprint);
        await _sprintRepository.SaveChangesAsync();
    }

    public async Task<SprintPlanningResponse> GetTeamPlanningAsync(int projectId, int sprintId)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Id == sprintId);

        if (sprint == null)
        {
            throw new KeyNotFoundException("Sprint not found");
        }

        var teamPlannings = await _teamPlanningRepository.FindAsync(tp => tp.SprintId == sprintId);

        return new SprintPlanningResponse
        {
            Id = sprint.Id,
            Name = sprint.Name,
            PlanningOneNotes = sprint.PlanningOneNotes,
            TeamPlannings = teamPlannings.Select(tp => new TeamPlanningResponse
            {
                Id = tp.Id,
                TeamId = tp.TeamId,
                TeamName = tp.Team?.Name,
                PlanningTwoNotes = tp.PlanningTwoNotes
            }).ToList()
        };
    }

    public async Task SaveTeamPlanningAsync(int projectId, int sprintId, SprintPlanningDto planning)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Id == sprintId);

        if (sprint == null)
        {
            throw new KeyNotFoundException("Sprint not found");
        }

        sprint.PlanningOneNotes = planning.PlanningOneNotes;
        sprint.UpdatedAt = DateTime.UtcNow;

        if (planning.TeamPlannings != null)
        {
            var existingPlannings = await _teamPlanningRepository.FindAsync(tp => tp.SprintId == sprint.Id);

            foreach (var teamPlanningDto in planning.TeamPlannings)
            {
                var existing = existingPlannings.FirstOrDefault(tp => tp.TeamId == teamPlanningDto.TeamId);
                if (existing != null)
                {
                    existing.PlanningTwoNotes = teamPlanningDto.PlanningTwoNotes;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _teamPlanningRepository.Update(existing);
                }
                else
                {
                    await _teamPlanningRepository.AddAsync(new TeamPlanning
                    {
                        SprintId = sprint.Id,
                        TeamId = teamPlanningDto.TeamId,
                        PlanningTwoNotes = teamPlanningDto.PlanningTwoNotes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        _sprintRepository.Update(sprint);
        await _sprintRepository.SaveChangesAsync();
    }

    public async Task UpdateReviewNotesAsync(int projectId, int sprintId, string? notes)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == sprintId && s.ProjectId == projectId);

        if (sprint == null)
        {
            throw new KeyNotFoundException("Sprint not found");
        }

        sprint.ReviewNotes = notes;
        sprint.UpdatedAt = DateTime.UtcNow;

        _sprintRepository.Update(sprint);
        await _sprintRepository.SaveChangesAsync();
    }

    public async Task UpdateRetroNotesAsync(int projectId, int sprintId, string? notes)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == sprintId && s.ProjectId == projectId);

        if (sprint == null)
        {
            throw new KeyNotFoundException("Sprint not found");
        }

        sprint.RetroNotes = notes;
        sprint.UpdatedAt = DateTime.UtcNow;

        _sprintRepository.Update(sprint);
        await _sprintRepository.SaveChangesAsync();
    }
}
