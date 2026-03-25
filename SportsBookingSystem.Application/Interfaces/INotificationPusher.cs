using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Interfaces;

public interface INotificationPusher
{
    Task PushAsync(int userId, NotificationDto notification, CancellationToken ct = default);
}