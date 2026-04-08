using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetBookingById;

public record GetBookingByIdQuery(int BookingId) : IQuery<ErrorOr<BookingDto>>;