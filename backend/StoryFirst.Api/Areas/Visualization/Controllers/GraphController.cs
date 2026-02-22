using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Areas.Visualization.Models;
using StoryFirst.Api.Areas.Visualization.Services;

namespace StoryFirst.Api.Areas.Visualization.Controllers;

[Area("Visualization")]
[Route("api/projects/{projectId}/graph")]
public class GraphController : BaseApiController
{
    private readonly IGraphService _graphService;

    public GraphController(IGraphService graphService)
    {
        _graphService = graphService;
    }

    [HttpGet]
    public async Task<ActionResult<GraphData>> GetGraphData(int projectId)
    {
        try
        {
            var graphData = await _graphService.GetGraphDataAsync(projectId);
            return Ok(graphData);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
