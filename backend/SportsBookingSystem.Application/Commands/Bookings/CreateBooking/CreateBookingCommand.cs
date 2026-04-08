using SportsBookingSystem.Domain.Enums;
using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Bookings.CreateBooking;

public record CreateBookingCommand(
    int FieldId,
    DateTimeOffset StartDate,
    BookingType BookingType,
    List<int> InvitedPlayersIds
) : ICommand<ErrorOr<int>>;