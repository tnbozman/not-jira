using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public interface IProblemService
{
    Task<IEnumerable<Problem>> GetAllByProjectAsync(int projectId);
    Task<Problem?> GetByIdAsync(int projectId, int id);
    Task<Problem> CreateAsync(int projectId, Problem problem);
    Task UpdateAsync(int projectId, int id, Problem problem);
    Task DeleteAsync(int projectId, int id);
}
