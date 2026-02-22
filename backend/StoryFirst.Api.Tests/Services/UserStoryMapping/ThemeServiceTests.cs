using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.UserStoryMapping.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.UserStoryMapping;

public class ThemeServiceTests
{
    private readonly Mock<IThemeRepository> _mockThemeRepo;
    private readonly Mock<IEpicRepository> _mockEpicRepo;
    private readonly ThemeService _service;

    public ThemeServiceTests()
    {
        _mockThemeRepo = new Mock<IThemeRepository>();
        _mockEpicRepo = new Mock<IEpicRepository>();
        _service = new ThemeService(_mockThemeRepo.Object, _mockEpicRepo.Object);
    }

    #region GetAllByProjectAsync Tests

    [Fact]
    public async Task GetAllByProjectAsync_ReturnsOrderedThemesWithEpics()
    {
        // Arrange
        var projectId = 1;
        var themes = new List<Theme>
        {
            new() { Id = 1, ProjectId = projectId, Name = "Theme 1", Order = 2 },
            new() { Id = 2, ProjectId = projectId, Name = "Theme 2", Order = 1 }
        };

        _mockThemeRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync(themes);
        
        foreach (var theme in themes)
        {
            _mockThemeRepo.Setup(x => x.GetWithDetailsAsync(theme.Id))
                .ReturnsAsync(new Theme 
                { 
                    Id = theme.Id, 
                    ProjectId = projectId, 
                    Name = theme.Name, 
                    Order = theme.Order,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            
            _mockEpicRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
                .ReturnsAsync(new List<Epic>());
        }

        // Act
        var result = await _service.GetAllByProjectAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockThemeRepo.Verify(x => x.FindAsync(It.IsAny<Expression<Func<Theme, bool>>>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingTheme_ReturnsThemeWithDetails()
    {
        // Arrange
        var projectId = 1;
        var themeId = 1;
        var theme = new Theme { Id = themeId, ProjectId = projectId, Name = "Theme 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync(theme);
        _mockThemeRepo.Setup(x => x.GetWithDetailsAsync(themeId))
            .ReturnsAsync(theme);
        _mockEpicRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Epic, bool>>>()))
            .ReturnsAsync(new List<Epic>());

        // Act
        var result = await _service.GetByIdAsync(projectId, themeId);

        // Assert
        result.Should().NotBeNull();
        _mockThemeRepo.Verify(x => x.GetWithDetailsAsync(themeId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTheme_ReturnsNull()
    {
        // Arrange
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync((Theme?)null);

        // Act
        var result = await _service.GetByIdAsync(1, 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WrongProject_ReturnsNull()
    {
        // Arrange
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync((Theme?)null);

        // Act
        var result = await _service.GetByIdAsync(999, 1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidTheme_ReturnsCreatedTheme()
    {
        // Arrange
        var projectId = 1;
        var theme = new Theme { Name = "New Theme", Description = "Description", Order = 1 };
        
        _mockThemeRepo.Setup(x => x.AddAsync(It.IsAny<Theme>()))
            .Returns(Task.CompletedTask);
        _mockThemeRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(projectId, theme);

        // Assert
        result.Should().NotBeNull();
        result.ProjectId.Should().Be(projectId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockThemeRepo.Verify(x => x.AddAsync(theme), Times.Once);
        _mockThemeRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsProjectIdCorrectly()
    {
        // Arrange
        var projectId = 5;
        var theme = new Theme { Name = "Test Theme", Order = 1 };
        
        _mockThemeRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(projectId, theme);

        // Assert
        result.ProjectId.Should().Be(5);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidTheme_UpdatesSuccessfully()
    {
        // Arrange
        var projectId = 1;
        var themeId = 1;
        var existingTheme = new Theme 
        { 
            Id = themeId, 
            ProjectId = projectId, 
            Name = "Old Name",
            Description = "Old Description",
            Order = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var updatedTheme = new Theme 
        { 
            Id = themeId, 
            Name = "New Name",
            Description = "New Description",
            Order = 2,
            OutcomeId = 1
        };

        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync(existingTheme);
        _mockThemeRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(projectId, themeId, updatedTheme);

        // Assert
        existingTheme.Name.Should().Be("New Name");
        existingTheme.Description.Should().Be("New Description");
        existingTheme.Order.Should().Be(2);
        existingTheme.OutcomeId.Should().Be(1);
        existingTheme.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockThemeRepo.Verify(x => x.Update(existingTheme), Times.Once);
        _mockThemeRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var theme = new Theme { Id = 2, Name = "Test" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, 1, theme));
        exception.Message.Should().Contain("mismatch");
    }

    [Fact]
    public async Task UpdateAsync_ThemeNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var theme = new Theme { Id = 999, Name = "Test" };
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync((Theme?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, 999, theme));
    }

    [Fact]
    public async Task UpdateAsync_WrongProject_ThrowsKeyNotFoundException()
    {
        // Arrange
        var theme = new Theme { Id = 1, Name = "Test" };
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync((Theme?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, 1, theme));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingTheme_DeletesSuccessfully()
    {
        // Arrange
        var projectId = 1;
        var themeId = 1;
        var theme = new Theme { Id = themeId, ProjectId = projectId, Name = "Test" };
        
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync(theme);
        _mockThemeRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(projectId, themeId);

        // Assert
        _mockThemeRepo.Verify(x => x.Remove(theme), Times.Once);
        _mockThemeRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTheme_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync((Theme?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1, 999));
    }

    [Fact]
    public async Task DeleteAsync_WrongProject_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockThemeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Theme, bool>>>()))
            .ReturnsAsync((Theme?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999, 1));
    }

    #endregion
}
