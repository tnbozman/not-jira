using StoryFirst.Api.Models;
using StoryFirst.Api.Repositories;

namespace StoryFirst.Api.Areas.UserStoryMapping.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;

    public TaskService(IRepository<TaskItem> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return task;
    }

    public async Task UpdateAsync(int id, TaskItem task)
    {
        if (id != task.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingTask = await _taskRepository.GetByIdAsync(id);

        if (existingTask == null)
        {
            throw new KeyNotFoundException("Task not found");
        }

        task.UpdatedAt = DateTime.UtcNow;
        task.CreatedAt = existingTask.CreatedAt;

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException("Task not found");
        }

        _taskRepository.Remove(task);
        await _taskRepository.SaveChangesAsync();
    }
}
