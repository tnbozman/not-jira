using StoryFirst.Api.Models;

namespace StoryFirst.Api.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByKeyAsync(string key);
    Task<Project?> GetWithMembersAsync(int id);
}
