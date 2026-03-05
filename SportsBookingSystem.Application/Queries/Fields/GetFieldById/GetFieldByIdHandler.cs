using SportsBookingSystem.Application.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
namespace SportsBookingSystem.Application.Queries.Fields.GetFieldById;

public class GetFieldByIdHandler : IQueryHandler<GetFieldByIdQuery,ErrorOr<FieldDto>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public GetFieldByIdHandler(IApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<ErrorOr<FieldDto>> HandleAsync(GetFieldByIdQuery query, CancellationToken ct = default)
    {
        var field = await _dbContext.Fields
            .Where(f => f.Id == query.FieldId)
            .Select(f => new FieldDto(
                f.Id,
                f.Name,
                f.SportType.ToString(),
                f.BaseHourlyPrice,
                f.ParkId,
                f.Park.Name))
            .FirstOrDefaultAsync(ct);
        
        if(field is null)
            return Error.NotFound("Field.NotFound", $"Field with ID {query.FieldId} was not found.");

        return field;

    }
}