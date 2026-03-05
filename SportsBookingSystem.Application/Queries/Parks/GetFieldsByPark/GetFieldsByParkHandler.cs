using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
namespace SportsBookingSystem.Application.Queries.Parks.GetFieldsByPark;

public class GetFieldsByParkHandler : IQueryHandler<GetFieldsByParkQuery,ErrorOr<List<FieldDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetFieldsByParkHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<FieldDto>>> HandleAsync(GetFieldsByParkQuery query, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks.AnyAsync(p => p.Id == query.ParkId, ct);
        if (!park)
            return Error.NotFound("Park.NotFound", $"Park with ID {query.ParkId} was not found.");
        
        var fields = await _dbContext.Fields
            .Where(f => f.ParkId == query.ParkId)
            .Select(f => new FieldDto(
                f.Id,
                f.Name,
                f.SportType.ToString(),
                f.BaseHourlyPrice,
                f.ParkId,
                f.Park.Name))
            .ToListAsync(ct);

        return fields;
    }
}