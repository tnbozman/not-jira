using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.ProductDiscovery.Services;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/interviews")]
public class InterviewsController : BaseApiController
{
    private readonly IInterviewService _interviewService;

    public InterviewsController(IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Interview>>> GetInterviews(int projectId)
    {
        var interviews = await _interviewService.GetAllByProjectAsync(projectId);
        return Ok(interviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Interview>> GetInterview(int projectId, int id)
    {
        var interview = await _interviewService.GetByIdAsync(projectId, id);
        if (interview == null)
        {
            return NotFound();
        }

        return Ok(interview);
    }

    [HttpPost]
    public async Task<ActionResult<Interview>> CreateInterview(int projectId, Interview interview)
    {
        try
        {
            var createdInterview = await _interviewService.CreateAsync(projectId, interview);
            return CreatedAtAction(nameof(GetInterview), new { projectId, id = createdInterview.Id }, createdInterview);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInterview(int projectId, int id, Interview interview)
    {
        try
        {
            await _interviewService.UpdateAsync(projectId, id, interview);
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
    public async Task<IActionResult> DeleteInterview(int projectId, int id)
    {
        try
        {
            await _interviewService.DeleteAsync(projectId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/notes")]
    public async Task<ActionResult<InterviewNote>> AddNote(int projectId, int id, InterviewNote note)
    {
        try
        {
            var createdNote = await _interviewService.AddNoteAsync(projectId, id, note);
            return CreatedAtAction(nameof(GetInterview), new { projectId, id }, createdNote);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
