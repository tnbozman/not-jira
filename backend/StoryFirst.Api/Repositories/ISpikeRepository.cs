using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public interface ISpikeRepository : IRepository<Spike>
{
    Task<IEnumerable<Spike>> GetByEpicIdAsync(int epicId);
    Task<Spike?> GetWithDetailsAsync(int id);
}
