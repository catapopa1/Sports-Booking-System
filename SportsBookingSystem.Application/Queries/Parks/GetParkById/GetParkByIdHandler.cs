using SportsBookingSystem.Application.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Queries.Parks.GetParkById;

public class GetParkByIdHandler : IQueryHandler<GetParkByIdQuery, ErrorOr<ParkDto>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public GetParkByIdHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ParkDto>> HandleAsync(GetParkByIdQuery query, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .Where(p => p.Id == query.ParkId)
            .Select(p => new ParkDto(
                p.Id,
                p.Name,
                p.Address,
                p.City,
                p.ManagerId,
                p.Manager.FirstName + " " + p.Manager.LastName
            ))
            .FirstOrDefaultAsync(ct);

        if (park is null)
            return Error.NotFound("Park.NotFound", $"Park with ID {query.ParkId} was not found.");
        
        return park;
    }
}