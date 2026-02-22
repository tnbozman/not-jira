using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public interface IExternalEntityService
{
    Task<IEnumerable<ExternalEntity>> GetAllByProjectAsync(int projectId);
    Task<ExternalEntity?> GetByIdAsync(int projectId, int id);
    Task<ExternalEntity?> GetWithDetailsAsync(int projectId, int id);
    Task<ExternalEntity> CreateAsync(int projectId, ExternalEntity entity);
    Task UpdateAsync(int projectId, int id, ExternalEntity entity);
    Task DeleteAsync(int projectId, int id);
}
