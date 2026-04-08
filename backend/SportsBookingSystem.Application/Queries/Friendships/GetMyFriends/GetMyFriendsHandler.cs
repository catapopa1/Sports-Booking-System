using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Friendships.GetMyFriends;

public class GetMyFriendsHandler : IQueryHandler<GetMyFriendsQuery,ErrorOr<List<FriendDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetMyFriendsHandler(IApplicationDbContext dbContext , ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<FriendDto>>> HandleAsync(GetMyFriendsQuery query,
        CancellationToken ct)
    {
        var friends = await _dbContext.Friendships
            .Where(f => (f.RequesterId == _currentUser.UserId || f.AddresseeId == _currentUser.UserId) && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == _currentUser.UserId
                ? new FriendDto(f.AddresseeId, f.Addressee.FirstName + " " + f.Addressee.LastName, f.Addressee.ProfilePictureUrl)
                : new FriendDto(f.RequesterId, f.Requester.FirstName + " " + f.Requester.LastName, f.Requester.ProfilePictureUrl))
            .ToListAsync(ct);
        return friends;
    }
}