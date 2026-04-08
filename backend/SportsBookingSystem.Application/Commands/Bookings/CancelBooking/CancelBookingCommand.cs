using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Bookings.CancelBooking;

public record CancelBookingCommand(int BookingId) : ICommand<ErrorOr<Updated>>;