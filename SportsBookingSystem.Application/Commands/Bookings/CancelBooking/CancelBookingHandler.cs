using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Bookings.CancelBooking;

public class CancelBookingHandler : ICommandHandler<CancelBookingCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public CancelBookingHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(CancelBookingCommand command, CancellationToken ct = default)
    {
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == command.BookingId, ct);

        if (booking is null)
            return Error.NotFound("Booking.NotFound", "Booking not found.");

        if (_currentUser.UserId != booking.OrganizerId)
            return Error.Forbidden("Booking.Forbidden", "Only the organizer can cancel this booking.");

        if (booking.Status is BookingStatus.Confirmed or BookingStatus.Cancelled or BookingStatus.TimedOut)
            return Error.Conflict("Booking.InvalidStatus", $"A booking with status '{booking.Status}' cannot be cancelled.");

        booking.Status = BookingStatus.Cancelled;

        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
}