using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public interface IStoryRepository : IRepository<Story>
{
    Task<IEnumerable<Story>> GetByEpicIdAsync(int epicId);
    Task<Story?> GetWithDetailsAsync(int id);
}
