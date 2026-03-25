using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetMyBookings;

public record GetMyBookingsQuery(int Page = 1, int PageSize = 20) : IQuery<ErrorOr<PagedResult<BookingSummaryDto>>>;
