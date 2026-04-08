using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetMyInvites;

public record GetMyInvitesQuery : IQuery<ErrorOr<List<InviteNotificationDto>>>;