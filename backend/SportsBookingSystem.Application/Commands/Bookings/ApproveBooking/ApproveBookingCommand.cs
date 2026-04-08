using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Bookings.ApproveBooking;

public record ApproveBookingCommand(int BookingId) : ICommand<ErrorOr<Updated>>;