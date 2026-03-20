using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Commands.Notifications.MarkAllNotificationsRead;
using SportsBookingSystem.Application.Commands.Notifications.MarkNotificationRead;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Notifications.GetMyNotifications;

namespace SportsBookingSystem.API.Controllers;
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : BaseController
{
    private readonly IQueryHandler<GetMyNotificationsQuery, ErrorOr<PagedResult<NotificationDto>>> _getMyNotifications;
    private readonly ICommandHandler<MarkNotificationReadCommand, ErrorOr<Updated>> _markRead;
    private readonly ICommandHandler<MarkAllNotificationsReadCommand, ErrorOr<Updated>> _markAllRead;

    public NotificationsController(
        IQueryHandler<GetMyNotificationsQuery, ErrorOr<PagedResult<NotificationDto>>> getMyNotifications,
        ICommandHandler<MarkNotificationReadCommand, ErrorOr<Updated>> markRead,
        ICommandHandler<MarkAllNotificationsReadCommand, ErrorOr<Updated>> markAllRead)
    {
        _getMyNotifications = getMyNotifications;
        _markRead = markRead;
        _markAllRead = markAllRead;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _getMyNotifications.HandleAsync(new GetMyNotificationsQuery(page, pageSize), ct);
        return result.Match(Ok,Problem);
    }
    
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken ct)
    {
        var result = await _markRead.HandleAsync(new MarkNotificationReadCommand(id), ct);
        return result.Match(_ => Ok(), Problem);
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var result = await _markAllRead.HandleAsync(new MarkAllNotificationsReadCommand(), ct);
        return result.Match(_ => Ok(), Problem);
    }
}