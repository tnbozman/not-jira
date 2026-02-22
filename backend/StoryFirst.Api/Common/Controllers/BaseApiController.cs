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
    protected ActionResult<T> HandleResult<T>(T? result)
    {
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
