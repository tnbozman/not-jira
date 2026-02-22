using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.UserStoryMapping.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.UserStoryMapping;

public class EpicServiceTests
{
    private readonly Mock<IEpicRepository> _mockEpicRepo;
    private readonly EpicService _service;

    public EpicServiceTests()
    {
        _mockEpicRepo = new Mock<IEpicRepository>();
        _service = new EpicService(_mockEpicRepo.Object);
    }

    #region GetByThemeIdAsync Tests

    [Fact]
    public async Task GetByThemeIdAsync_ReturnsEpicsForTheme()
    {
        // Arrange
        var themeId = 1;
        var epics = new List<Epic>
        {
            new() { Id = 1, ThemeId = themeId, Name = "Epic 1", Order = 1 },
            new() { Id = 2, ThemeId = themeId, Name = "Epic 2", Order = 2 }
        };

        _mockEpicRepo.Setup(x => x.GetByThemeIdAsync(themeId))
            .ReturnsAsync(epics);

        // Act
        var result = await _service.GetByThemeIdAsync(themeId);

        // Assert
        result.Should().HaveCount(2);
        result.All(e => e.ThemeId == themeId).Should().BeTrue();
        _mockEpicRepo.Verify(x => x.GetByThemeIdAsync(themeId), Times.Once);
    }

    [Fact]
    public async Task GetByThemeIdAsync_NoEpics_ReturnsEmptyList()
    {
        // Arrange
        _mockEpicRepo.Setup(x => x.GetByThemeIdAsync(999))
            .ReturnsAsync(new List<Epic>());

        // Act
        var result = await _service.GetByThemeIdAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingEpic_ReturnsEpicWithDetails()
    {
        // Arrange
        var themeId = 1;
        var epicId = 1;
        var epic = new Epic { Id = epicId, ThemeId = themeId, Name = "Epic 1" };
        var detailedEpic = new Epic 
        { 
            Id = epicId, 
            ThemeId = themeId, 
            Name = "Epic 1",
            Stories = new List<Story>(),
            Spikes = new List<Spike>()
        };

        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync(epic);
        _mockEpicRepo.Setup(x => x.GetWithDetailsAsync(epicId))
            .ReturnsAsync(detailedEpic);

        // Act
        var result = await _service.GetByIdAsync(themeId, epicId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(epicId);
        result.ThemeId.Should().Be(themeId);
        _mockEpicRepo.Verify(x => x.GetWithDetailsAsync(epicId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEpic_ReturnsNull()
    {
        // Arrange
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync((Epic?)null);

        // Act
        var result = await _service.GetByIdAsync(1, 999);

        // Assert
        result.Should().BeNull();
        _mockEpicRepo.Verify(x => x.GetWithDetailsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WrongTheme_ReturnsNull()
    {
        // Arrange
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync((Epic?)null);

        // Act
        var result = await _service.GetByIdAsync(999, 1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidEpic_ReturnsCreatedEpic()
    {
        // Arrange
        var themeId = 1;
        var epic = new Epic { Name = "New Epic", Description = "Description", Order = 1 };
        
        _mockEpicRepo.Setup(x => x.AddAsync(It.IsAny<Epic>()))
            .Returns(Task.CompletedTask);
        _mockEpicRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(themeId, epic);

        // Assert
        result.Should().NotBeNull();
        result.ThemeId.Should().Be(themeId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockEpicRepo.Verify(x => x.AddAsync(epic), Times.Once);
        _mockEpicRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsThemeIdCorrectly()
    {
        // Arrange
        var themeId = 5;
        var epic = new Epic { Name = "Test Epic", Order = 1 };
        
        _mockEpicRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(themeId, epic);

        // Assert
        result.ThemeId.Should().Be(5);
    }

    [Fact]
    public async Task CreateAsync_WithOutcomeId_PreservesOutcomeId()
    {
        // Arrange
        var themeId = 1;
        var epic = new Epic { Name = "Epic", Order = 1, OutcomeId = 10 };
        
        _mockEpicRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(themeId, epic);

        // Assert
        result.OutcomeId.Should().Be(10);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidEpic_UpdatesSuccessfully()
    {
        // Arrange
        var themeId = 1;
        var epicId = 1;
        var existingEpic = new Epic 
        { 
            Id = epicId, 
            ThemeId = themeId, 
            Name = "Old Name",
            Description = "Old Description",
            Order = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var updatedEpic = new Epic 
        { 
            Id = epicId, 
            Name = "New Name",
            Description = "New Description",
            Order = 2,
            OutcomeId = 1
        };

        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync(existingEpic);
        _mockEpicRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(themeId, epicId, updatedEpic);

        // Assert
        existingEpic.Name.Should().Be("New Name");
        existingEpic.Description.Should().Be("New Description");
        existingEpic.Order.Should().Be(2);
        existingEpic.OutcomeId.Should().Be(1);
        existingEpic.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockEpicRepo.Verify(x => x.Update(existingEpic), Times.Once);
        _mockEpicRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var epic = new Epic { Id = 2, Name = "Test" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, 1, epic));
        exception.Message.Should().Contain("mismatch");
    }

    [Fact]
    public async Task UpdateAsync_EpicNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var epic = new Epic { Id = 999, Name = "Test" };
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync((Epic?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, 999, epic));
    }

    [Fact]
    public async Task UpdateAsync_WrongTheme_ThrowsKeyNotFoundException()
    {
        // Arrange
        var epic = new Epic { Id = 1, Name = "Test" };
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync((Epic?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, 1, epic));
    }

    [Fact]
    public async Task UpdateAsync_ClearOutcomeId_SetsToNull()
    {
        // Arrange
        var existingEpic = new Epic { Id = 1, ThemeId = 1, Name = "Epic", OutcomeId = 5 };
        var updatedEpic = new Epic { Id = 1, Name = "Epic", OutcomeId = null };
        
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync(existingEpic);
        _mockEpicRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(1, 1, updatedEpic);

        // Assert
        existingEpic.OutcomeId.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingEpic_DeletesSuccessfully()
    {
        // Arrange
        var themeId = 1;
        var epicId = 1;
        var epic = new Epic { Id = epicId, ThemeId = themeId, Name = "Test" };
        
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync(epic);
        _mockEpicRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(themeId, epicId);

        // Assert
        _mockEpicRepo.Verify(x => x.Remove(epic), Times.Once);
        _mockEpicRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingEpic_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync((Epic?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1, 999));
    }

    [Fact]
    public async Task DeleteAsync_WrongTheme_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockEpicRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync((Epic?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999, 1));
    }

    #endregion
}
