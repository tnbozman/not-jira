using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.SprintPlanning.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.SprintPlanning;

public class ReleaseServiceTests
{
    private readonly Mock<IRepository<Release>> _mockReleaseRepo;
    private readonly ReleaseService _service;

    public ReleaseServiceTests()
    {
        _mockReleaseRepo = new Mock<IRepository<Release>>();
        _service = new ReleaseService(_mockReleaseRepo.Object);
    }

    [Fact]
    public async Task GetAllByProjectAsync_ReturnsReleases()
    {
        var releases = new List<Release> { new() { Id = 1, ProjectId = 1, Name = "v1.0" } };
        _mockReleaseRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Release, bool>>>())).ReturnsAsync(releases);
        var result = await _service.GetAllByProjectAsync(1);
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRelease()
    {
        var release = new Release { Id = 1, ProjectId = 1, Name = "v1.0" };
        _mockReleaseRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Release, bool>>>())).ReturnsAsync(release);
        var result = await _service.GetByIdAsync(1, 1);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_CreatesRelease()
    {
        var release = new Release { Name = "v2.0" };
        _mockReleaseRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var result = await _service.CreateAsync(1, release);
        result.ProjectId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesRelease()
    {
        var existing = new Release { Id = 1, ProjectId = 1, Name = "v1.0" };
        var updated = new Release { Id = 1, Name = "v1.1" };
        _mockReleaseRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Release, bool>>>())).ReturnsAsync(existing);
        _mockReleaseRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        await _service.UpdateAsync(1, 1, updated);
        existing.Name.Should().Be("v1.1");
    }

    [Fact]
    public async Task DeleteAsync_DeletesRelease()
    {
        var release = new Release { Id = 1, ProjectId = 1, Name = "v1.0" };
        _mockReleaseRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Release, bool>>>())).ReturnsAsync(release);
        _mockReleaseRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        await _service.DeleteAsync(1, 1);
        _mockReleaseRepo.Verify(x => x.Remove(release), Times.Once);
    }
}
