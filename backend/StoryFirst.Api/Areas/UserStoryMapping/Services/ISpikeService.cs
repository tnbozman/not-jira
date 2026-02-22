using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public interface ISpikeService
{
    Task<IEnumerable<Spike>> GetByEpicIdAsync(int epicId);
    Task<Spike?> GetByIdAsync(int epicId, int id);
    Task<Spike> CreateAsync(int epicId, Spike spike);
    Task UpdateAsync(int epicId, int id, Spike spike);
    Task DeleteAsync(int epicId, int id);
}
