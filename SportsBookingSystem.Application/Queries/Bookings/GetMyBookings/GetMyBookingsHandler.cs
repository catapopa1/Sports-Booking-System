using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetMyBookings;

public class GetMyBookingsHandler : IQueryHandler<GetMyBookingsQuery, ErrorOr<List<BookingSummaryDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetMyBookingsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<BookingSummaryDto>>> HandleAsync(GetMyBookingsQuery query, CancellationToken ct = default)
    {
        var bookings = await _dbContext.Bookings
            .Where(b => b.OrganizerId == _currentUser.UserId)
            .OrderByDescending(b => b.StartTime)
            .Select(b => new BookingSummaryDto(
                b.Id,
                b.Field.Name,
                b.Field.Park.Name,
                b.StartTime,
                b.Status.ToString(),
                b.TotalPrice))
            .ToListAsync(ct);

        return bookings;
    }
}