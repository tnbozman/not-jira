using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.SprintPlanning.Models;
using StoryFirst.Api.Areas.SprintPlanning.Services;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class SprintsController : BaseApiController
{
    private readonly ISprintService _sprintService;

    public SprintsController(ISprintService sprintService)
    {
        _sprintService = sprintService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sprint>>> GetSprints(int projectId)
    {
        var sprints = await _sprintService.GetAllByProjectAsync(projectId);
        return Ok(sprints);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sprint>> GetSprint(int projectId, int id)
    {
        var sprint = await _sprintService.GetByIdAsync(projectId, id);

        if (sprint == null)
        {
            return NotFound();
        }

        return Ok(sprint);
    }

    [HttpPost]
    public async Task<ActionResult<Sprint>> CreateSprint(int projectId, Sprint sprint)
    {
        var createdSprint = await _sprintService.CreateAsync(projectId, sprint);
        return CreatedAtAction(nameof(GetSprint), new { projectId, id = createdSprint.Id }, createdSprint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSprint(int projectId, int id, Sprint sprint)
    {
        if (id != sprint.Id)
        {
            return BadRequest();
        }

        try
        {
            await _sprintService.UpdateAsync(projectId, id, sprint);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpGet("{id}/planning")]
    public async Task<ActionResult<SprintPlanningResponse>> GetSprintPlanning(int projectId, int id)
    {
        try
        {
            var planning = await _sprintService.GetTeamPlanningAsync(projectId, id);
            return Ok(planning);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}/planning")]
    public async Task<IActionResult> UpdateSprintPlanning(int projectId, int id, [FromBody] SprintPlanningDto dto)
    {
        try
        {
            await _sprintService.SaveTeamPlanningAsync(projectId, id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}/review")]
    public async Task<IActionResult> UpdateSprintReview(int projectId, int id, [FromBody] SprintNotesDto dto)
    {
        try
        {
            await _sprintService.UpdateReviewNotesAsync(projectId, id, dto.Notes);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}/retro")]
    public async Task<IActionResult> UpdateSprintRetro(int projectId, int id, [FromBody] SprintNotesDto dto)
    {
        try
        {
            await _sprintService.UpdateRetroNotesAsync(projectId, id, dto.Notes);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    public class SprintNotesDto
    {
        public string? Notes { get; set; }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSprint(int projectId, int id)
    {
        try
        {
            await _sprintService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
