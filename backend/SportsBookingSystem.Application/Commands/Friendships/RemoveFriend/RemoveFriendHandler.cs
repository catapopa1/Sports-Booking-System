using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Friendships.RemoveFriend;

public class RemoveFriendHandler : ICommandHandler<RemoveFriendCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public RemoveFriendHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Deleted>> HandleAsync(RemoveFriendCommand command, CancellationToken ct)
    {
        var friendship = await  _dbContext.Friendships.FirstOrDefaultAsync(f => f.Id == command.FriendShipId,ct);

        if (friendship is null)
            return Error.NotFound("Friendship.NotFound", "Friendship not found.");

        if (friendship.RequesterId != _currentUser.UserId && friendship.AddresseeId != _currentUser.UserId)
            return Error.Forbidden("Friendship.Forbidden", "You are not part of this friendship.");

        _dbContext.Friendships.Remove(friendship);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }

}