using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/external-entities")]
public class ExternalEntitiesController : BaseApiController
{
    private readonly IExternalEntityRepository _entityRepository;
    private readonly IProjectRepository _projectRepository;

    public ExternalEntitiesController(
        IExternalEntityRepository entityRepository,
        IProjectRepository projectRepository)
    {
        _entityRepository = entityRepository;
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExternalEntity>>> GetExternalEntities(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        var entities = await _entityRepository.GetByProjectIdAsync(projectId);
        return Ok(entities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExternalEntity>> GetExternalEntity(int projectId, int id)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.ProjectId == projectId && e.Id == id);

        if (entity == null)
        {
            return NotFound();
        }

        var entityDetails = await _entityRepository.GetWithDetailsAsync(id);
        return Ok(entityDetails);
    }

    [HttpPost]
    public async Task<ActionResult<ExternalEntity>> CreateExternalEntity(int projectId, ExternalEntity entity)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        entity.ProjectId = projectId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _entityRepository.AddAsync(entity);
        await _entityRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExternalEntity), new { projectId, id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExternalEntity(int projectId, int id, ExternalEntity entity)
    {
        if (id != entity.Id || projectId != entity.ProjectId)
        {
            return BadRequest();
        }

        var existingEntity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == id && e.ProjectId == projectId);

        if (existingEntity == null)
        {
            return NotFound();
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

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExternalEntity(int projectId, int id)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == id && e.ProjectId == projectId);

        if (entity == null)
        {
            return NotFound();
        }

        _entityRepository.Remove(entity);
        await _entityRepository.SaveChangesAsync();

        return NoContent();
    }
}
