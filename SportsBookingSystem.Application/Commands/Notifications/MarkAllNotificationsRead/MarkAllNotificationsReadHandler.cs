using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Notifications.MarkAllNotificationsRead;

public class MarkAllNotificationsReadHandler : ICommandHandler<MarkAllNotificationsReadCommand,ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsReadHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(MarkAllNotificationsReadCommand command,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        await _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);

        return Result.Updated;
    }
}