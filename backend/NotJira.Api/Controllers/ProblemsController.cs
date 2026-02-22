using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace NotJira.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId}/problems")]
[Authorize]
public class ProblemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProblemsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/projects/{projectId}/problems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Problem>>> GetProblems(int projectId)
    {
        return await _context.Problems
            .Where(p => p.ExternalEntity!.ProjectId == projectId)
            .Include(p => p.ExternalEntity)
            .Include(p => p.Outcomes)
            .Include(p => p.ProblemTags)
                .ThenInclude(pt => pt.Tag)
            .ToListAsync();
    }

    // GET: api/projects/{projectId}/problems/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Problem>> GetProblem(int projectId, int id)
    {
        var problem = await _context.Problems
            .Where(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId)
            .Include(p => p.ExternalEntity)
            .Include(p => p.Outcomes)
                .ThenInclude(o => o.SuccessMetrics)
            .Include(p => p.ProblemTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync();

        if (problem == null)
        {
            return NotFound();
        }

        return problem;
    }

    // POST: api/projects/{projectId}/problems
    [HttpPost]
    public async Task<ActionResult<Problem>> CreateProblem(int projectId, Problem problem)
    {
        // Verify the external entity belongs to the project
        var entity = await _context.ExternalEntities
            .FirstOrDefaultAsync(e => e.Id == problem.ExternalEntityId && e.ProjectId == projectId);

        if (entity == null)
        {
            return BadRequest("External entity not found or does not belong to this project");
        }

        problem.CreatedAt = DateTime.UtcNow;
        problem.UpdatedAt = DateTime.UtcNow;

        _context.Problems.Add(problem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProblem), new { projectId, id = problem.Id }, problem);
    }

    // PUT: api/projects/{projectId}/problems/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProblem(int projectId, int id, Problem problem)
    {
        if (id != problem.Id)
        {
            return BadRequest();
        }

        var existingProblem = await _context.Problems
            .Include(p => p.ExternalEntity)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);

        if (existingProblem == null)
        {
            return NotFound();
        }

        problem.UpdatedAt = DateTime.UtcNow;
        problem.CreatedAt = existingProblem.CreatedAt;

        _context.Entry(problem).State = EntityState.Modified;
        _context.Entry(problem).Property(p => p.CreatedAt).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProblemExists(id))
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

    // DELETE: api/projects/{projectId}/problems/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProblem(int projectId, int id)
    {
        var problem = await _context.Problems
            .Include(p => p.ExternalEntity)
            .FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);

        if (problem == null)
        {
            return NotFound();
        }

        _context.Problems.Remove(problem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProblemExists(int id)
    {
        return _context.Problems.Any(e => e.Id == id);
    }
}
