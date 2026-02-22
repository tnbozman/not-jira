using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public class SpikeService : ISpikeService
{
    private readonly ISpikeRepository _spikeRepository;

    public SpikeService(ISpikeRepository spikeRepository)
    {
        _spikeRepository = spikeRepository;
    }

    public async Task<IEnumerable<Spike>> GetByEpicIdAsync(int epicId)
    {
        return await _spikeRepository.GetByEpicIdAsync(epicId);
    }

    public async Task<Spike?> GetByIdAsync(int epicId, int id)
    {
        var spike = await _spikeRepository.FirstOrDefaultAsync(s => s.EpicId == epicId && s.Id == id);

        if (spike == null)
        {
            return null;
        }

        return await _spikeRepository.GetWithDetailsAsync(id);
    }

    public async Task<Spike> CreateAsync(int epicId, Spike spike)
    {
        spike.EpicId = epicId;
        spike.CreatedAt = DateTime.UtcNow;
        spike.UpdatedAt = DateTime.UtcNow;

        await _spikeRepository.AddAsync(spike);
        await _spikeRepository.SaveChangesAsync();

        return spike;
    }

    public async Task UpdateAsync(int epicId, int id, Spike spike)
    {
        if (id != spike.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingSpike = await _spikeRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (existingSpike == null)
        {
            throw new KeyNotFoundException("Spike not found");
        }

        existingSpike.Title = spike.Title;
        existingSpike.Description = spike.Description;
        existingSpike.InvestigationGoal = spike.InvestigationGoal;
        existingSpike.Findings = spike.Findings;
        existingSpike.Order = spike.Order;
        existingSpike.Priority = spike.Priority;
        existingSpike.Status = spike.Status;
        existingSpike.StoryPoints = spike.StoryPoints;
        existingSpike.SprintId = spike.SprintId;
        existingSpike.ReleaseId = spike.ReleaseId;
        existingSpike.OutcomeId = spike.OutcomeId;
        existingSpike.UpdatedAt = DateTime.UtcNow;

        _spikeRepository.Update(existingSpike);
        await _spikeRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int epicId, int id)
    {
        var spike = await _spikeRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (spike == null)
        {
            throw new KeyNotFoundException("Spike not found");
        }

        _spikeRepository.Remove(spike);
        await _spikeRepository.SaveChangesAsync();
    }
}
