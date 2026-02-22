using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.ProjectManagement.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project?> GetByKeyAsync(string key);
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(int id, Project project);
    Task DeleteAsync(int id);
    Task<ProjectMember> AddMemberAsync(int projectId, ProjectMember member);
    Task<ProjectMember?> GetMemberAsync(int projectId, int memberId);
    Task RemoveMemberAsync(int projectId, int memberId);
    Task UpdateMemberAsync(int projectId, int memberId, ProjectMember member);
}
