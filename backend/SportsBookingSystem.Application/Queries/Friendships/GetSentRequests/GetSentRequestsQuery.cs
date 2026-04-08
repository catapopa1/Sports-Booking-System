using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Friendships.GetSentRequests;

public record GetSentRequestsQuery() : IQuery<ErrorOr<List<FriendRequestDto>>>;