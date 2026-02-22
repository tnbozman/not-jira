using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Areas.SprintPlanning.Models;
using StoryFirst.Api.Areas.SprintPlanning.Services;

namespace StoryFirst.Api.Areas.SprintPlanning.Controllers;

[Area("SprintPlanning")]
[Route("api/projects/{projectId}/[controller]")]
public class BacklogController : BaseApiController
{
    private readonly IBacklogService _backlogService;

    public BacklogController(IBacklogService backlogService)
    {
        _backlogService = backlogService;
    }

    [HttpGet]
    public async Task<ActionResult<BacklogResponse>> GetBacklog(
        int projectId,
        [FromQuery] int? teamId = null,
        [FromQuery] string? assigneeId = null,
        [FromQuery] int? releaseId = null,
        [FromQuery] int? epicId = null)
    {
        var response = await _backlogService.GetBacklogAsync(projectId, teamId, assigneeId, releaseId, epicId);
        return Ok(response);
    }
}
