using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Users.SearchUsers;
using ErrorOr;
using SportsBookingSystem.Application.Queries.Users.GetUserProfile;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IQueryHandler<SearchUsersQuery, ErrorOr<List<UserSearchResultDto>>> _searchUsersQueryHandler;
    private readonly IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>> _getUserProfileHandler;

    public UsersController(
        IQueryHandler<SearchUsersQuery, ErrorOr<List<UserSearchResultDto>>> searchUsersQueryHandler,
        IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>> getUserProfileHandler)
    {
        _searchUsersQueryHandler = searchUsersQueryHandler;
        _getUserProfileHandler = getUserProfileHandler;
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchUsers([FromQuery] string q, CancellationToken ct)
    {
        var result = await _searchUsersQueryHandler.HandleAsync(new SearchUsersQuery(q), ct);
        return result.Match(Ok, Problem);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserProfile(int id, CancellationToken ct)
    {
        var result = await _getUserProfileHandler.HandleAsync(new GetUserProfileQuery(id), ct);
        return result.Match(Ok, Problem);
    }

}