using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/epics/{epicId}/[controller]")]
public class SpikesController : BaseApiController
{
    private readonly ISpikeRepository _spikeRepository;

    public SpikesController(ISpikeRepository spikeRepository)
    {
        _spikeRepository = spikeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Spike>>> GetSpikes(int projectId, int epicId)
    {
        var spikes = await _spikeRepository.GetByEpicIdAsync(epicId);
        return Ok(spikes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Spike>> GetSpike(int projectId, int epicId, int id)
    {
        var spike = await _spikeRepository.FirstOrDefaultAsync(s => s.EpicId == epicId && s.Id == id);

        if (spike == null)
        {
            return NotFound();
        }

        var spikeDetails = await _spikeRepository.GetWithDetailsAsync(id);
        return Ok(spikeDetails);
    }

    [HttpPost]
    public async Task<ActionResult<Spike>> CreateSpike(int projectId, int epicId, Spike spike)
    {
        spike.EpicId = epicId;
        spike.CreatedAt = DateTime.UtcNow;
        spike.UpdatedAt = DateTime.UtcNow;

        await _spikeRepository.AddAsync(spike);
        await _spikeRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSpike), new { projectId, epicId, id = spike.Id }, spike);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpike(int projectId, int epicId, int id, Spike spike)
    {
        if (id != spike.Id)
        {
            return BadRequest();
        }

        var existingSpike = await _spikeRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (existingSpike == null)
        {
            return NotFound();
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

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpike(int projectId, int epicId, int id)
    {
        var spike = await _spikeRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (spike == null)
        {
            return NotFound();
        }

        _spikeRepository.Remove(spike);
        await _spikeRepository.SaveChangesAsync();

        return NoContent();
    }
}
