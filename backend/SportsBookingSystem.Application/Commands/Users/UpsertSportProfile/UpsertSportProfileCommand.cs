using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Users.UpsertSportProfile;

public record UpsertSportProfileCommand(
    SportType Sport,
    SportLevel? Level,
    string? FavoriteAthlete
) : ICommand<ErrorOr<Updated>>;
