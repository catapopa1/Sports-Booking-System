using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetMyBookings;

public record GetMyBookingsQuery : IQuery<ErrorOr<List<BookingSummaryDto>>>;