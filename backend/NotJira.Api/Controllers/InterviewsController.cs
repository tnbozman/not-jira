using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotJira.Api.Data;
using NotJira.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace NotJira.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId}/interviews")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InterviewsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/projects/{projectId}/interviews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Interview>>> GetInterviews(int projectId)
    {
        return await _context.Interviews
            .Where(i => i.ProjectId == projectId)
            .Include(i => i.ExternalEntity)
            .Include(i => i.Notes)
            .OrderByDescending(i => i.InterviewDate)
            .ToListAsync();
    }

    // GET: api/projects/{projectId}/interviews/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Interview>> GetInterview(int projectId, int id)
    {
        var interview = await _context.Interviews
            .Where(i => i.Id == id && i.ProjectId == projectId)
            .Include(i => i.ExternalEntity)
            .Include(i => i.Notes)
                .ThenInclude(n => n.RelatedProblem)
            .Include(i => i.Notes)
                .ThenInclude(n => n.RelatedOutcome)
            .FirstOrDefaultAsync();

        if (interview == null)
        {
            return NotFound();
        }

        return interview;
    }

    // POST: api/projects/{projectId}/interviews
    [HttpPost]
    public async Task<ActionResult<Interview>> CreateInterview(int projectId, Interview interview)
    {
        // Verify the external entity belongs to the project
        var entity = await _context.ExternalEntities
            .FirstOrDefaultAsync(e => e.Id == interview.ExternalEntityId && e.ProjectId == projectId);

        if (entity == null)
        {
            return BadRequest("External entity not found or does not belong to this project");
        }

        interview.ProjectId = projectId;
        interview.CreatedAt = DateTime.UtcNow;
        interview.UpdatedAt = DateTime.UtcNow;

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInterview), new { projectId, id = interview.Id }, interview);
    }

    // PUT: api/projects/{projectId}/interviews/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInterview(int projectId, int id, Interview interview)
    {
        if (id != interview.Id || projectId != interview.ProjectId)
        {
            return BadRequest();
        }

        var existingInterview = await _context.Interviews
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (existingInterview == null)
        {
            return NotFound();
        }

        interview.UpdatedAt = DateTime.UtcNow;
        interview.CreatedAt = existingInterview.CreatedAt;

        _context.Entry(interview).State = EntityState.Modified;
        _context.Entry(interview).Property(i => i.CreatedAt).IsModified = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InterviewExists(id))
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

    // DELETE: api/projects/{projectId}/interviews/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInterview(int projectId, int id)
    {
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (interview == null)
        {
            return NotFound();
        }

        _context.Interviews.Remove(interview);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/projects/{projectId}/interviews/{id}/notes
    [HttpPost("{id}/notes")]
    public async Task<ActionResult<InterviewNote>> AddNote(int projectId, int id, InterviewNote note)
    {
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);

        if (interview == null)
        {
            return NotFound("Interview not found");
        }

        note.InterviewId = id;
        note.CreatedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;

        _context.InterviewNotes.Add(note);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInterview), new { projectId, id }, note);
    }

    private bool InterviewExists(int id)
    {
        return _context.Interviews.Any(e => e.Id == id);
    }
}
