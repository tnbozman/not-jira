using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.SprintPlanning.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.SprintPlanning;

public class BacklogServiceTests
{
    private readonly Mock<IRepository<Sprint>> _mockSprintRepo;
    private readonly Mock<IStoryRepository> _mockStoryRepo;
    private readonly Mock<ISpikeRepository> _mockSpikeRepo;
    private readonly BacklogService _service;

    public BacklogServiceTests()
    {
        _mockSprintRepo = new Mock<IRepository<Sprint>>();
        _mockStoryRepo = new Mock<IStoryRepository>();
        _mockSpikeRepo = new Mock<ISpikeRepository>();
        _service = new BacklogService(_mockSprintRepo.Object, _mockStoryRepo.Object, _mockSpikeRepo.Object);
    }

    [Fact]
    public async Task GetBacklogAsync_ReturnsBacklogWithStories()
    {
        // Arrange
        var projectId = 1;
        var sprints = new List<Sprint>
        {
            new() { Id = 1, ProjectId = projectId, Name = "Sprint 1", StartDate = DateTime.UtcNow }
        };
        var stories = new List<Story>
        {
            new() { Id = 1, Title = "Story 1", EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } },
            new() { Id = 2, Title = "Story 2", EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } }
        };
        var spikes = new List<Spike>();

        _mockSprintRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprints);
        _mockStoryRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(stories);
        _mockSpikeRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(spikes);

        // Act
        var result = await _service.GetBacklogAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Sprints.Should().HaveCount(1);
        result.BacklogItems.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBacklogAsync_FilterByTeam_ReturnsFilteredItems()
    {
        // Arrange
        var projectId = 1;
        var teamId = 5;
        var sprints = new List<Sprint>();
        var stories = new List<Story>
        {
            new() { Id = 1, Title = "Story 1", TeamId = teamId, EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } },
            new() { Id = 2, Title = "Story 2", TeamId = 10, EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } }
        };
        var spikes = new List<Spike>();

        _mockSprintRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprints);
        _mockStoryRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(stories);
        _mockSpikeRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(spikes);

        // Act
        var result = await _service.GetBacklogAsync(projectId, teamId: teamId);

        // Assert
        result.BacklogItems.Should().HaveCount(1);
        result.BacklogItems.First().TeamId.Should().Be(teamId);
    }

    [Fact]
    public async Task GetBacklogAsync_FilterByAssignee_ReturnsFilteredItems()
    {
        // Arrange
        var projectId = 1;
        var assigneeId = "user123";
        var sprints = new List<Sprint>();
        var stories = new List<Story>
        {
            new() { Id = 1, Title = "Story 1", AssigneeId = assigneeId, EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } },
            new() { Id = 2, Title = "Story 2", AssigneeId = "user456", EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } }
        };
        var spikes = new List<Spike>();

        _mockSprintRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprints);
        _mockStoryRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(stories);
        _mockSpikeRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(spikes);

        // Act
        var result = await _service.GetBacklogAsync(projectId, assigneeId: assigneeId);

        // Assert
        result.BacklogItems.Should().HaveCount(1);
        result.BacklogItems.First().AssigneeId.Should().Be(assigneeId);
    }

    [Fact]
    public async Task GetBacklogAsync_WithSpikes_ReturnsBothStoriesAndSpikes()
    {
        // Arrange
        var projectId = 1;
        var sprints = new List<Sprint>();
        var stories = new List<Story>
        {
            new() { Id = 1, Title = "Story 1", EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } }
        };
        var spikes = new List<Spike>
        {
            new() { Id = 1, Title = "Spike 1", EpicId = 1, Epic = new Epic { ThemeId = 1, Theme = new Theme { ProjectId = projectId } } }
        };

        _mockSprintRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprints);
        _mockStoryRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(stories);
        _mockSpikeRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(spikes);

        // Act
        var result = await _service.GetBacklogAsync(projectId);

        // Assert
        result.BacklogItems.Should().HaveCount(2);
        result.BacklogItems.Should().Contain(item => item.Type == "Story");
        result.BacklogItems.Should().Contain(item => item.Type == "Spike");
    }
}
