using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public interface IEpicService
{
    Task<IEnumerable<Epic>> GetByThemeIdAsync(int themeId);
    Task<Epic?> GetByIdAsync(int themeId, int id);
    Task<Epic> CreateAsync(int themeId, Epic epic);
    Task UpdateAsync(int themeId, int id, Epic epic);
    Task DeleteAsync(int themeId, int id);
}
