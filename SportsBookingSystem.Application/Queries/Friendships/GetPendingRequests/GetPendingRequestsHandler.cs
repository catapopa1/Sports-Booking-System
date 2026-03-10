using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Friendships.GetPendingRequests;

public class GetPendingRequestsHandler : IQueryHandler<GetPendingRequestsQuery, ErrorOr<List<FriendRequestDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetPendingRequestsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<FriendRequestDto>>> HandleAsync(GetPendingRequestsQuery query, CancellationToken ct = default)
    {
        var friendRequests = await _dbContext.Friendships
            .Where(f => f.AddresseeId == _currentUser.UserId && f.Status == FriendshipStatus.Requested)
            .Select(f => new FriendRequestDto(
                f.Id,
                f.RequesterId,
                f.Requester.FirstName + " " + f.Requester.LastName,
                f.Requester.ProfilePictureUrl,
                f.CreatedAt))
            .ToListAsync(ct);

        return friendRequests;
    }
}