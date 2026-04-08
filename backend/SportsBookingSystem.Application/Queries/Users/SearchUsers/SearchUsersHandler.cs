using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Users.SearchUsers;

public class SearchUsersHandler : IQueryHandler<SearchUsersQuery, ErrorOr<PagedResult<UserSearchResultDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchUsersHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<UserSearchResultDto>>> HandleAsync(SearchUsersQuery query, CancellationToken ct)
    {
        var me = _currentUser.UserId;
        var term = query.SearchTerm.ToLower().Trim();

        var myFriendships = await _context.Friendships
            .Where(f => (f.RequesterId == me || f.AddresseeId == me)
                        && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == me ? f.AddresseeId : f.RequesterId)
            .ToListAsync(ct);

        var friendSet = myFriendships.ToHashSet();

        var baseQuery = _context.Users
            .Where(u => u.Id != me &&
                        (u.FirstName.ToLower().Contains(term) ||
                         u.LastName.ToLower().Contains(term) ||
                         u.Email.ToLower().Contains(term)));

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserSearchResultDto(
                u.Id,
                u.FirstName + " " + u.LastName,
                u.ProfilePictureUrl,
                friendSet.Contains(u.Id)))
            .ToListAsync(ct);

        return new PagedResult<UserSearchResultDto>(items, totalCount, query.Page, query.PageSize);
    }
}