using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public interface IEpicRepository : IRepository<Epic>
{
    Task<IEnumerable<Epic>> GetByThemeIdAsync(int themeId);
    Task<Epic?> GetWithDetailsAsync(int id);
}
