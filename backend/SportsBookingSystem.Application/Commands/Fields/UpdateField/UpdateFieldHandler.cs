using SportsBookingSystem.Application.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Fields.UpdateField;

public class UpdateFieldHandler : ICommandHandler<UpdateFieldCommand,ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public UpdateFieldHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(UpdateFieldCommand command, CancellationToken ct = default)
    {
        var field = await _dbContext.Fields
            .FirstOrDefaultAsync(f => f.Id == command.FieldId, ct);

        if (field is null)
            return Error.NotFound("Field.NotFound", $"Field with id {command.FieldId} was not found");

        field.Name = command.Name;
        field.BaseHourlyPrice = command.BaseHourlyPrice;

        await _dbContext.SaveChangesAsync(ct);

        return Result.Updated;
    }
}