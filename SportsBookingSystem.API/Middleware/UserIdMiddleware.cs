using System.Security.Claims;
using Serilog.Context;

namespace SportsBookingSystem.API.Middleware;

public class UserIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

        using (LogContext.PushProperty("UserId", userId))
        {
            await next(context);
        }
    }
}