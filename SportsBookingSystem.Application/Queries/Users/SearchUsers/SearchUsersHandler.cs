using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Users.SearchUsers;

public class SearchUsersHandler : IQueryHandler<SearchUsersQuery, ErrorOr<List<UserSearchResultDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchUsersHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<UserSearchResultDto>>> HandleAsync(SearchUsersQuery query, CancellationToken ct)
    {
        var me = _currentUser.UserId;
        var term = query.SearchTerm.ToLower().Trim();

        var myFriendships = await _context.Friendships
            .Where(f => (f.RequesterId == me || f.AddresseeId == me)
                        && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == me ? f.AddresseeId : f.RequesterId)
            .ToListAsync(ct);

        var friendSet = myFriendships.ToHashSet();

        var users = await _context.Users
            .Where(u => u.Id != me &&
                        (u.FirstName.ToLower().Contains(term) ||
                         u.LastName.ToLower().Contains(term) ||
                         u.Email.ToLower().Contains(term)))
            .Select(u => new UserSearchResultDto(
                u.Id,
                u.FirstName + " " + u.LastName,
                u.ProfilePictureUrl,
                friendSet.Contains(u.Id)))
            .ToListAsync(ct);

        return users;
    }
}