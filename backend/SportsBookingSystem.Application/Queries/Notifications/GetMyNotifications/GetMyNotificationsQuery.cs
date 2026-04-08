using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
namespace SportsBookingSystem.Application.Queries.Notifications.GetMyNotifications;

public record GetMyNotificationsQuery (int Page = 1, int PageSize = 20) 
    : IQuery<ErrorOr<PagedResult<NotificationDto>>>;