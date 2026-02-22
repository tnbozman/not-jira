using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Controllers;

[Area("ProductDiscovery")]
[Route("api/projects/{projectId}/interviews")]
public class InterviewsController : BaseApiController
{
    private readonly IRepository<Interview> _interviewRepository;
    private readonly IRepository<InterviewNote> _noteRepository;
    private readonly IExternalEntityRepository _entityRepository;

    public InterviewsController(
        IRepository<Interview> interviewRepository,
        IRepository<InterviewNote> noteRepository,
        IExternalEntityRepository entityRepository)
    {
        _interviewRepository = interviewRepository;
        _noteRepository = noteRepository;
        _entityRepository = entityRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Interview>>> GetInterviews(int projectId)
    {
        var interviews = (await _interviewRepository.FindAsync(i => i.ProjectId == projectId))
            .OrderByDescending(i => i.InterviewDate)
            .ToList();

        return Ok(interviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Interview>> GetInterview(int projectId, int id)
    {
        var interview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (interview == null)
        {
            return NotFound();
        }

        return Ok(interview);
    }

    [HttpPost]
    public async Task<ActionResult<Interview>> CreateInterview(int projectId, Interview interview)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == interview.ExternalEntityId && e.ProjectId == projectId);

        if (entity == null)
        {
            return BadRequest("External entity not found or does not belong to this project");
        }

        interview.ProjectId = projectId;
        interview.CreatedAt = DateTime.UtcNow;
        interview.UpdatedAt = DateTime.UtcNow;

        await _interviewRepository.AddAsync(interview);
        await _interviewRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInterview), new { projectId, id = interview.Id }, interview);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInterview(int projectId, int id, Interview interview)
    {
        if (id != interview.Id || projectId != interview.ProjectId)
        {
            return BadRequest();
        }

        var existingInterview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (existingInterview == null)
        {
            return NotFound();
        }

        existingInterview.Type = interview.Type;
        existingInterview.InterviewDate = interview.InterviewDate;
        existingInterview.Interviewer = interview.Interviewer;
        existingInterview.ExternalEntityId = interview.ExternalEntityId;
        existingInterview.UpdatedAt = DateTime.UtcNow;

        _interviewRepository.Update(existingInterview);
        await _interviewRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInterview(int projectId, int id)
    {
        var interview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (interview == null)
        {
            return NotFound();
        }

        _interviewRepository.Remove(interview);
        await _interviewRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/notes")]
    public async Task<ActionResult<InterviewNote>> AddNote(int projectId, int id, InterviewNote note)
    {
        var interview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (interview == null)
        {
            return NotFound("Interview not found");
        }

        note.InterviewId = id;
        note.CreatedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.AddAsync(note);
        await _noteRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInterview), new { projectId, id }, note);
    }
}
