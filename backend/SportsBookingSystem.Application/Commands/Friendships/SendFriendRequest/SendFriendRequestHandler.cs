using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Friendships.SendFriendRequest;

public class SendFriendRequestHandler : ICommandHandler<SendFriendRequestCommand, ErrorOr<int>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<SendFriendRequestCommand> _validator;

    public SendFriendRequestHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUser,
        IValidator<SendFriendRequestCommand> validator)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<ErrorOr<int>> HandleAsync(SendFriendRequestCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        var requesterId = _currentUser.UserId;

        if (requesterId == command.AddresseeId)
            return Error.Validation("Friendship.SelfRequest", "You cannot send a friend request to yourself.");

        var alreadyExists = await _dbContext.Friendships.AnyAsync(f =>
            (f.RequesterId == requesterId && f.AddresseeId == command.AddresseeId ||
             f.RequesterId == command.AddresseeId && f.AddresseeId == requesterId)
            && f.Status != FriendshipStatus.Rejected, ct);

        if (alreadyExists)
            return Error.Conflict("Friendship.Conflict", "A friendship or pending request already exists.");

        var friendship = new Friendship
        {
            RequesterId = requesterId,
            AddresseeId = command.AddresseeId,
            Status = FriendshipStatus.Requested,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Friendships.Add(friendship);
        await _dbContext.SaveChangesAsync(ct);
        return friendship.Id;
    }
}