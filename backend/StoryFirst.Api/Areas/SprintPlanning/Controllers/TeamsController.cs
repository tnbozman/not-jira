using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class TeamsController : BaseApiController
{
    private readonly IRepository<Team> _teamRepository;

    public TeamsController(IRepository<Team> teamRepository)
    {
        _teamRepository = teamRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams(int projectId)
    {
        var teams = (await _teamRepository.FindAsync(t => t.ProjectId == projectId))
            .OrderBy(t => t.Name)
            .ToList();
            
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeam(int projectId, int id)
    {
        var team = await _teamRepository.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == id);

        if (team == null)
        {
            return NotFound();
        }

        return Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<Team>> CreateTeam(int projectId, Team team)
    {
        team.ProjectId = projectId;
        team.CreatedAt = DateTime.UtcNow;
        team.UpdatedAt = DateTime.UtcNow;

        await _teamRepository.AddAsync(team);
        await _teamRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeam), new { projectId, id = team.Id }, team);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int projectId, int id, Team team)
    {
        if (id != team.Id)
        {
            return BadRequest();
        }

        var existingTeam = await _teamRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (existingTeam == null)
        {
            return NotFound();
        }

        existingTeam.Name = team.Name;
        existingTeam.Description = team.Description;
        existingTeam.UpdatedAt = DateTime.UtcNow;

        _teamRepository.Update(existingTeam);
        await _teamRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int projectId, int id)
    {
        var team = await _teamRepository.FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (team == null)
        {
            return NotFound();
        }

        _teamRepository.Remove(team);
        await _teamRepository.SaveChangesAsync();

        return NoContent();
    }
}
