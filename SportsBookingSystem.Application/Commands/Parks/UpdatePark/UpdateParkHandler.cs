using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Parks.UpdatePark;

public class UpdateParkHandler : ICommandHandler<UpdateParkCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public UpdateParkHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(UpdateParkCommand command, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .FirstOrDefaultAsync(p => p.Id == command.ParkId, ct);

        if (park is null)
            return Error.NotFound("Park.NotFound", $"Park with id{command.ParkId} was not found");
        
        park.Name = command.Name;
        park.Address = command.Address;
        park.City = command.City;

        await _dbContext.SaveChangesAsync(ct);

        return Result.Updated;
    }
}