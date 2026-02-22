using StoryFirst.Api.Areas.Visualization.Models;

namespace StoryFirst.Api.Areas.Visualization.Services;

public interface IGraphService
{
    Task<GraphData> GetGraphDataAsync(int projectId);
}
