using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProjectManagement.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IRepository<ProjectMember> _projectMemberRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        IRepository<ProjectMember> projectMemberRepository)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _projectRepository.GetAllAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _projectRepository.GetWithMembersAsync(id);
    }

    public async Task<Project?> GetByKeyAsync(string key)
    {
        return await _projectRepository.GetByKeyAsync(key);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Key))
        {
            throw new ArgumentException("Project key is required");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(project.Key, @"^[A-Z0-9-]+$"))
        {
            throw new ArgumentException("Project key must contain only uppercase letters, numbers, and hyphens");
        }

        if (await _projectRepository.AnyAsync(p => p.Key == project.Key))
        {
            throw new InvalidOperationException("A project with this key already exists");
        }

        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();

        return project;
    }

    public async Task UpdateAsync(int id, Project project)
    {
        if (id != project.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        if (existingProject.Key != project.Key)
        {
            throw new InvalidOperationException("Cannot change project key");
        }

        project.UpdatedAt = DateTime.UtcNow;
        project.CreatedAt = existingProject.CreatedAt;
        
        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        _projectRepository.Remove(project);
        await _projectRepository.SaveChangesAsync();
    }

    public async Task<ProjectMember> AddMemberAsync(int projectId, ProjectMember member)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        var existingMember = await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == member.UserId);

        if (existingMember != null)
        {
            throw new InvalidOperationException("User is already a member of this project");
        }

        member.ProjectId = projectId;
        member.AddedAt = DateTime.UtcNow;

        await _projectMemberRepository.AddAsync(member);
        await _projectMemberRepository.SaveChangesAsync();

        return member;
    }

    public async Task<ProjectMember?> GetMemberAsync(int projectId, int memberId)
    {
        return await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Id == memberId);
    }

    public async Task RemoveMemberAsync(int projectId, int memberId)
    {
        var member = await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Id == memberId);

        if (member == null)
        {
            throw new KeyNotFoundException("Member not found");
        }

        _projectMemberRepository.Remove(member);
        await _projectMemberRepository.SaveChangesAsync();
    }

    public async Task UpdateMemberAsync(int projectId, int memberId, ProjectMember member)
    {
        if (projectId != member.ProjectId || memberId != member.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingMember = await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Id == memberId);

        if (existingMember == null)
        {
            throw new KeyNotFoundException("Member not found");
        }

        member.AddedAt = existingMember.AddedAt;

        _projectMemberRepository.Update(member);
        await _projectMemberRepository.SaveChangesAsync();
    }
}
