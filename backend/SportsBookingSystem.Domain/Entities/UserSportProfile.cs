using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Entities;

public class UserSportProfile : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public SportType SportType { get; set; }

    public SportLevel? Level { get; set; }
    public string? FavoriteAthlete { get; set; }
}
