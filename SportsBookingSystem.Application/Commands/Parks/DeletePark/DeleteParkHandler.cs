using ErrorOr;
using SportsBookingSystem.Application.Common;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Parks.DeletePark;

public class DeleteParkHandler : ICommandHandler<DeleteParkCommand,ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    public DeleteParkHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Deleted>> HandleAsync(DeleteParkCommand command, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .FirstOrDefaultAsync(p => p.Id == command.ParkId, ct);

        if (park is null)
            return Error.NotFound("Park.NotFound", $"Park with id{command.ParkId} was not found");
        
        _dbContext.Parks.Remove(park);

        await _dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}