namespace SportsBookingSystem.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using ErrorOr;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        var first = errors.First();

        var statusCode = first.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return Problem(title: first.Description,statusCode: statusCode);
    }
}