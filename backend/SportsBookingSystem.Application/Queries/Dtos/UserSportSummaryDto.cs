using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Dtos;

public record UserSportSummaryDto(
    SportType Sport,
    SportLevel? Level,
    string? FavoriteAthlete,
    int MatchesPlayed);
