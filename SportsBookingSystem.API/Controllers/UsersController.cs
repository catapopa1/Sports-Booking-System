using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Users.SearchUsers;
using ErrorOr;
using SportsBookingSystem.Application.Commands.Users.ChangePassword;
using SportsBookingSystem.Application.Commands.Users.UpdateProfile;
using SportsBookingSystem.Application.Commands.Users.UploadAvatar;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Users.GetUserProfile;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly ICurrentUserService _currentUser;
    
    private readonly IQueryHandler<SearchUsersQuery, ErrorOr<PagedResult<UserSearchResultDto>>> _searchUsersQueryHandler;
    private readonly IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>> _getUserProfileHandler;
    
    private readonly ICommandHandler<UploadAvatarCommand,ErrorOr<string>> _uploadAvatarHandler;
    private readonly ICommandHandler<UpdateProfileCommand,ErrorOr<Updated>> _updateProfileHandler;
    private readonly ICommandHandler<ChangePasswordCommand,ErrorOr<Updated>> _changePasswordHandler;

    public UsersController(ICurrentUserService currentUser, 
        IQueryHandler<SearchUsersQuery, ErrorOr<PagedResult<UserSearchResultDto>>> searchUsersQueryHandler,
        IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>> getUserProfileHandler, 
        ICommandHandler<UploadAvatarCommand, ErrorOr<string>> uploadAvatarHandler, 
        ICommandHandler<UpdateProfileCommand, ErrorOr<Updated>> updateProfileHandler,
        ICommandHandler<ChangePasswordCommand, ErrorOr<Updated>> changePasswordHandler)
    {
        _currentUser = currentUser;
        _searchUsersQueryHandler = searchUsersQueryHandler;
        _getUserProfileHandler = getUserProfileHandler;
        _uploadAvatarHandler = uploadAvatarHandler;
        _updateProfileHandler = updateProfileHandler;
        _changePasswordHandler = changePasswordHandler;
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchUsers([FromQuery] string q,[FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _searchUsersQueryHandler.HandleAsync(new SearchUsersQuery(q,page,pageSize), ct);
        return result.Match(Ok, Problem);
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile(int id, CancellationToken ct = default)
    {
        var result = await _getUserProfileHandler.HandleAsync(new GetUserProfileQuery(id), ct);
        return result.Match(Ok, Problem);
    }
    
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct = default)
    {
        var result = await _getUserProfileHandler.HandleAsync(new GetUserProfileQuery(_currentUser.UserId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken ct = default)
    {
        var result = await _updateProfileHandler.HandleAsync(command, ct);
        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("me/avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct = default)
    {
        using var stream = file.OpenReadStream();
        var result = await _uploadAvatarHandler.HandleAsync(new UploadAvatarCommand(stream, file.FileName), ct);
        return result.Match(url => Ok(new { url }), Problem);
    }

    [HttpPut("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct)
    {
        var result = await _changePasswordHandler.HandleAsync(command, ct);
        return result.Match(_ => Ok(), Problem);
    }
}