using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public class ExternalEntityService : IExternalEntityService
{
    private readonly IExternalEntityRepository _entityRepository;
    private readonly IProjectRepository _projectRepository;

    public ExternalEntityService(
        IExternalEntityRepository entityRepository,
        IProjectRepository projectRepository)
    {
        _entityRepository = entityRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ExternalEntity>> GetAllByProjectAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        return await _entityRepository.GetByProjectIdAsync(projectId);
    }

    public async Task<ExternalEntity?> GetByIdAsync(int projectId, int id)
    {
        return await _entityRepository.FirstOrDefaultAsync(e => e.ProjectId == projectId && e.Id == id);
    }

    public async Task<ExternalEntity?> GetWithDetailsAsync(int projectId, int id)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.ProjectId == projectId && e.Id == id);
        if (entity == null)
        {
            return null;
        }

        return await _entityRepository.GetWithDetailsAsync(id);
    }

    public async Task<ExternalEntity> CreateAsync(int projectId, ExternalEntity entity)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        entity.ProjectId = projectId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _entityRepository.AddAsync(entity);
        await _entityRepository.SaveChangesAsync();

        return entity;
    }

    public async Task UpdateAsync(int projectId, int id, ExternalEntity entity)
    {
        if (id != entity.Id || projectId != entity.ProjectId)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingEntity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == id && e.ProjectId == projectId);
        if (existingEntity == null)
        {
            throw new KeyNotFoundException("External entity not found");
        }

        existingEntity.Name = entity.Name;
        existingEntity.Type = entity.Type;
        existingEntity.Email = entity.Email;
        existingEntity.Organization = entity.Organization;
        existingEntity.Phone = entity.Phone;
        existingEntity.Notes = entity.Notes;
        existingEntity.UpdatedAt = DateTime.UtcNow;

        _entityRepository.Update(existingEntity);
        await _entityRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == id && e.ProjectId == projectId);
        if (entity == null)
        {
            throw new KeyNotFoundException("External entity not found");
        }

        _entityRepository.Remove(entity);
        await _entityRepository.SaveChangesAsync();
    }
}
