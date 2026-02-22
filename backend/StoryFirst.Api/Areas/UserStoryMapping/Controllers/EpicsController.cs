using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/themes/{themeId}/[controller]")]
public class EpicsController : BaseApiController
{
    private readonly IEpicRepository _epicRepository;

    public EpicsController(IEpicRepository epicRepository)
    {
        _epicRepository = epicRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Epic>>> GetEpics(int projectId, int themeId)
    {
        var epics = await _epicRepository.GetByThemeIdAsync(themeId);
        return Ok(epics);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Epic>> GetEpic(int projectId, int themeId, int id)
    {
        var epic = await _epicRepository.FirstOrDefaultAsync(e => e.ThemeId == themeId && e.Id == id);

        if (epic == null)
        {
            return NotFound();
        }

        var epicDetails = await _epicRepository.GetWithDetailsAsync(id);
        return Ok(epicDetails);
    }

    [HttpPost]
    public async Task<ActionResult<Epic>> CreateEpic(int projectId, int themeId, Epic epic)
    {
        epic.ThemeId = themeId;
        epic.CreatedAt = DateTime.UtcNow;
        epic.UpdatedAt = DateTime.UtcNow;

        await _epicRepository.AddAsync(epic);
        await _epicRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEpic), new { projectId, themeId, id = epic.Id }, epic);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEpic(int projectId, int themeId, int id, Epic epic)
    {
        if (id != epic.Id)
        {
            return BadRequest();
        }

        var existingEpic = await _epicRepository.FirstOrDefaultAsync(e => e.Id == id && e.ThemeId == themeId);

        if (existingEpic == null)
        {
            return NotFound();
        }

        existingEpic.Name = epic.Name;
        existingEpic.Description = epic.Description;
        existingEpic.Order = epic.Order;
        existingEpic.OutcomeId = epic.OutcomeId;
        existingEpic.UpdatedAt = DateTime.UtcNow;

        _epicRepository.Update(existingEpic);
        await _epicRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEpic(int projectId, int themeId, int id)
    {
        var epic = await _epicRepository.FirstOrDefaultAsync(e => e.Id == id && e.ThemeId == themeId);

        if (epic == null)
        {
            return NotFound();
        }

        _epicRepository.Remove(epic);
        await _epicRepository.SaveChangesAsync();

        return NoContent();
    }
}
