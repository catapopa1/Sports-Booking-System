using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Friendships.GetPendingRequests;

public record GetPendingRequestsQuery() : IQuery<ErrorOr<List<FriendRequestDto>>>;