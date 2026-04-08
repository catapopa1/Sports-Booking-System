using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Friendships.RespondToFriendRequestCommand;

public class RespondToFriendRequestHandler : ICommandHandler<RespondtoFriendRequestCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    
    public RespondToFriendRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUser = currentUserService;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(RespondtoFriendRequestCommand command,
        CancellationToken ct = default)
    {
        if (command.NewStatus is not (FriendshipStatus.Accepted or FriendshipStatus.Rejected))
            return Error.Validation("Friendship.InvalidStatus", "You can only accept or reject a friend request.");

        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.Id == command.FriendshipId, ct);

        if (friendship is null)
            return Error.NotFound("Friendship.NotFound", "Friend request not found.");

        if (friendship.AddresseeId != _currentUser.UserId)
            return Error.Forbidden("Friendship.Forbidden", "You can only respond to requests sent to you.");

        if (friendship.Status != FriendshipStatus.Requested)
            return Error.Conflict("Friendship.NotRequested", "This request has already been responded to.");

        friendship.Status = command.NewStatus;

        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;

    }
}