using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetMyBookings;

public class GetMyBookingsHandler : IQueryHandler<GetMyBookingsQuery, ErrorOr<PagedResult<BookingSummaryDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetMyBookingsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<BookingSummaryDto>>> HandleAsync(GetMyBookingsQuery query, CancellationToken ct = default)
    {
        var baseQuery = _dbContext.Bookings
            .Where(b => b.OrganizerId == _currentUser.UserId);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(b => b.StartTime)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => new BookingSummaryDto(
                b.Id,
                b.Field.Name,
                b.Field.Park.Name,
                b.StartTime,
                b.Status.ToString(),
                b.TotalPrice))
            .ToListAsync(ct);

        return new PagedResult<BookingSummaryDto>(items, totalCount, query.Page, query.PageSize);
    }
}