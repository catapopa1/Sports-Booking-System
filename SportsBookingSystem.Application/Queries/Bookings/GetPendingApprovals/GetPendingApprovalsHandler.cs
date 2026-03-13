using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Bookings.GetPendingApprovals;

public class GetPendingApprovalsHandler : IQueryHandler<GetPendingApprovalsQuery, ErrorOr<List<BookingSummaryDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetPendingApprovalsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<BookingSummaryDto>>> HandleAsync(GetPendingApprovalsQuery query, CancellationToken ct = default)
    {
        var bookings = await _dbContext.Bookings
            .Where(b => b.Status == BookingStatus.PendingManagerApproval &&
                        b.Field.Park.ManagerId == _currentUser.UserId)
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