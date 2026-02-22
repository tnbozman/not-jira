using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public interface ITeamService
{
    Task<IEnumerable<Team>> GetAllByProjectAsync(int projectId);
    Task<Team?> GetByIdAsync(int projectId, int id);
    Task<Team> CreateAsync(int projectId, Team team);
    Task UpdateAsync(int projectId, int id, Team team);
    Task DeleteAsync(int projectId, int id);
}
