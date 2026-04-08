using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;
using SportsBookingSystem.Domain.Events;

namespace SportsBookingSystem.Application.Commands.Bookings.RespondToInvite;

public class RespondToInviteHandler : ICommandHandler<RespondToInviteCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public RespondToInviteHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(RespondToInviteCommand command, CancellationToken ct = default)
    {
        if (command.Response == InviteStatus.Pending)
            return Error.Validation("Invite.InvalidResponse", "Response must be Accepted or Declined.");

        var invite = await _dbContext.BookingInvites
            .Include(bi => bi.Booking)
            .FirstOrDefaultAsync(bi => bi.BookingId == command.BookingId && bi.PlayerId == _currentUser.UserId, ct);

        if (invite is null)
            return Error.NotFound("Invite.NotFound", "Invite not found.");

        if (invite.Status != InviteStatus.Pending)
            return Error.Conflict("Invite.AlreadyResponded", "You have already responded to this invite.");

        invite.Status = command.Response;
        var booking = invite.Booking;

        if (command.Response == InviteStatus.Declined)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.RaiseDomainEvent(new BookingCancelledEvent(booking.Id));
        }
        else
        {
            booking.Status = BookingStatus.PendingPlayerConfirmations;

            var anyStillPending = await _dbContext.BookingInvites.AnyAsync(
                bi => bi.BookingId == command.BookingId && bi.Status == InviteStatus.Pending, ct);

            if (!anyStillPending)
            {
                booking.Status = BookingStatus.PendingManagerApproval;
                booking.RaiseDomainEvent(new AllPlayersAcceptedEvent(booking.Id));
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
}