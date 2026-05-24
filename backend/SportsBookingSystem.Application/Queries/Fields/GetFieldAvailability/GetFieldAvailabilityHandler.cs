using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Fields.GetFieldAvailability;

public class GetFieldAvailabilityHandler : IQueryHandler<GetFieldAvailabilityQuery, ErrorOr<List<FieldOccupancySlotDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetFieldAvailabilityHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<FieldOccupancySlotDto>>> HandleAsync(GetFieldAvailabilityQuery query, CancellationToken ct = default)
    {
        if (query.To <= query.From)
            return Error.Validation("Field.Availability.InvalidRange", "'to' must be after 'from'.");

        var fieldExists = await _dbContext.Fields.AnyAsync(f => f.Id == query.FieldId, ct);
        if (!fieldExists)
            return Error.NotFound("Field.NotFound", $"Field with ID {query.FieldId} was not found.");

        var slots = await _dbContext.Bookings
            .Where(b => b.FieldId == query.FieldId
                        && b.StartTime >= query.From
                        && b.StartTime < query.To
                        && b.Status != BookingStatus.Cancelled
                        && b.Status != BookingStatus.TimedOut)
            .Select(b => new FieldOccupancySlotDto(b.StartTime, b.BookingType.ToString()))
            .ToListAsync(ct);

        return slots;
    }
}
