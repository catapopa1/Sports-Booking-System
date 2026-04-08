using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Fields.DeleteField;
using ErrorOr;

public class DeleteFieldHandler : ICommandHandler<DeleteFieldCommand,ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public DeleteFieldHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Deleted>> HandleAsync(DeleteFieldCommand command, CancellationToken ct = default)
    {
        var field = await _dbContext.Fields
            .FirstOrDefaultAsync(f => f.Id == command.FieldId, ct);

        if (field is null)
            return Error.NotFound("Field.NotFound", $"Field with id {command.FieldId} was not found");

        _dbContext.Fields.Remove(field);
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Result.Deleted;
        
    }
}