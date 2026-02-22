using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public class TagService : ITagService
{
    private readonly IRepository<Tag> _tagRepository;
    private readonly IProjectRepository _projectRepository;

    public TagService(
        IRepository<Tag> tagRepository,
        IProjectRepository projectRepository)
    {
        _tagRepository = tagRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<Tag>> GetAllByProjectAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        var tags = (await _tagRepository.FindAsync(t => t.ProjectId == projectId))
            .OrderBy(t => t.Name)
            .ToList();

        return tags;
    }

    public async Task<Tag?> GetByIdAsync(int projectId, int id)
    {
        return await _tagRepository.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == id);
    }

    public async Task<Tag> CreateAsync(int projectId, Tag tag)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        if (await _tagRepository.AnyAsync(t => t.ProjectId == projectId && t.Name == tag.Name))
        {
            throw new InvalidOperationException("A tag with this name already exists in this project");
        }

        tag.ProjectId = projectId;
        tag.CreatedAt = DateTime.UtcNow;

        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync();

        return tag;
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var tag = await _tagRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);
        if (tag == null)
        {
            throw new KeyNotFoundException("Tag not found");
        }

        _tagRepository.Remove(tag);
        await _tagRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<Tag>> SearchAsync(int projectId, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query parameter is required");
        }

        var tags = (await _tagRepository.FindAsync(t => t.ProjectId == projectId && 
               (t.Name.Contains(query) || (t.Description != null && t.Description.Contains(query)))))
            .OrderBy(t => t.Name)
            .Take(20)
            .ToList();

        return tags;
    }
}
