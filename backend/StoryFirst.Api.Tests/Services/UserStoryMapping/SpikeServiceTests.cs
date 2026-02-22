using FluentAssertions;
using Moq;
using StoryFirst.Api.Areas.UserStoryMapping.Services;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace StoryFirst.Api.Tests.Services.UserStoryMapping;

public class SpikeServiceTests
{
    private readonly Mock<ISpikeRepository> _mockSpikeRepo;
    private readonly SpikeService _service;

    public SpikeServiceTests()
    {
        _mockSpikeRepo = new Mock<ISpikeRepository>();
        _service = new SpikeService(_mockSpikeRepo.Object);
    }

    [Fact]
    public async Task GetByEpicIdAsync_ReturnsSpikesForEpic()
    {
        // Arrange
        var epicId = 1;
        var spikes = new List<Spike>
        {
            new() { Id = 1, EpicId = epicId, Title = "Spike 1", Order = 1 },
            new() { Id = 2, EpicId = epicId, Title = "Spike 2", Order = 2 }
        };

        _mockSpikeRepo.Setup(x => x.GetByEpicIdAsync(epicId)).ReturnsAsync(spikes);

        // Act
        var result = await _service.GetByEpicIdAsync(epicId);

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.EpicId == epicId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSpike_ReturnsSpike()
    {
        // Arrange
        var spike = new Spike { Id = 1, EpicId = 1, Title = "Spike 1" };
        _mockSpikeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(spike);
        _mockSpikeRepo.Setup(x => x.GetWithDetailsAsync(1)).ReturnsAsync(spike);

        // Act
        var result = await _service.GetByIdAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingSpike_ReturnsNull()
    {
        // Arrange
        _mockSpikeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync((Spike?)null);

        // Act
        var result = await _service.GetByIdAsync(1, 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidSpike_ReturnsCreatedSpike()
    {
        // Arrange
        var spike = new Spike { Title = "New Spike", Order = 1 };
        _mockSpikeRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(1, spike);

        // Assert
        result.EpicId.Should().Be(1);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ValidSpike_UpdatesSuccessfully()
    {
        // Arrange
        var existingSpike = new Spike { Id = 1, EpicId = 1, Title = "Old Title" };
        var updatedSpike = new Spike { Id = 1, Title = "New Title", Priority = Priority.High };
        _mockSpikeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(existingSpike);
        _mockSpikeRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(1, 1, updatedSpike);

        // Assert
        existingSpike.Title.Should().Be("New Title");
        existingSpike.Priority.Should().Be(Priority.High);
        existingSpike.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var spike = new Spike { Id = 2, Title = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, 1, spike));
    }

    [Fact]
    public async Task UpdateAsync_SpikeNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var spike = new Spike { Id = 999, Title = "Test" };
        _mockSpikeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync((Spike?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, 999, spike));
    }

    [Fact]
    public async Task DeleteAsync_ExistingSpike_DeletesSuccessfully()
    {
        // Arrange
        var spike = new Spike { Id = 1, EpicId = 1, Title = "Test" };
        _mockSpikeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync(spike);
        _mockSpikeRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(1, 1);

        // Assert
        _mockSpikeRepo.Verify(x => x.Remove(spike), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingSpike_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockSpikeRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Spike, bool>>>()))
            .ReturnsAsync((Spike?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1, 999));
    }
}
