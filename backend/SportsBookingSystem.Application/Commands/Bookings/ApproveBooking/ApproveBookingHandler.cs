using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;
using SportsBookingSystem.Domain.Events;

namespace SportsBookingSystem.Application.Commands.Bookings.ApproveBooking;

public class ApproveBookingHandler : ICommandHandler<ApproveBookingCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public ApproveBookingHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(ApproveBookingCommand command, CancellationToken ct = default)
    {
        var booking = await _dbContext.Bookings
            .Include(b => b.Field)
            .ThenInclude(f => f.Park)
            .FirstOrDefaultAsync(b => b.Id == command.BookingId, ct);

        if (booking is null)
            return Error.NotFound("Booking.NotFound", "Booking not found.");

        if (_currentUser.UserId != booking.Field.Park.ManagerId)
            return Error.Forbidden("Booking.Forbidden", "Only the park manager can approve this booking.");

        if (booking.Status != BookingStatus.PendingManagerApproval)
            return Error.Conflict("Booking.InvalidStatus", "Booking is not awaiting manager approval.");

        booking.Status = BookingStatus.Confirmed;
        booking.RaiseDomainEvent(new BookingConfirmedEvent(booking.Id));

        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
}