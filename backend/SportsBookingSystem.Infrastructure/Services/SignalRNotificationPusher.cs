using Microsoft.AspNetCore.SignalR;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Infrastructure.Hubs;

namespace SportsBookingSystem.Infrastructure.Services;

public class SignalRNotificationPusher : INotificationPusher
{
    private readonly IHubContext<NotificationHub> _hub;
    
    public SignalRNotificationPusher(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task PushAsync(int userId, NotificationDto notification, CancellationToken ct = default)
    {
        await _hub.Clients
            .Group($"user-{userId}")
            .SendAsync("ReceiveNotification", notification, ct);
    }
}