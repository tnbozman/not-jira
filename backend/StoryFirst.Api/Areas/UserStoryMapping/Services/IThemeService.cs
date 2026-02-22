using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public interface IThemeService
{
    Task<IEnumerable<object>> GetAllByProjectAsync(int projectId);
    Task<object?> GetByIdAsync(int projectId, int id);
    Task<Theme> CreateAsync(int projectId, Theme theme);
    Task UpdateAsync(int projectId, int id, Theme theme);
    Task DeleteAsync(int projectId, int id);
}
