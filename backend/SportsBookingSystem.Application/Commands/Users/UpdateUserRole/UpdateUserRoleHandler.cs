using SportsBookingSystem.Application.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
namespace SportsBookingSystem.Application.Commands.Users.UpdateUserRole;

public class UpdateUserRoleHandler : ICommandHandler<UpdateUserRoleCommand,ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserRoleHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(UpdateUserRoleCommand command, CancellationToken ct = default)
    {
        if (command.UserId == _currentUser.UserId)
            return Error.Conflict("Admin.SelfRoleChange", "You cannot change your own role");

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
            return Error.NotFound("User.NotFound", "The specified user does not exist");

        user.Role = command.NewRole;

        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
}