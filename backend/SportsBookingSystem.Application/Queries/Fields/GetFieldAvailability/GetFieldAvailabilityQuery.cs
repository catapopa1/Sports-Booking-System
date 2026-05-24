using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Queries.Fields.GetFieldAvailability;

public record GetFieldAvailabilityQuery(int FieldId, DateTimeOffset From, DateTimeOffset To)
    : IQuery<ErrorOr<List<FieldOccupancySlotDto>>>;

public record FieldOccupancySlotDto(DateTimeOffset StartTime, string BookingType);
