namespace SportsBookingSystem.Application.Queries.Parks.GetParkById;

public record ParkDto(
    int Id, string Name, string Address, string City, int ManagerId, string ManagerName
);