using StoryFirst.Api.Areas.SprintPlanning.Models;

namespace StoryFirst.Api.Areas.SprintPlanning.Services;

public interface IBacklogService
{
    Task<BacklogResponse> GetBacklogAsync(
        int projectId,
        int? teamId = null,
        string? assigneeId = null,
        int? releaseId = null,
        int? epicId = null);
}
