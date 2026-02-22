using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.SprintPlanning.Services;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class TeamsController : BaseApiController
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams(int projectId)
    {
        var teams = await _teamService.GetAllByProjectAsync(projectId);
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeam(int projectId, int id)
    {
        var team = await _teamService.GetByIdAsync(projectId, id);

        if (team == null)
        {
            return NotFound();
        }

        return Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<Team>> CreateTeam(int projectId, Team team)
    {
        var createdTeam = await _teamService.CreateAsync(projectId, team);
        return CreatedAtAction(nameof(GetTeam), new { projectId, id = createdTeam.Id }, createdTeam);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int projectId, int id, Team team)
    {
        if (id != team.Id)
        {
            return BadRequest();
        }

        try
        {
            await _teamService.UpdateAsync(projectId, id, team);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int projectId, int id)
    {
        try
        {
            await _teamService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
