namespace SportsBookingSystem.Application.Queries.Fields.GetFieldById;

public record FieldDto(
    int Id, string Name, string SportType, decimal BaseHourlyPrice, int ParkId, string ParkName
);