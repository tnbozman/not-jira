using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.ProjectManagement.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.ProjectManagement;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepo;
    private readonly Mock<IRepository<ProjectMember>> _mockMemberRepo;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _mockProjectRepo = new Mock<IProjectRepository>();
        _mockMemberRepo = new Mock<IRepository<ProjectMember>>();
        _service = new ProjectService(_mockProjectRepo.Object, _mockMemberRepo.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = 1, Key = "TEST-1", Name = "Test Project 1" },
            new() { Id = 2, Key = "TEST-2", Name = "Test Project 2" }
        };
        _mockProjectRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(projects);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockProjectRepo.Verify(x => x.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProject()
    {
        // Arrange
        var project = new Project { Id = 1, Key = "TEST-1", Name = "Test Project" };
        _mockProjectRepo.Setup(x => x.GetWithMembersAsync(1)).ReturnsAsync(project);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        _mockProjectRepo.Verify(x => x.GetWithMembersAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockProjectRepo.Setup(x => x.GetWithMembersAsync(999)).ReturnsAsync((Project?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByKeyAsync Tests

    [Fact]
    public async Task GetByKeyAsync_ExistingKey_ReturnsProject()
    {
        // Arrange
        var project = new Project { Id = 1, Key = "TEST-1", Name = "Test Project" };
        _mockProjectRepo.Setup(x => x.GetByKeyAsync("TEST-1")).ReturnsAsync(project);

        // Act
        var result = await _service.GetByKeyAsync("TEST-1");

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("TEST-1");
    }

    [Fact]
    public async Task GetByKeyAsync_NonExistingKey_ReturnsNull()
    {
        // Arrange
        _mockProjectRepo.Setup(x => x.GetByKeyAsync("NONEXISTENT")).ReturnsAsync((Project?)null);

        // Act
        var result = await _service.GetByKeyAsync("NONEXISTENT");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidProject_ReturnsProject()
    {
        // Arrange
        var project = new Project { Key = "TEST-1", Name = "Test Project" };
        _mockProjectRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Project, bool>>>()))
            .ReturnsAsync(false);
        _mockProjectRepo.Setup(x => x.AddAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);
        _mockProjectRepo.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(project);

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockProjectRepo.Verify(x => x.AddAsync(project), Times.Once);
        _mockProjectRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_EmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var project = new Project { Key = "", Name = "Test Project" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(project));
    }

    [Fact]
    public async Task CreateAsync_NullKey_ThrowsArgumentException()
    {
        // Arrange
        var project = new Project { Key = null!, Name = "Test Project" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(project));
    }

    [Fact]
    public async Task CreateAsync_WhitespaceKey_ThrowsArgumentException()
    {
        // Arrange
        var project = new Project { Key = "   ", Name = "Test Project" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(project));
    }

    [Fact]
    public async Task CreateAsync_InvalidKeyFormat_ThrowsArgumentException()
    {
        // Arrange
        var project = new Project { Key = "test-1", Name = "Test Project" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(project));
        exception.Message.Should().Contain("uppercase");
    }

    [Fact]
    public async Task CreateAsync_DuplicateKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var project = new Project { Key = "TEST-1", Name = "Test Project" };
        _mockProjectRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Project, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(project));
        exception.Message.Should().Contain("already exists");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidProject_UpdatesSuccessfully()
    {
        // Arrange
        var existingProject = new Project { Id = 1, Key = "TEST-1", Name = "Old Name", CreatedAt = DateTime.UtcNow.AddDays(-1) };
        var updatedProject = new Project { Id = 1, Key = "TEST-1", Name = "New Name" };
        
        _mockProjectRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingProject);
        _mockProjectRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(1, updatedProject);

        // Assert
        updatedProject.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        updatedProject.CreatedAt.Should().Be(existingProject.CreatedAt);
        _mockProjectRepo.Verify(x => x.Update(updatedProject), Times.Once);
        _mockProjectRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var project = new Project { Id = 2, Key = "TEST-1", Name = "Test" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, project));
        exception.Message.Should().Contain("mismatch");
    }

    [Fact]
    public async Task UpdateAsync_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var project = new Project { Id = 999, Key = "TEST-1", Name = "Test" };
        _mockProjectRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, project));
    }

    [Fact]
    public async Task UpdateAsync_AttemptToChangeKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingProject = new Project { Id = 1, Key = "TEST-1", Name = "Test" };
        var updatedProject = new Project { Id = 1, Key = "TEST-2", Name = "Test" };
        _mockProjectRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingProject);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, updatedProject));
        exception.Message.Should().Contain("Cannot change");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingProject_DeletesSuccessfully()
    {
        // Arrange
        var project = new Project { Id = 1, Key = "TEST-1", Name = "Test" };
        _mockProjectRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(project);
        _mockProjectRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockProjectRepo.Verify(x => x.Remove(project), Times.Once);
        _mockProjectRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingProject_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockProjectRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
    }

    #endregion

    #region AddMemberAsync Tests

    [Fact]
    public async Task AddMemberAsync_ValidMember_AddsMemberSuccessfully()
    {
        // Arrange
        var project = new Project { Id = 1, Key = "TEST-1", Name = "Test" };
        var member = new ProjectMember { UserId = "user123", Role = ProjectRole.Developer };
        
        _mockProjectRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(project);
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync((ProjectMember?)null);
        _mockMemberRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AddMemberAsync(1, member);

        // Assert
        result.ProjectId.Should().Be(1);
        result.AddedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockMemberRepo.Verify(x => x.AddAsync(member), Times.Once);
        _mockMemberRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var member = new ProjectMember { UserId = "user123", Role = ProjectRole.Developer };
        _mockProjectRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddMemberAsync(999, member));
    }

    [Fact]
    public async Task AddMemberAsync_DuplicateMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var project = new Project { Id = 1, Key = "TEST-1", Name = "Test" };
        var member = new ProjectMember { UserId = "user123", Role = ProjectRole.Developer };
        var existingMember = new ProjectMember { Id = 1, ProjectId = 1, UserId = "user123", Role = ProjectRole.Developer };
        
        _mockProjectRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(project);
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync(existingMember);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddMemberAsync(1, member));
        exception.Message.Should().Contain("already a member");
    }

    #endregion

    #region GetMemberAsync Tests

    [Fact]
    public async Task GetMemberAsync_ExistingMember_ReturnsMember()
    {
        // Arrange
        var member = new ProjectMember { Id = 1, ProjectId = 1, UserId = "user123", Role = ProjectRole.Developer };
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync(member);

        // Act
        var result = await _service.GetMemberAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetMemberAsync_NonExistingMember_ReturnsNull()
    {
        // Arrange
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync((ProjectMember?)null);

        // Act
        var result = await _service.GetMemberAsync(1, 999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region RemoveMemberAsync Tests

    [Fact]
    public async Task RemoveMemberAsync_ExistingMember_RemovesSuccessfully()
    {
        // Arrange
        var member = new ProjectMember { Id = 1, ProjectId = 1, UserId = "user123", Role = ProjectRole.Developer };
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync(member);
        _mockMemberRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.RemoveMemberAsync(1, 1);

        // Assert
        _mockMemberRepo.Verify(x => x.Remove(member), Times.Once);
        _mockMemberRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveMemberAsync_NonExistingMember_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync((ProjectMember?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RemoveMemberAsync(1, 999));
    }

    #endregion

    #region UpdateMemberAsync Tests

    [Fact]
    public async Task UpdateMemberAsync_ValidMember_UpdatesSuccessfully()
    {
        // Arrange
        var existingMember = new ProjectMember { Id = 1, ProjectId = 1, UserId = "user123", Role = ProjectRole.Developer, AddedAt = DateTime.UtcNow.AddDays(-1) };
        var updatedMember = new ProjectMember { Id = 1, ProjectId = 1, UserId = "user123", Role = ProjectRole.ProductManager };
        
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync(existingMember);
        _mockMemberRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateMemberAsync(1, 1, updatedMember);

        // Assert
        updatedMember.AddedAt.Should().Be(existingMember.AddedAt);
        _mockMemberRepo.Verify(x => x.Update(updatedMember), Times.Once);
        _mockMemberRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateMemberAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var member = new ProjectMember { Id = 2, ProjectId = 1, UserId = "user123", Role = ProjectRole.Developer };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateMemberAsync(1, 1, member));
        exception.Message.Should().Contain("mismatch");
    }

    [Fact]
    public async Task UpdateMemberAsync_ProjectIdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var member = new ProjectMember { Id = 1, ProjectId = 2, UserId = "user123", Role = ProjectRole.Developer };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateMemberAsync(1, 1, member));
        exception.Message.Should().Contain("mismatch");
    }

    [Fact]
    public async Task UpdateMemberAsync_MemberNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var member = new ProjectMember { Id = 999, ProjectId = 1, UserId = "user123", Role = ProjectRole.Developer };
        _mockMemberRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>()))
            .ReturnsAsync((ProjectMember?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateMemberAsync(1, 999, member));
    }

    #endregion
}
