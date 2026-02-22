using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;

namespace NotJira.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TeamsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams(int projectId)
    {
        var teams = await _context.Teams
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .ToListAsync();
            
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeam(int projectId, int id)
    {
        var team = await _context.Teams
            .Where(t => t.ProjectId == projectId && t.Id == id)
            .FirstOrDefaultAsync();

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

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeam), new { projectId, id = team.Id }, team);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int projectId, int id, Team team)
    {
        if (id != team.Id)
        {
            return BadRequest();
        }

        var existingTeam = await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (existingTeam == null)
        {
            return NotFound();
        }

        existingTeam.Name = team.Name;
        existingTeam.Description = team.Description;
        existingTeam.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int projectId, int id)
    {
        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);

        if (team == null)
        {
            return NotFound();
        }

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
