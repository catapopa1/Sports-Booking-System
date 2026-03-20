using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Notifications.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand() : ICommand<ErrorOr<Updated>>;