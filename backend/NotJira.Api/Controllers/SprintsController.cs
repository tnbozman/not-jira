using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
public class SprintsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SprintsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sprint>>> GetSprints(int projectId)
    {
        var sprints = await _context.Sprints
            .Where(s => s.ProjectId == projectId)
            .Include(s => s.Stories)
            .Include(s => s.Spikes)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();
            
        return Ok(sprints);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sprint>> GetSprint(int projectId, int id)
    {
        var sprint = await _context.Sprints
            .Where(s => s.ProjectId == projectId && s.Id == id)
            .Include(s => s.Stories)
            .Include(s => s.Spikes)
            .FirstOrDefaultAsync();

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

        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSprint), new { projectId, id = sprint.Id }, sprint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSprint(int projectId, int id, Sprint sprint)
    {
        if (id != sprint.Id)
        {
            return BadRequest();
        }

        var existingSprint = await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

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

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/planning")]
    public async Task<ActionResult> GetSprintPlanning(int projectId, int id)
    {
        var sprint = await _context.Sprints
            .Where(s => s.ProjectId == projectId && s.Id == id)
            .Include(s => s.TeamPlannings)
                .ThenInclude(tp => tp.Team)
            .FirstOrDefaultAsync();

        if (sprint == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            sprint.Id,
            sprint.Name,
            sprint.PlanningOneNotes,
            TeamPlannings = sprint.TeamPlannings.Select(tp => new
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
        var sprint = await _context.Sprints
            .Where(s => s.ProjectId == projectId && s.Id == id)
            .Include(s => s.TeamPlannings)
            .FirstOrDefaultAsync();

        if (sprint == null)
        {
            return NotFound();
        }

        sprint.PlanningOneNotes = dto.PlanningOneNotes;
        sprint.UpdatedAt = DateTime.UtcNow;

        // Update or create team plannings
        if (dto.TeamPlannings != null)
        {
            foreach (var teamPlanningDto in dto.TeamPlannings)
            {
                var existing = sprint.TeamPlannings.FirstOrDefault(tp => tp.TeamId == teamPlanningDto.TeamId);
                if (existing != null)
                {
                    existing.PlanningTwoNotes = teamPlanningDto.PlanningTwoNotes;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    sprint.TeamPlannings.Add(new TeamPlanning
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

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/review")]
    public async Task<IActionResult> UpdateSprintReview(int projectId, int id, [FromBody] SprintNotesDto dto)
    {
        var sprint = await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        sprint.ReviewNotes = dto.Notes;
        sprint.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/retro")]
    public async Task<IActionResult> UpdateSprintRetro(int projectId, int id, [FromBody] SprintNotesDto dto)
    {
        var sprint = await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        sprint.RetroNotes = dto.Notes;
        sprint.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

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
        var sprint = await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id && s.ProjectId == projectId);

        if (sprint == null)
        {
            return NotFound();
        }

        _context.Sprints.Remove(sprint);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
