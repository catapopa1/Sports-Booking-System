using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Friendships.RespondToFriendRequestCommand;

public record RespondtoFriendRequestCommand
(
    int FriendshipId, 
    FriendshipStatus NewStatus
) : ICommand;