using System.Net;
using System.Text.Json;

namespace SportsBookingSystem.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. Method: {Method} Path: {Path} TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await WriteErrorResposeAsync(context);
        }
    }

    private static async Task WriteErrorResposeAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = "500",
            error = "Internal Server Error",
            message = "An unexpected error occured. Please try again later"
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}