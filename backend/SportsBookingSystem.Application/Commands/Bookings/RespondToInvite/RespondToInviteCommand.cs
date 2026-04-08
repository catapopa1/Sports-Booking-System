using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Bookings.RespondToInvite;

public record RespondToInviteCommand(int BookingId, InviteStatus Response) : ICommand<ErrorOr<Updated>>;