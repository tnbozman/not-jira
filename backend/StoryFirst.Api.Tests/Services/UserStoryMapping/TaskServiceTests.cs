using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.UserStoryMapping.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using Xunit;

namespace StoryFirst.Api.Tests.Services.UserStoryMapping;

public class TaskServiceTests
{
    private readonly Mock<IRepository<TaskItem>> _mockTaskRepo;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _mockTaskRepo = new Mock<IRepository<TaskItem>>();
        _service = new TaskService(_mockTaskRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Task 1" },
            new() { Id = 2, Title = "Task 2" }
        };
        _mockTaskRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTask_ReturnsTask()
    {
        // Arrange
        var task = new TaskItem { Id = 1, Title = "Task 1" };
        _mockTaskRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTask_ReturnsNull()
    {
        // Arrange
        _mockTaskRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidTask_ReturnsCreatedTask()
    {
        // Arrange
        var task = new TaskItem { Title = "New Task" };
        _mockTaskRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(task);

        // Assert
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ValidTask_UpdatesSuccessfully()
    {
        // Arrange
        var existingTask = new TaskItem { Id = 1, Title = "Old Title", CreatedAt = DateTime.UtcNow.AddDays(-1) };
        var updatedTask = new TaskItem { Id = 1, Title = "New Title" };
        _mockTaskRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTask);
        _mockTaskRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(1, updatedTask);

        // Assert
        updatedTask.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        updatedTask.CreatedAt.Should().Be(existingTask.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var task = new TaskItem { Id = 2, Title = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, task));
    }

    [Fact]
    public async Task UpdateAsync_TaskNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var task = new TaskItem { Id = 999, Title = "Test" };
        _mockTaskRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, task));
    }

    [Fact]
    public async Task DeleteAsync_ExistingTask_DeletesSuccessfully()
    {
        // Arrange
        var task = new TaskItem { Id = 1, Title = "Test" };
        _mockTaskRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(task);
        _mockTaskRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockTaskRepo.Verify(x => x.Remove(task), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTask_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockTaskRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
    }
}
