using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public interface IStoryService
{
    Task<IEnumerable<Story>> GetByEpicIdAsync(int epicId);
    Task<Story?> GetByIdAsync(int epicId, int id);
    Task<Story> CreateAsync(int epicId, Story story);
    Task UpdateAsync(int epicId, int id, Story story);
    Task DeleteAsync(int epicId, int id);
}
