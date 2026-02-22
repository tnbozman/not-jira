using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.ProductDiscovery.Services;

public interface IInterviewService
{
    Task<IEnumerable<Interview>> GetAllByProjectAsync(int projectId);
    Task<Interview?> GetByIdAsync(int projectId, int id);
    Task<Interview> CreateAsync(int projectId, Interview interview);
    Task UpdateAsync(int projectId, int id, Interview interview);
    Task DeleteAsync(int projectId, int id);
    Task<InterviewNote> AddNoteAsync(int projectId, int interviewId, InterviewNote note);
}
