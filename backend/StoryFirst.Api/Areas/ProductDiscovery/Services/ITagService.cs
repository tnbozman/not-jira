using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllByProjectAsync(int projectId);
    Task<Tag?> GetByIdAsync(int projectId, int id);
    Task<Tag> CreateAsync(int projectId, Tag tag);
    Task DeleteAsync(int projectId, int id);
    Task<IEnumerable<Tag>> SearchAsync(int projectId, string query);
}
