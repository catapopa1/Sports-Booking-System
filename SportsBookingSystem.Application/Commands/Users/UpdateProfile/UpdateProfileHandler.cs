using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Users.UpdateProfile;

public class UpdateProfileHandler : ICommandHandler<UpdateProfileCommand,ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(UpdateProfileCommand command, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId,ct);

        if (user is null)
            return Error.NotFound("User.NotFound", "The specified user does not exist");

        user.Bio = command.bio;

        await _dbContext.SaveChangesAsync(ct);

        return Result.Updated;
    }
}