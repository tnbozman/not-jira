using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.ProductDiscovery.Services;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/problems")]
public class ProblemsController : BaseApiController
{
    private readonly IProblemService _problemService;

    public ProblemsController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Problem>>> GetProblems(int projectId)
    {
        var problems = await _problemService.GetAllByProjectAsync(projectId);
        return Ok(problems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Problem>> GetProblem(int projectId, int id)
    {
        var problem = await _problemService.GetByIdAsync(projectId, id);
        if (problem == null)
        {
            return NotFound();
        }

        return Ok(problem);
    }

    [HttpPost]
    public async Task<ActionResult<Problem>> CreateProblem(int projectId, Problem problem)
    {
        try
        {
            var createdProblem = await _problemService.CreateAsync(projectId, problem);
            return CreatedAtAction(nameof(GetProblem), new { projectId, id = createdProblem.Id }, createdProblem);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProblem(int projectId, int id, Problem problem)
    {
        try
        {
            await _problemService.UpdateAsync(projectId, id, problem);
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
    public async Task<IActionResult> DeleteProblem(int projectId, int id)
    {
        try
        {
            await _problemService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
