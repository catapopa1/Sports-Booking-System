using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Friendships.SendFriendRequest;

public record SendFriendRequestCommand(int AddresseeId) : ICommand<ErrorOr<int>>;