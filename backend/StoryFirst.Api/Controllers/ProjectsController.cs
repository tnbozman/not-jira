using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Data;
using StoryFirst.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace StoryFirst.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        return await _context.Projects
            .Include(p => p.Members)
            .ToListAsync();
    }

    // GET: api/projects/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return project;
    }

    // GET: api/projects/by-key/{key}
    [HttpGet("by-key/{key}")]
    public async Task<ActionResult<Project>> GetProjectByKey(string key)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Key == key);

        if (project == null)
        {
            return NotFound();
        }

        return project;
    }

    // POST: api/projects
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        // Validate project key
        if (string.IsNullOrWhiteSpace(project.Key))
        {
            return BadRequest("Project key is required");
        }

        // Validate key format (alphanumeric and hyphens only, uppercase)
        if (!System.Text.RegularExpressions.Regex.IsMatch(project.Key, @"^[A-Z0-9-]+$"))
        {
            return BadRequest("Project key must contain only uppercase letters, numbers, and hyphens");
        }

        // Check for duplicate key
        if (await _context.Projects.AnyAsync(p => p.Key == project.Key))
        {
            return Conflict("A project with this key already exists");
        }

        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    // PUT: api/projects/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, Project project)
    {
        if (id != project.Id)
        {
            return BadRequest();
        }

        // Don't allow changing the key
        var existingProject = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (existingProject == null)
        {
            return NotFound();
        }

        if (existingProject.Key != project.Key)
        {
            return BadRequest("Cannot change project key");
        }

        project.UpdatedAt = DateTime.UtcNow;
        project.CreatedAt = existingProject.CreatedAt; // Preserve original creation date
        
        _context.Entry(project).State = EntityState.Modified;
        _context.Entry(project).Property(p => p.CreatedAt).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(id))
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

    // DELETE: api/projects/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/projects/{id}/members
    [HttpPost("{id}/members")]
    public async Task<ActionResult<ProjectMember>> AddMember(int id, ProjectMember member)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        // Check if user is already a member
        var existingMember = await _context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.UserId == member.UserId);

        if (existingMember != null)
        {
            return Conflict("User is already a member of this project");
        }

        member.ProjectId = id;
        member.AddedAt = DateTime.UtcNow;

        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjectMember), new { id = id, memberId = member.Id }, member);
    }

    // GET: api/projects/{id}/members/{memberId}
    [HttpGet("{id}/members/{memberId}")]
    public async Task<ActionResult<ProjectMember>> GetProjectMember(int id, int memberId)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.Id == memberId);

        if (member == null)
        {
            return NotFound();
        }

        return member;
    }

    // DELETE: api/projects/{id}/members/{memberId}
    [HttpDelete("{id}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(int id, int memberId)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.Id == memberId);

        if (member == null)
        {
            return NotFound();
        }

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/projects/{id}/members/{memberId}
    [HttpPut("{id}/members/{memberId}")]
    public async Task<IActionResult> UpdateMember(int id, int memberId, ProjectMember member)
    {
        if (id != member.ProjectId || memberId != member.Id)
        {
            return BadRequest();
        }

        var existingMember = await _context.ProjectMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.Id == memberId);

        if (existingMember == null)
        {
            return NotFound();
        }

        // Preserve the original AddedAt timestamp
        member.AddedAt = existingMember.AddedAt;

        _context.Entry(member).State = EntityState.Modified;
        _context.Entry(member).Property(m => m.AddedAt).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.ProjectMembers.Any(m => m.Id == memberId))
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

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }
}
