using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Queries.Notifications.GetMyNotifications;

public class GetMyNotificationsHandler : IQueryHandler<GetMyNotificationsQuery,ErrorOr<PagedResult<NotificationDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<NotificationDto>>> HandleAsync(GetMyNotificationsQuery query,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var pagination = new PaginationParams(query.Page, query.PageSize);

        var baseQuery = _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderBy(n => n.IsRead)
            .ThenByDescending(n => n.CreatedAt);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt))
            .ToListAsync(ct);
        
        return new PagedResult<NotificationDto>(items,totalCount,query.Page,query.PageSize);
    }
}
