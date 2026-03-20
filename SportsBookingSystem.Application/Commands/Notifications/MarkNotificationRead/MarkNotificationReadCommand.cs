using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Notifications.MarkNotificationRead;

public record MarkNotificationReadCommand(int NotificationId) 
    : ICommand<ErrorOr<Updated>>;