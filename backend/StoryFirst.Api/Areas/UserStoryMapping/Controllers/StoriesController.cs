using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/projects/{projectId}/epics/{epicId}/[controller]")]
public class StoriesController : BaseApiController
{
    private readonly IStoryRepository _storyRepository;

    public StoriesController(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Story>>> GetStories(int projectId, int epicId)
    {
        var stories = await _storyRepository.GetByEpicIdAsync(epicId);
        return Ok(stories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Story>> GetStory(int projectId, int epicId, int id)
    {
        var story = await _storyRepository.FirstOrDefaultAsync(s => s.EpicId == epicId && s.Id == id);

        if (story == null)
        {
            return NotFound();
        }

        var storyDetails = await _storyRepository.GetWithDetailsAsync(id);
        return Ok(storyDetails);
    }

    [HttpPost]
    public async Task<ActionResult<Story>> CreateStory(int projectId, int epicId, Story story)
    {
        story.EpicId = epicId;
        story.CreatedAt = DateTime.UtcNow;
        story.UpdatedAt = DateTime.UtcNow;

        await _storyRepository.AddAsync(story);
        await _storyRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStory), new { projectId, epicId, id = story.Id }, story);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStory(int projectId, int epicId, int id, Story story)
    {
        if (id != story.Id)
        {
            return BadRequest();
        }

        var existingStory = await _storyRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (existingStory == null)
        {
            return NotFound();
        }

        existingStory.Title = story.Title;
        existingStory.Description = story.Description;
        existingStory.SolutionDescription = story.SolutionDescription;
        existingStory.AcceptanceCriteria = story.AcceptanceCriteria;
        existingStory.Order = story.Order;
        existingStory.Priority = story.Priority;
        existingStory.Status = story.Status;
        existingStory.StoryPoints = story.StoryPoints;
        existingStory.SprintId = story.SprintId;
        existingStory.ReleaseId = story.ReleaseId;
        existingStory.TeamId = story.TeamId;
        existingStory.AssigneeId = story.AssigneeId;
        existingStory.AssigneeName = story.AssigneeName;
        existingStory.OutcomeId = story.OutcomeId;
        existingStory.UpdatedAt = DateTime.UtcNow;

        _storyRepository.Update(existingStory);
        await _storyRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStory(int projectId, int epicId, int id)
    {
        var story = await _storyRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (story == null)
        {
            return NotFound();
        }

        _storyRepository.Remove(story);
        await _storyRepository.SaveChangesAsync();

        return NoContent();
    }
}
