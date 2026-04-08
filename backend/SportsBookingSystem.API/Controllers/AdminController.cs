using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Users.GetAllUsers;
using SportsBookingSystem.Application.Commands.Users.UpdateUserRole;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly IQueryHandler<GetAllUsersQuery, ErrorOr<PagedResult<AdminUserDto>>> _getAllUsersHandler;
    private readonly ICommandHandler<UpdateUserRoleCommand, ErrorOr<Updated>> _updateUserRoleHandler;

    public AdminController(
        IQueryHandler<GetAllUsersQuery, ErrorOr<PagedResult<AdminUserDto>>> getAllUsersHandler,
        ICommandHandler<UpdateUserRoleCommand, ErrorOr<Updated>> updateUserRoleHandler)
    {
        _getAllUsersHandler = getAllUsersHandler;
        _updateUserRoleHandler = updateUserRoleHandler;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _getAllUsersHandler.HandleAsync(new GetAllUsersQuery(q, page, pageSize), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRole newRole, CancellationToken ct)
    {
        var result = await _updateUserRoleHandler.HandleAsync(new UpdateUserRoleCommand(id, newRole), ct);
        return result.Match(_ => Ok(), Problem);
    }
}