using Microsoft.AspNetCore.Mvc;
using StoryFirst.Api.Common.Controllers;
using StoryFirst.Api.Models;
using StoryFirst.Api.Areas.UserStoryMapping.Services;

namespace StoryFirst.Api.Areas.UserStoryMapping.Controllers;

[Area("UserStoryMapping")]
[Route("api/[controller]")]
public class TasksController : BaseApiController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _taskService.GetByIdAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        return task;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
    {
        try
        {
            var result = await _taskService.CreateAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
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
    public async Task<IActionResult> UpdateTask(int id, TaskItem task)
    {
        try
        {
            await _taskService.UpdateAsync(id, task);
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
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
