using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Friendships.RemoveFriend;

public record RemoveFriendCommand(
    int FriendShipId) : ICommand;