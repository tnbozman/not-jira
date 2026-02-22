using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public class TeamService : ITeamService
{
    private readonly IRepository<Team> _teamRepository;

    public TeamService(IRepository<Team> teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<IEnumerable<Team>> GetAllByProjectAsync(int projectId)
    {
        var teams = (await _teamRepository.FindAsync(t => t.ProjectId == projectId))
            .OrderBy(t => t.Name)
            .ToList();

        return teams;
    }

    public async Task<Team?> GetByIdAsync(int projectId, int id)
    {
        return await _teamRepository.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == id);
    }

    public async Task<Team> CreateAsync(int projectId, Team team)
    {
        team.ProjectId = projectId;
        team.CreatedAt = DateTime.UtcNow;
        team.UpdatedAt = DateTime.UtcNow;

        await _teamRepository.AddAsync(team);
        await _teamRepository.SaveChangesAsync();

        return team;
    }

    public async Task UpdateAsync(int projectId, int id, Team team)
    {
        if (id != team.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingTeam = await _teamRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (existingTeam == null)
        {
            throw new KeyNotFoundException("Team not found");
        }

        existingTeam.Name = team.Name;
        existingTeam.Description = team.Description;
        existingTeam.UpdatedAt = DateTime.UtcNow;

        _teamRepository.Update(existingTeam);
        await _teamRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var team = await _teamRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (team == null)
        {
            throw new KeyNotFoundException("Team not found");
        }

        _teamRepository.Remove(team);
        await _teamRepository.SaveChangesAsync();
    }
}
