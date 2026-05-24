using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;
using SportsBookingSystem.Domain.Events;
using SportsBookingSystem.Domain.Rules;

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
                .ThenInclude(b => b.Field)
            .FirstOrDefaultAsync(bi => bi.BookingId == command.BookingId && bi.PlayerId == _currentUser.UserId, ct);

        if (invite is null)
            return Error.NotFound("Invite.NotFound", "Invite not found.");

        if (invite.Status != InviteStatus.Pending)
            return Error.Conflict("Invite.AlreadyResponded", "You have already responded to this invite.");

        var booking = invite.Booking;

        if (booking.Status is BookingStatus.Confirmed or BookingStatus.Cancelled or BookingStatus.TimedOut)
            return Error.Conflict("Booking.InvalidStatus",
                $"Cannot respond to an invite on a booking that is already {booking.Status}.");

        invite.Status = command.Response;

        var allInvites = await _dbContext.BookingInvites
            .Where(bi => bi.BookingId == command.BookingId)
            .ToListAsync(ct);

        var accepted = allInvites.Count(bi => bi.Status == InviteStatus.Accepted);
        var pending  = allInvites.Count(bi => bi.Status == InviteStatus.Pending);

        var minRequired = BookingRules.MinRequiredPlayers(booking.Field.SportType, booking.BookingType);

        // Organizer always plays, so total accepters = 1 (organizer) + accepted invitees.
        var totalAcceptors = 1 + accepted;
        var maxReachable   = totalAcceptors + pending;

        if (totalAcceptors >= minRequired)
        {
            booking.Status = BookingStatus.Confirmed;
            booking.RaiseDomainEvent(new BookingConfirmedEvent(booking.Id));
        }
        else if (maxReachable < minRequired)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.RaiseDomainEvent(new BookingCancelledEvent(booking.Id));
        }
        else
        {
            booking.Status = BookingStatus.PendingPlayerConfirmations;
        }

        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
