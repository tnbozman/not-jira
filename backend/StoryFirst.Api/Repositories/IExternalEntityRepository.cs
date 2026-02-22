using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public interface IExternalEntityRepository : IRepository<ExternalEntity>
{
    Task<IEnumerable<ExternalEntity>> GetByProjectIdAsync(int projectId);
    Task<ExternalEntity?> GetWithDetailsAsync(int id);
}
