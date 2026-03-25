using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Users.SearchUsers;
using ErrorOr;
using SportsBookingSystem.Application.Commands.Users.UpdateProfile;
using SportsBookingSystem.Application.Commands.Users.UploadAvatar;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Users.GetUserProfile;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly ICurrentUserService _currentUser;
    
    private readonly IQueryHandler<SearchUsersQuery, ErrorOr<List<UserSearchResultDto>>> _searchUsersQueryHandler;
    private readonly IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>> _getUserProfileHandler;
    
    private readonly ICommandHandler<UploadAvatarCommand,ErrorOr<string>> _uploadAvatarHandler;
    private readonly ICommandHandler<UpdateProfileCommand,ErrorOr<Updated>> _updateProfileHandler;

    public UsersController(ICurrentUserService currentUser, 
        IQueryHandler<SearchUsersQuery, ErrorOr<List<UserSearchResultDto>>> searchUsersQueryHandler, 
        IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>> getUserProfileHandler, 
        ICommandHandler<UploadAvatarCommand, ErrorOr<string>> uploadAvatarHandler, 
        ICommandHandler<UpdateProfileCommand, ErrorOr<Updated>> updateProfileHandler)
    {
        _currentUser = currentUser;
        _searchUsersQueryHandler = searchUsersQueryHandler;
        _getUserProfileHandler = getUserProfileHandler;
        _uploadAvatarHandler = uploadAvatarHandler;
        _updateProfileHandler = updateProfileHandler;
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchUsers([FromQuery] string q, CancellationToken ct)
    {
        var result = await _searchUsersQueryHandler.HandleAsync(new SearchUsersQuery(q), ct);
        return result.Match(Ok, Problem);
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile(int id, CancellationToken ct)
    {
        var result = await _getUserProfileHandler.HandleAsync(new GetUserProfileQuery(id), ct);
        return result.Match(Ok, Problem);
    }
    
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await _getUserProfileHandler.HandleAsync(new GetUserProfileQuery(_currentUser.UserId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken ct)
    {
        var result = await _updateProfileHandler.HandleAsync(command, ct);
        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("me/avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var result = await _uploadAvatarHandler.HandleAsync(new UploadAvatarCommand(stream, file.FileName), ct);
        return result.Match(url => Ok(new { url }), Problem);
    }
}