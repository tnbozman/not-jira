using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;

    public StoryService(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository;
    }

    public async Task<IEnumerable<Story>> GetByEpicIdAsync(int epicId)
    {
        return await _storyRepository.GetByEpicIdAsync(epicId);
    }

    public async Task<Story?> GetByIdAsync(int epicId, int id)
    {
        var story = await _storyRepository.FirstOrDefaultAsync(s => s.EpicId == epicId && s.Id == id);

        if (story == null)
        {
            return null;
        }

        return await _storyRepository.GetWithDetailsAsync(id);
    }

    public async Task<Story> CreateAsync(int epicId, Story story)
    {
        story.EpicId = epicId;
        story.CreatedAt = DateTime.UtcNow;
        story.UpdatedAt = DateTime.UtcNow;

        await _storyRepository.AddAsync(story);
        await _storyRepository.SaveChangesAsync();

        return story;
    }

    public async Task UpdateAsync(int epicId, int id, Story story)
    {
        if (id != story.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingStory = await _storyRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (existingStory == null)
        {
            throw new KeyNotFoundException("Story not found");
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
    }

    public async Task DeleteAsync(int epicId, int id)
    {
        var story = await _storyRepository.FirstOrDefaultAsync(s => s.Id == id && s.EpicId == epicId);

        if (story == null)
        {
            throw new KeyNotFoundException("Story not found");
        }

        _storyRepository.Remove(story);
        await _storyRepository.SaveChangesAsync();
    }
}
