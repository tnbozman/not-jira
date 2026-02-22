using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.UserStoryMapping.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.UserStoryMapping;

public class StoryServiceTests
{
    private readonly Mock<IStoryRepository> _mockStoryRepo;
    private readonly StoryService _service;

    public StoryServiceTests()
    {
        _mockStoryRepo = new Mock<IStoryRepository>();
        _service = new StoryService(_mockStoryRepo.Object);
    }

    #region GetByEpicIdAsync Tests

    [Fact]
    public async Task GetByEpicIdAsync_ReturnsStoriesForEpic()
    {
        // Arrange
        var epicId = 1;
        var stories = new List<Story>
        {
            new() { Id = 1, EpicId = epicId, Title = "Story 1", Order = 1 },
            new() { Id = 2, EpicId = epicId, Title = "Story 2", Order = 2 }
        };

        _mockStoryRepo.Setup(x => x.GetByEpicIdAsync(epicId))
            .ReturnsAsync(stories);

        // Act
        var result = await _service.GetByEpicIdAsync(epicId);

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.EpicId == epicId).Should().BeTrue();
        _mockStoryRepo.Verify(x => x.GetByEpicIdAsync(epicId), Times.Once);
    }

    [Fact]
    public async Task GetByEpicIdAsync_NoStories_ReturnsEmptyList()
    {
        // Arrange
        _mockStoryRepo.Setup(x => x.GetByEpicIdAsync(999))
            .ReturnsAsync(new List<Story>());

        // Act
        var result = await _service.GetByEpicIdAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingStory_ReturnsStoryWithDetails()
    {
        // Arrange
        var epicId = 1;
        var storyId = 1;
        var story = new Story { Id = storyId, EpicId = epicId, Title = "Story 1" };
        var detailedStory = new Story 
        { 
            Id = storyId, 
            EpicId = epicId, 
            Title = "Story 1"
        };

        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(story);
        _mockStoryRepo.Setup(x => x.GetWithDetailsAsync(storyId))
            .ReturnsAsync(detailedStory);

        // Act
        var result = await _service.GetByIdAsync(epicId, storyId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(storyId);
        result.EpicId.Should().Be(epicId);
        _mockStoryRepo.Verify(x => x.GetWithDetailsAsync(storyId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingStory_ReturnsNull()
    {
        // Arrange
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync((Story?)null);

        // Act
        var result = await _service.GetByIdAsync(1, 999);

        // Assert
        result.Should().BeNull();
        _mockStoryRepo.Verify(x => x.GetWithDetailsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WrongEpic_ReturnsNull()
    {
        // Arrange
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync((Story?)null);

        // Act
        var result = await _service.GetByIdAsync(999, 1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidStory_ReturnsCreatedStory()
    {
        // Arrange
        var epicId = 1;
        var story = new Story 
        { 
            Title = "New Story", 
            Description = "Description", 
            Order = 1,
            Priority = Priority.High,
            Status = "To Do"
        };
        
        _mockStoryRepo.Setup(x => x.AddAsync(It.IsAny<Story>()))
            .Returns(Task.CompletedTask);
        _mockStoryRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(epicId, story);

        // Assert
        result.Should().NotBeNull();
        result.EpicId.Should().Be(epicId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockStoryRepo.Verify(x => x.AddAsync(story), Times.Once);
        _mockStoryRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsEpicIdCorrectly()
    {
        // Arrange
        var epicId = 5;
        var story = new Story { Title = "Test Story", Order = 1 };
        
        _mockStoryRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(epicId, story);

        // Assert
        result.EpicId.Should().Be(5);
    }

    [Fact]
    public async Task CreateAsync_WithAllFields_PreservesAllFields()
    {
        // Arrange
        var epicId = 1;
        var story = new Story 
        { 
            Title = "Story",
            Description = "Description",
            SolutionDescription = "Solution",
            AcceptanceCriteria = "Criteria",
            Order = 1,
            Priority = Priority.High,
            Status = "In Progress",
            StoryPoints = 5,
            SprintId = 10,
            ReleaseId = 20,
            TeamId = 30,
            AssigneeId = "user123",
            AssigneeName = "John Doe",
            OutcomeId = 40
        };
        
        _mockStoryRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(epicId, story);

        // Assert
        result.Title.Should().Be("Story");
        result.StoryPoints.Should().Be(5);
        result.SprintId.Should().Be(10);
        result.AssigneeId.Should().Be("user123");
        result.OutcomeId.Should().Be(40);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidStory_UpdatesSuccessfully()
    {
        // Arrange
        var epicId = 1;
        var storyId = 1;
        var existingStory = new Story 
        { 
            Id = storyId, 
            EpicId = epicId, 
            Title = "Old Title",
            Description = "Old Description",
            Order = 1,
            Priority = Priority.Low,
            Status = "To Do",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var updatedStory = new Story 
        { 
            Id = storyId, 
            Title = "New Title",
            Description = "New Description",
            SolutionDescription = "Solution",
            AcceptanceCriteria = "Criteria",
            Order = 2,
            Priority = Priority.High,
            Status = "Done",
            StoryPoints = 8,
            SprintId = 5,
            ReleaseId = 10,
            TeamId = 15,
            AssigneeId = "user456",
            AssigneeName = "Jane Doe",
            OutcomeId = 20
        };

        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(existingStory);
        _mockStoryRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(epicId, storyId, updatedStory);

        // Assert
        existingStory.Title.Should().Be("New Title");
        existingStory.Description.Should().Be("New Description");
        existingStory.SolutionDescription.Should().Be("Solution");
        existingStory.AcceptanceCriteria.Should().Be("Criteria");
        existingStory.Order.Should().Be(2);
        existingStory.Priority.Should().Be(Priority.High);
        existingStory.Status.Should().Be("Done");
        existingStory.StoryPoints.Should().Be(8);
        existingStory.SprintId.Should().Be(5);
        existingStory.ReleaseId.Should().Be(10);
        existingStory.TeamId.Should().Be(15);
        existingStory.AssigneeId.Should().Be("user456");
        existingStory.AssigneeName.Should().Be("Jane Doe");
        existingStory.OutcomeId.Should().Be(20);
        existingStory.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockStoryRepo.Verify(x => x.Update(existingStory), Times.Once);
        _mockStoryRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var story = new Story { Id = 2, Title = "Test" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, 1, story));
        exception.Message.Should().Contain("mismatch");
    }

    [Fact]
    public async Task UpdateAsync_StoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var story = new Story { Id = 999, Title = "Test" };
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync((Story?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, 999, story));
    }

    [Fact]
    public async Task UpdateAsync_WrongEpic_ThrowsKeyNotFoundException()
    {
        // Arrange
        var story = new Story { Id = 1, Title = "Test" };
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync((Story?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, 1, story));
    }

    [Fact]
    public async Task UpdateAsync_ClearOptionalFields_SetsToNull()
    {
        // Arrange
        var existingStory = new Story 
        { 
            Id = 1, 
            EpicId = 1, 
            Title = "Story",
            SprintId = 5,
            ReleaseId = 10,
            TeamId = 15,
            AssigneeId = "user",
            OutcomeId = 20
        };
        var updatedStory = new Story 
        { 
            Id = 1, 
            Title = "Story",
            SprintId = null,
            ReleaseId = null,
            TeamId = null,
            AssigneeId = null,
            OutcomeId = null
        };
        
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(existingStory);
        _mockStoryRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(1, 1, updatedStory);

        // Assert
        existingStory.SprintId.Should().BeNull();
        existingStory.ReleaseId.Should().BeNull();
        existingStory.TeamId.Should().BeNull();
        existingStory.AssigneeId.Should().BeNull();
        existingStory.OutcomeId.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingStory_DeletesSuccessfully()
    {
        // Arrange
        var epicId = 1;
        var storyId = 1;
        var story = new Story { Id = storyId, EpicId = epicId, Title = "Test" };
        
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync(story);
        _mockStoryRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(epicId, storyId);

        // Assert
        _mockStoryRepo.Verify(x => x.Remove(story), Times.Once);
        _mockStoryRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingStory_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync((Story?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1, 999));
    }

    [Fact]
    public async Task DeleteAsync_WrongEpic_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockStoryRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Story, bool>>>()))
            .ReturnsAsync((Story?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999, 1));
    }

    #endregion
}
