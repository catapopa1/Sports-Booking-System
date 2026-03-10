using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Friendships.GetSentRequests;

public class GetSentRequestsHandler : IQueryHandler<GetSentRequestsQuery, ErrorOr<List<FriendRequestDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetSentRequestsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<FriendRequestDto>>> HandleAsync(GetSentRequestsQuery query, CancellationToken ct = default)
    {
        var friendRequests = await _dbContext.Friendships
            .Where(f => f.RequesterId == _currentUser.UserId && f.Status == FriendshipStatus.Requested)
            .Select(f => new FriendRequestDto(
                f.Id,
                f.AddresseeId,
                f.Addressee.FirstName + " " + f.Addressee.LastName,
                f.Addressee.ProfilePictureUrl,
                f.CreatedAt))
            .ToListAsync(ct);

        return friendRequests;
    }
}