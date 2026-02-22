using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public interface IThemeRepository : IRepository<Theme>
{
    Task<IEnumerable<Theme>> GetByProjectIdAsync(int projectId);
    Task<Theme?> GetWithDetailsAsync(int id);
}
