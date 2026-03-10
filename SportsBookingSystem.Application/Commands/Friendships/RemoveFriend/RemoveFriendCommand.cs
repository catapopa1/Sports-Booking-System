using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Domain.Entities;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Friendships.RemoveFriend;

public record RemoveFriendCommand(
    int FriendShipId) : ICommand;