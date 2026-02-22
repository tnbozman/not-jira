using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using StoryFirst.Api.Common.Controllers;

namespace StoryFirst.Api.Areas.ProjectManagement.Controllers;

[Area("ProjectManagement")]
[Route("api/[controller]")]
public class ProjectsController : BaseApiController
{
    private readonly IProjectRepository _projectRepository;
    private readonly IRepository<ProjectMember> _projectMemberRepository;

    public ProjectsController(
        IProjectRepository projectRepository,
        IRepository<ProjectMember> projectMemberRepository)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var projects = await _projectRepository.GetAllAsync();
        return Ok(projects);
    }

    // GET: api/projects/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _projectRepository.GetWithMembersAsync(id);

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
        var project = await _projectRepository.GetByKeyAsync(key);

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
        if (await _projectRepository.AnyAsync(p => p.Key == project.Key))
        {
            return Conflict("A project with this key already exists");
        }

        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();

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
        var existingProject = await _projectRepository.GetByIdAsync(id);
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
        
        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/projects/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _projectRepository.Remove(project);
        await _projectRepository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/projects/{id}/members
    [HttpPost("{id}/members")]
    public async Task<ActionResult<ProjectMember>> AddMember(int id, ProjectMember member)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        // Check if user is already a member
        var existingMember = await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.UserId == member.UserId);

        if (existingMember != null)
        {
            return Conflict("User is already a member of this project");
        }

        member.ProjectId = id;
        member.AddedAt = DateTime.UtcNow;

        await _projectMemberRepository.AddAsync(member);
        await _projectMemberRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjectMember), new { id = id, memberId = member.Id }, member);
    }

    // GET: api/projects/{id}/members/{memberId}
    [HttpGet("{id}/members/{memberId}")]
    public async Task<ActionResult<ProjectMember>> GetProjectMember(int id, int memberId)
    {
        var member = await _projectMemberRepository
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
        var member = await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.Id == memberId);

        if (member == null)
        {
            return NotFound();
        }

        _projectMemberRepository.Remove(member);
        await _projectMemberRepository.SaveChangesAsync();

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

        var existingMember = await _projectMemberRepository
            .FirstOrDefaultAsync(m => m.ProjectId == id && m.Id == memberId);

        if (existingMember == null)
        {
            return NotFound();
        }

        // Preserve the original AddedAt timestamp
        member.AddedAt = existingMember.AddedAt;

        _projectMemberRepository.Update(member);
        await _projectMemberRepository.SaveChangesAsync();

        return NoContent();
    }
}
