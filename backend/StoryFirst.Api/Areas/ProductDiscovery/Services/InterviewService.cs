using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public class InterviewService : IInterviewService
{
    private readonly IRepository<Interview> _interviewRepository;
    private readonly IRepository<InterviewNote> _noteRepository;
    private readonly IExternalEntityRepository _entityRepository;

    public InterviewService(
        IRepository<Interview> interviewRepository,
        IRepository<InterviewNote> noteRepository,
        IExternalEntityRepository entityRepository)
    {
        _interviewRepository = interviewRepository;
        _noteRepository = noteRepository;
        _entityRepository = entityRepository;
    }

    public async Task<IEnumerable<Interview>> GetAllByProjectAsync(int projectId)
    {
        var interviews = (await _interviewRepository.FindAsync(i => i.ProjectId == projectId))
            .OrderByDescending(i => i.InterviewDate)
            .ToList();

        return interviews;
    }

    public async Task<Interview?> GetByIdAsync(int projectId, int id)
    {
        return await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);
    }

    public async Task<Interview> CreateAsync(int projectId, Interview interview)
    {
        var entity = await _entityRepository.FirstOrDefaultAsync(e => e.Id == interview.ExternalEntityId && e.ProjectId == projectId);
        if (entity == null)
        {
            throw new InvalidOperationException("External entity not found or does not belong to this project");
        }

        interview.ProjectId = projectId;
        interview.CreatedAt = DateTime.UtcNow;
        interview.UpdatedAt = DateTime.UtcNow;

        await _interviewRepository.AddAsync(interview);
        await _interviewRepository.SaveChangesAsync();

        return interview;
    }

    public async Task UpdateAsync(int projectId, int id, Interview interview)
    {
        if (id != interview.Id || projectId != interview.ProjectId)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingInterview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);
        if (existingInterview == null)
        {
            throw new KeyNotFoundException("Interview not found");
        }

        existingInterview.Type = interview.Type;
        existingInterview.InterviewDate = interview.InterviewDate;
        existingInterview.Interviewer = interview.Interviewer;
        existingInterview.ExternalEntityId = interview.ExternalEntityId;
        existingInterview.UpdatedAt = DateTime.UtcNow;

        _interviewRepository.Update(existingInterview);
        await _interviewRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId, int id)
    {
        var interview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == projectId);
        if (interview == null)
        {
            throw new KeyNotFoundException("Interview not found");
        }

        _interviewRepository.Remove(interview);
        await _interviewRepository.SaveChangesAsync();
    }

    public async Task<InterviewNote> AddNoteAsync(int projectId, int interviewId, InterviewNote note)
    {
        var interview = await _interviewRepository.FirstOrDefaultAsync(i => i.Id == interviewId && i.ProjectId == projectId);
        if (interview == null)
        {
            throw new KeyNotFoundException("Interview not found");
        }

        note.InterviewId = interviewId;
        note.CreatedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.AddAsync(note);
        await _noteRepository.SaveChangesAsync();

        return note;
    }
}
