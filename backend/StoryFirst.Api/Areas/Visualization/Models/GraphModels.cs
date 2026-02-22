namespace StoryFirst.Api.Areas.Visualization.Models;

public class GraphData
{
    public List<GraphNode> Nodes { get; set; } = new();
    public List<GraphEdge> Edges { get; set; } = new();
    public GraphStats? Stats { get; set; }
}

public class GraphNode
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class GraphEdge
{
    public string Id { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class GraphStats
{
    public int EntityCount { get; set; }
    public int ProblemCount { get; set; }
    public int OutcomeCount { get; set; }
    public int InterviewCount { get; set; }
}
