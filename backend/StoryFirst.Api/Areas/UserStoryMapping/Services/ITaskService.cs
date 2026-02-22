using StoryFirst.Api.Models;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task UpdateAsync(int id, TaskItem task);
    Task DeleteAsync(int id);
}
