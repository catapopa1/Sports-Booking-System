namespace SportsBookingSystem.Application.Queries.Parks.GetAllParks;

public record ParkSummaryDto(
    int Id, string Name, string City, int FieldCount
);