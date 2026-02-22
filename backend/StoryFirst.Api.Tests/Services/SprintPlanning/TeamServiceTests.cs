using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.SprintPlanning.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.SprintPlanning;

public class TeamServiceTests
{
    private readonly Mock<IRepository<Team>> _mockTeamRepo;
    private readonly TeamService _service;

    public TeamServiceTests()
    {
        _mockTeamRepo = new Mock<IRepository<Team>>();
        _service = new TeamService(_mockTeamRepo.Object);
    }

    [Fact]
    public async Task GetAllByProjectAsync_ReturnsTeams()
    {
        var teams = new List<Team> { new() { Id = 1, ProjectId = 1, Name = "Team 1" } };
        _mockTeamRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Team, bool>>>())).ReturnsAsync(teams);
        var result = await _service.GetAllByProjectAsync(1);
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTeam()
    {
        var team = new Team { Id = 1, ProjectId = 1, Name = "Team 1" };
        _mockTeamRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Team, bool>>>())).ReturnsAsync(team);
        var result = await _service.GetByIdAsync(1, 1);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_CreatesTeam()
    {
        var team = new Team { Name = "New Team" };
        _mockTeamRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var result = await _service.CreateAsync(1, team);
        result.ProjectId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTeam()
    {
        var existing = new Team { Id = 1, ProjectId = 1, Name = "Old" };
        var updated = new Team { Id = 1, Name = "New" };
        _mockTeamRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Team, bool>>>())).ReturnsAsync(existing);
        _mockTeamRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        await _service.UpdateAsync(1, 1, updated);
        existing.Name.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_DeletesTeam()
    {
        var team = new Team { Id = 1, ProjectId = 1, Name = "Test" };
        _mockTeamRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Team, bool>>>())).ReturnsAsync(team);
        _mockTeamRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        await _service.DeleteAsync(1, 1);
        _mockTeamRepo.Verify(x => x.Remove(team), Times.Once);
    }
}
