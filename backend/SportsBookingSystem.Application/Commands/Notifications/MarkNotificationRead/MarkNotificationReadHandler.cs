using SportsBookingSystem.Application.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Notifications.MarkNotificationRead;

public class MarkNotificationReadHandler : ICommandHandler<MarkNotificationReadCommand,ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(MarkNotificationReadCommand command, CancellationToken ct = default)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == command.NotificationId, ct);

        if (notification is null)
            return Error.NotFound("Notification.NotFound", "Notification not found.");


        if (notification.UserId != _currentUser.UserId)
            return Error.Forbidden("Notification.Forbidden", "This notification does not belong to you.");
        
        notification.IsRead = true;
        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
 }