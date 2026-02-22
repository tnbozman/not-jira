using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/problems")]
public class ProblemsController : BaseApiController
{
    private readonly IRepository<Problem> _problemRepository;
    private readonly IExternalEntityRepository _entityRepository;

    public ProblemsController(
        IRepository<Problem> problemRepository,
        IExternalEntityRepository entityRepository)
    {
        _problemRepository = problemRepository;
        _entityRepository = entityRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Problem>>> GetProblems(int projectId)
    {
        var problems = await _problemRepository.FindAsync(p => p.ExternalEntity!.ProjectId == projectId);
        return Ok(problems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Problem>> GetProblem(int projectId, int id)
    {
        var problem = await _problemRepository.FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);

        if (problem == null)
        {
            return NotFound();
        }

        return Ok(problem);
    }

    [HttpPost]
    public async Task<ActionResult<Problem>> CreateProblem(int projectId, Problem problem)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == problem.ExternalEntityId && e.ProjectId == projectId);

        if (entity == null)
        {
            return BadRequest("External entity not found or does not belong to this project");
        }

        problem.CreatedAt = DateTime.UtcNow;
        problem.UpdatedAt = DateTime.UtcNow;

        await _problemRepository.AddAsync(problem);
        await _problemRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProblem), new { projectId, id = problem.Id }, problem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProblem(int projectId, int id, Problem problem)
    {
        if (id != problem.Id)
        {
            return BadRequest();
        }

        var existingProblem = await _problemRepository.FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);

        if (existingProblem == null)
        {
            return NotFound();
        }

        existingProblem.Description = problem.Description;
        existingProblem.Severity = problem.Severity;
        existingProblem.Context = problem.Context;
        existingProblem.ExternalEntityId = problem.ExternalEntityId;
        existingProblem.UpdatedAt = DateTime.UtcNow;

        _problemRepository.Update(existingProblem);
        await _problemRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProblem(int projectId, int id)
    {
        var problem = await _problemRepository.FirstOrDefaultAsync(p => p.Id == id && p.ExternalEntity!.ProjectId == projectId);

        if (problem == null)
        {
            return NotFound();
        }

        _problemRepository.Remove(problem);
        await _problemRepository.SaveChangesAsync();

        return NoContent();
    }
}
