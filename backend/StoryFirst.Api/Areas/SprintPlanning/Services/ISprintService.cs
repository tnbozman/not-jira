using StoryFirst.Api.Areas.SprintPlanning.Models;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public interface ISprintService
{
    Task<IEnumerable<Sprint>> GetAllByProjectAsync(int projectId);
    Task<Sprint?> GetByIdAsync(int projectId, int id);
    Task<Sprint> CreateAsync(int projectId, Sprint sprint);
    Task UpdateAsync(int projectId, int id, Sprint sprint);
    Task DeleteAsync(int projectId, int id);
    Task<SprintPlanningResponse> GetTeamPlanningAsync(int projectId, int sprintId);
    Task SaveTeamPlanningAsync(int projectId, int sprintId, SprintPlanningDto planning);
    Task UpdateReviewNotesAsync(int projectId, int sprintId, string? notes);
    Task UpdateRetroNotesAsync(int projectId, int sprintId, string? notes);
}
