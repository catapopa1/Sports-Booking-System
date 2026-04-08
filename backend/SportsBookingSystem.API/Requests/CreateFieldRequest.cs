using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.API.Requests;

public record CreateFieldRequest(
    string Name, SportType SportType, decimal BaseHourlyPrice
);
