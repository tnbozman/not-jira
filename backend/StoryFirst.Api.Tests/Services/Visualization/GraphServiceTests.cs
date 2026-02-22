using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.Visualization.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.Visualization;

public class GraphServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepo;
    private readonly Mock<IExternalEntityRepository> _mockEntityRepo;
    private readonly Mock<IRepository<Problem>> _mockProblemRepo;
    private readonly Mock<IRepository<Outcome>> _mockOutcomeRepo;
    private readonly Mock<IRepository<Interview>> _mockInterviewRepo;
    private readonly GraphService _service;

    public GraphServiceTests()
    {
        _mockProjectRepo = new Mock<IProjectRepository>();
        _mockEntityRepo = new Mock<IExternalEntityRepository>();
        _mockProblemRepo = new Mock<IRepository<Problem>>();
        _mockOutcomeRepo = new Mock<IRepository<Outcome>>();
        _mockInterviewRepo = new Mock<IRepository<Interview>>();
        _service = new GraphService(
            _mockProjectRepo.Object,
            _mockEntityRepo.Object,
            _mockProblemRepo.Object,
            _mockOutcomeRepo.Object,
            _mockInterviewRepo.Object
        );
    }

    [Fact]
    public async Task GetGraphDataAsync_ReturnsGraph()
    {
        // Arrange
        var projectId = 1;
        var project = new Project { Id = projectId, Key = "TEST", Name = "Test" };
        var entities = new List<ExternalEntity> { new() { Id = 1, ProjectId = projectId, Name = "User" } };
        var problems = new List<Problem>();
        var outcomes = new List<Outcome>();
        var interviews = new List<Interview>();

        _mockProjectRepo.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
        _mockEntityRepo.Setup(x => x.GetByProjectIdAsync(projectId)).ReturnsAsync(entities);
        _mockProblemRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Problem, bool>>>())).ReturnsAsync(problems);
        _mockOutcomeRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Outcome, bool>>>())).ReturnsAsync(outcomes);
        _mockInterviewRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Interview, bool>>>())).ReturnsAsync(interviews);

        // Act
        var result = await _service.GetGraphDataAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetGraphDataAsync_ProjectNotFound_ThrowsException()
    {
        // Arrange
        _mockProjectRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetGraphDataAsync(999));
    }
}
