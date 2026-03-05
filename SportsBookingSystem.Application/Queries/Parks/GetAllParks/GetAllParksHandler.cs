using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Queries.Parks.GetAllParks;

public class GetAllParksHandler : IQueryHandler<GetAllParksQuery,ErrorOr<List<ParkSummaryDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllParksHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<ParkSummaryDto>>> HandleAsync(GetAllParksQuery request, CancellationToken ct = default)
    {
        var parks = await _dbContext.Parks
            .Select(p => new ParkSummaryDto(
                p.Id,
                p.Name,
                p.City,
                p.Fields.Count))
            .ToListAsync(ct);

        return parks;
    }
}