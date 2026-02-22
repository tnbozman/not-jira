using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.UserStoryMapping.Services;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/epics/{epicId}/[controller]")]
public class SpikesController : BaseApiController
{
    private readonly ISpikeService _spikeService;

    public SpikesController(ISpikeService spikeService)
    {
        _spikeService = spikeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Spike>>> GetSpikes(int projectId, int epicId)
    {
        var spikes = await _spikeService.GetByEpicIdAsync(epicId);
        return Ok(spikes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Spike>> GetSpike(int projectId, int epicId, int id)
    {
        var spike = await _spikeService.GetByIdAsync(epicId, id);

        if (spike == null)
        {
            return NotFound();
        }

        return Ok(spike);
    }

    [HttpPost]
    public async Task<ActionResult<Spike>> CreateSpike(int projectId, int epicId, Spike spike)
    {
        try
        {
            var result = await _spikeService.CreateAsync(epicId, spike);
            return CreatedAtAction(nameof(GetSpike), new { projectId, epicId, id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpike(int projectId, int epicId, int id, Spike spike)
    {
        try
        {
            await _spikeService.UpdateAsync(epicId, id, spike);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpike(int projectId, int epicId, int id)
    {
        try
        {
            await _spikeService.DeleteAsync(epicId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
