using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public int UserId { get; }

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        var claim = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserId = int.TryParse(claim, out var id) ? id : throw new UnauthorizedAccessException();
    }
}