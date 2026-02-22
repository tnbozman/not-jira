using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public interface IReleaseService
{
    Task<IEnumerable<Release>> GetAllByProjectAsync(int projectId);
    Task<Release?> GetByIdAsync(int projectId, int id);
    Task<Release> CreateAsync(int projectId, Release release);
    Task UpdateAsync(int projectId, int id, Release release);
    Task DeleteAsync(int projectId, int id);
}
