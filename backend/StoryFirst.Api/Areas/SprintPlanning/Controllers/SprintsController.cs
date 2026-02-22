using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class SprintsController : BaseApiController
{
    private readonly IRepository<Sprint> _sprintRepository;
    private readonly IRepository<TeamPlanning> _teamPlanningRepository;

    public SprintsController(
        IRepository<Sprint> sprintRepository,
        IRepository<TeamPlanning> teamPlanningRepository)
    {
        _sprintRepository = sprintRepository;
        _teamPlanningRepository = teamPlanningRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sprint>>> GetSprints(int projectId)
    {
        var sprints = (await _sprintRepository.FindAsync(s => s.ProjectId == projectId))
            .OrderByDescending(s => s.StartDate)
            .ToList();
            
        return Ok(sprints);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sprint>> GetSprint(int projectId, int id)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Id == id);

        if (sprint == null)
        {
            return NotFound();
        }

        return Ok(sprint);
    }

    [HttpPost]
    public async Task<ActionResult<Sprint>> CreateSprint(int projectId, Sprint sprint)
    {
        sprint.ProjectId = projectId;
        sprint.CreatedAt = DateTime.UtcNow;
        sprint.UpdatedAt = DateTime.UtcNow;

        await _sprintRepository.AddAsync(sprint);
        await _sprintRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSprint), new { projectId, id = sprint.Id }, sprint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSprint(int projectId, int id, Sprint sprint)
    {
        if (id != sprint.Id)
        {
            return BadRequest();
        }

        var existingSprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (existingSprint == null)
        {
            return NotFound();
        }

        existingSprint.Name = sprint.Name;
        existingSprint.Goal = sprint.Goal;
        existingSprint.StartDate = sprint.StartDate;
        existingSprint.EndDate = sprint.EndDate;
        existingSprint.Status = sprint.Status;
        existingSprint.PlanningOneNotes = sprint.PlanningOneNotes;
        existingSprint.ReviewNotes = sprint.ReviewNotes;
        existingSprint.RetroNotes = sprint.RetroNotes;
        existingSprint.UpdatedAt = DateTime.UtcNow;

        _sprintRepository.Update(existingSprint);
        await _sprintRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/planning")]
    public async Task<ActionResult> GetSprintPlanning(int projectId, int id)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Id == id);

        if (sprint == null)
        {
            return NotFound();
        }

        var teamPlannings = await _teamPlanningRepository.FindAsync(tp => tp.SprintId == id);

        return Ok(new
        {
            sprint.Id,
            sprint.Name,
            sprint.PlanningOneNotes,
            TeamPlannings = teamPlannings.Select(tp => new
            {
                tp.Id,
                tp.TeamId,
                TeamName = tp.Team?.Name,
                tp.PlanningTwoNotes
            })
        });
    }

    [HttpPut("{id}/planning")]
    public async Task<IActionResult> UpdateSprintPlanning(int projectId, int id, [FromBody] SprintPlanningDto dto)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Id == id);

        if (sprint == null)
        {
            return NotFound();
        }

        sprint.PlanningOneNotes = dto.PlanningOneNotes;
        sprint.UpdatedAt = DateTime.UtcNow;

        if (dto.TeamPlannings != null)
        {
            var existingPlannings = await _teamPlanningRepository.FindAsync(tp => tp.SprintId == sprint.Id);
            
            foreach (var teamPlanningDto in dto.TeamPlannings)
            {
                var existing = existingPlannings.FirstOrDefault(tp => tp.TeamId == teamPlanningDto.TeamId);
                if (existing != null)
                {
                    existing.PlanningTwoNotes = teamPlanningDto.PlanningTwoNotes;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _teamPlanningRepository.Update(existing);
                }
                else
                {
                    await _teamPlanningRepository.AddAsync(new TeamPlanning
                    {
                        SprintId = sprint.Id,
                        TeamId = teamPlanningDto.TeamId,
                        PlanningTwoNotes = teamPlanningDto.PlanningTwoNotes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        _sprintRepository.Update(sprint);
        await _sprintRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/review")]
    public async Task<IActionResult> UpdateSprintReview(int projectId, int id, [FromBody] SprintNotesDto dto)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        sprint.ReviewNotes = dto.Notes;
        sprint.UpdatedAt = DateTime.UtcNow;

        _sprintRepository.Update(sprint);
        await _sprintRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/retro")]
    public async Task<IActionResult> UpdateSprintRetro(int projectId, int id, [FromBody] SprintNotesDto dto)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        sprint.RetroNotes = dto.Notes;
        sprint.UpdatedAt = DateTime.UtcNow;

        _sprintRepository.Update(sprint);
        await _sprintRepository.SaveChangesAsync();

        return NoContent();
    }

    public class SprintPlanningDto
    {
        public string? PlanningOneNotes { get; set; }
        public List<TeamPlanningDto>? TeamPlannings { get; set; }
    }

    public class TeamPlanningDto
    {
        public int TeamId { get; set; }
        public string? PlanningTwoNotes { get; set; }
    }

    public class SprintNotesDto
    {
        public string? Notes { get; set; }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSprint(int projectId, int id)
    {
        var sprint = await _sprintRepository.FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        _sprintRepository.Remove(sprint);
        await _sprintRepository.SaveChangesAsync();

        return NoContent();
    }
}
