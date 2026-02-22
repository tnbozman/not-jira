using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace NotJira.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId}/external-entities")]
[Authorize]
public class ExternalEntitiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExternalEntitiesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/projects/{projectId}/external-entities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExternalEntity>>> GetExternalEntities(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        return await _context.ExternalEntities
            .Where(e => e.ProjectId == projectId)
            .Include(e => e.Problems)
            .Include(e => e.EntityTags)
                .ThenInclude(et => et.Tag)
            .ToListAsync();
    }

    // GET: api/projects/{projectId}/external-entities/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ExternalEntity>> GetExternalEntity(int projectId, int id)
    {
        var entity = await _context.ExternalEntities
            .Where(e => e.ProjectId == projectId && e.Id == id)
            .Include(e => e.Problems)
                .ThenInclude(p => p.Outcomes)
            .Include(e => e.Interviews)
            .Include(e => e.EntityTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return NotFound();
        }

        return entity;
    }

    // POST: api/projects/{projectId}/external-entities
    [HttpPost]
    public async Task<ActionResult<ExternalEntity>> CreateExternalEntity(int projectId, ExternalEntity entity)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        entity.ProjectId = projectId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.ExternalEntities.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExternalEntity), new { projectId, id = entity.Id }, entity);
    }

    // PUT: api/projects/{projectId}/external-entities/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExternalEntity(int projectId, int id, ExternalEntity entity)
    {
        if (id != entity.Id || projectId != entity.ProjectId)
        {
            return BadRequest();
        }

        var existingEntity = await _context.ExternalEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.ProjectId == projectId);

        if (existingEntity == null)
        {
            return NotFound();
        }

        entity.UpdatedAt = DateTime.UtcNow;
        entity.CreatedAt = existingEntity.CreatedAt;

        _context.Entry(entity).State = EntityState.Modified;
        _context.Entry(entity).Property(e => e.CreatedAt).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExternalEntityExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/projects/{projectId}/external-entities/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExternalEntity(int projectId, int id)
    {
        var entity = await _context.ExternalEntities
            .FirstOrDefaultAsync(e => e.Id == id && e.ProjectId == projectId);

        if (entity == null)
        {
            return NotFound();
        }

        _context.ExternalEntities.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExternalEntityExists(int id)
    {
        return _context.ExternalEntities.Any(e => e.Id == id);
    }
}
