using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Application.Queries.Users.GetAllUsers;

public class GetAllUsersHandler : IQueryHandler<GetAllUsersQuery,ErrorOr<PagedResult<AdminUserDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public GetAllUsersHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PagedResult<AdminUserDto>>> HandleAsync(GetAllUsersQuery query, CancellationToken ct = default)
    {
        var baseQuery = _dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            baseQuery = baseQuery.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term));
        }
        
        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new AdminUserDto(u.Id, u.FirstName + " " + u.LastName, u.Email, u.Role.ToString()))
            .ToListAsync(ct);
        
        return new PagedResult<AdminUserDto>(items, totalCount, query.Page, query.PageSize);
    }
}