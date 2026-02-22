using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.ProductDiscovery.Services;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/external-entities")]
public class ExternalEntitiesController : BaseApiController
{
    private readonly IExternalEntityService _entityService;

    public ExternalEntitiesController(IExternalEntityService entityService)
    {
        _entityService = entityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExternalEntity>>> GetExternalEntities(int projectId)
    {
        try
        {
            var entities = await _entityService.GetAllByProjectAsync(projectId);
            return Ok(entities);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExternalEntity>> GetExternalEntity(int projectId, int id)
    {
        var entityDetails = await _entityService.GetWithDetailsAsync(projectId, id);
        if (entityDetails == null)
        {
            return NotFound();
        }

        return Ok(entityDetails);
    }

    [HttpPost]
    public async Task<ActionResult<ExternalEntity>> CreateExternalEntity(int projectId, ExternalEntity entity)
    {
        try
        {
            var createdEntity = await _entityService.CreateAsync(projectId, entity);
            return CreatedAtAction(nameof(GetExternalEntity), new { projectId, id = createdEntity.Id }, createdEntity);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExternalEntity(int projectId, int id, ExternalEntity entity)
    {
        try
        {
            await _entityService.UpdateAsync(projectId, id, entity);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExternalEntity(int projectId, int id)
    {
        try
        {
            await _entityService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
