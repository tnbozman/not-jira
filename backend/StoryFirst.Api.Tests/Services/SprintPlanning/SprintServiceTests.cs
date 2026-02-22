using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.SprintPlanning.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.SprintPlanning;

public class SprintServiceTests
{
    private readonly Mock<IRepository<Sprint>> _mockSprintRepo;
    private readonly Mock<IRepository<TeamPlanning>> _mockTeamPlanningRepo;
    private readonly SprintService _service;

    public SprintServiceTests()
    {
        _mockSprintRepo = new Mock<IRepository<Sprint>>();
        _mockTeamPlanningRepo = new Mock<IRepository<TeamPlanning>>();
        _service = new SprintService(_mockSprintRepo.Object, _mockTeamPlanningRepo.Object);
    }

    [Fact]
    public async Task GetAllByProjectAsync_ReturnsSprintsForProject()
    {
        // Arrange
        var projectId = 1;
        var sprints = new List<Sprint>
        {
            new() { Id = 1, ProjectId = projectId, Name = "Sprint 1", StartDate = DateTime.UtcNow.AddDays(-30) },
            new() { Id = 2, ProjectId = projectId, Name = "Sprint 2", StartDate = DateTime.UtcNow.AddDays(-14) }
        };
        _mockSprintRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprints);

        // Act
        var result = await _service.GetAllByProjectAsync(projectId);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSprint_ReturnsSprint()
    {
        // Arrange
        var sprint = new Sprint { Id = 1, ProjectId = 1, Name = "Sprint 1" };
        _mockSprintRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprint);

        // Act
        var result = await _service.GetByIdAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingSprint_ReturnsNull()
    {
        // Arrange
        _mockSprintRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync((Sprint?)null);

        // Act
        var result = await _service.GetByIdAsync(1, 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidSprint_ReturnsCreatedSprint()
    {
        // Arrange
        var sprint = new Sprint { Name = "New Sprint", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(14) };
        _mockSprintRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(1, sprint);

        // Assert
        result.ProjectId.Should().Be(1);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ValidSprint_UpdatesSuccessfully()
    {
        // Arrange
        var existingSprint = new Sprint { Id = 1, ProjectId = 1, Name = "Old Name" };
        var updatedSprint = new Sprint { Id = 1, Name = "New Name", Goal = "New Goal", Status = "Active" };
        _mockSprintRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(existingSprint);
        _mockSprintRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(1, 1, updatedSprint);

        // Assert
        existingSprint.Name.Should().Be("New Name");
        existingSprint.Goal.Should().Be("New Goal");
        existingSprint.Status.Should().Be("Active");
        existingSprint.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var sprint = new Sprint { Id = 2, Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, 1, sprint));
    }

    [Fact]
    public async Task UpdateAsync_SprintNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var sprint = new Sprint { Id = 999, Name = "Test" };
        _mockSprintRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync((Sprint?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, 999, sprint));
    }

    [Fact]
    public async Task DeleteAsync_ExistingSprint_DeletesSuccessfully()
    {
        // Arrange
        var sprint = new Sprint { Id = 1, ProjectId = 1, Name = "Test" };
        _mockSprintRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync(sprint);
        _mockSprintRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(1, 1);

        // Assert
        _mockSprintRepo.Verify(x => x.Remove(sprint), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingSprint_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockSprintRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
            .ReturnsAsync((Sprint?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1, 999));
    }
}
