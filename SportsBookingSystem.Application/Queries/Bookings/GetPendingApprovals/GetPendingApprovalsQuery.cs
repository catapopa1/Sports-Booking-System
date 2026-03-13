using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetPendingApprovals;

public record GetPendingApprovalsQuery : IQuery<ErrorOr<List<BookingSummaryDto>>>;