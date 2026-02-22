using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Models;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Areas.ProjectManagement.Services;

namespace StoryFirst.Api.Areas.ProjectManagement.Controllers;

[Area("ProjectManagement")]
[Route("api/[controller]")]
public class ProjectsController : BaseApiController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    // GET: api/projects/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _projectService.GetByIdAsync(id);

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
        var project = await _projectService.GetByKeyAsync(key);

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
        try
        {
            var result = await _projectService.CreateAsync(project);
            return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // PUT: api/projects/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, Project project)
    {
        try
        {
            await _projectService.UpdateAsync(id, project);
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/projects/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            await _projectService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST: api/projects/{id}/members
    [HttpPost("{id}/members")]
    public async Task<ActionResult<ProjectMember>> AddMember(int id, ProjectMember member)
    {
        try
        {
            var result = await _projectService.AddMemberAsync(id, member);
            return CreatedAtAction(nameof(GetProjectMember), new { id = id, memberId = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // GET: api/projects/{id}/members/{memberId}
    [HttpGet("{id}/members/{memberId}")]
    public async Task<ActionResult<ProjectMember>> GetProjectMember(int id, int memberId)
    {
        var member = await _projectService.GetMemberAsync(id, memberId);

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
        try
        {
            await _projectService.RemoveMemberAsync(id, memberId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // PUT: api/projects/{id}/members/{memberId}
    [HttpPut("{id}/members/{memberId}")]
    public async Task<IActionResult> UpdateMember(int id, int memberId, ProjectMember member)
    {
        try
        {
            await _projectService.UpdateMemberAsync(id, memberId, member);
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
}
