using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StoryFirst.Api.Common.Controllers;

/// <summary>
/// Base controller with common functionality
/// </summary>
[Authorize]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleException(Exception ex)
    {
        // Log the exception (injected logger would be used in derived classes)
        // The global exception handler will catch this, but we can provide
        // additional context-specific handling here if needed
        throw ex;
    }

    protected ActionResult<T> HandleResult<T>(T? result)
    {
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
