namespace SportsBookingSystem.Application.Queries.Dtos;

public record ParkStatsDto(
    int ParkId,
    string ParkName,
    int TotalConfirmedBookings,
    int TotalUniquePlayers,
    string? MostPlayedSport,
    int? BusiestHour,
    string? BusiestWeekday,
    List<TopOrganizerDto> TopOrganizers);

public record TopOrganizerDto(
    int UserId,
    string FullName,
    int BookingCount);