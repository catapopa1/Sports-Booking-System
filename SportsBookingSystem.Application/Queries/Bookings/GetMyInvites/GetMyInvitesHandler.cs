using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetMyInvites;

public class GetMyInvitesHandler : IQueryHandler<GetMyInvitesQuery, ErrorOr<List<InviteNotificationDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetMyInvitesHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<InviteNotificationDto>>> HandleAsync(GetMyInvitesQuery query, CancellationToken ct = default)
    {
        var invites = await _dbContext.BookingInvites
            .Where(bi => bi.PlayerId == _currentUser.UserId)
            .Select(bi => new InviteNotificationDto(
                bi.BookingId,
                $"{bi.Booking.Organizer.FirstName} {bi.Booking.Organizer.LastName}",
                bi.Booking.Field.Name,
                bi.Booking.Field.Park.Name,
                bi.Booking.StartTime,
                bi.Status.ToString()))
            .ToListAsync(ct);

        return invites;
    }
}