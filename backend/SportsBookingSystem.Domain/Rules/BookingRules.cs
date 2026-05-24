using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Rules;

public static class BookingRules
{
    public static (int MinInvites, int MaxInvites) InviteRange(SportType sport, BookingType bookingType) =>
        (sport, bookingType) switch
        {
            (SportType.Football,   _)                     => (5, 11),
            (SportType.Tennis,     _)                     => (1, 3),
            (SportType.Basketball, BookingType.FullCourt) => (3, 9),
            (SportType.Basketball, BookingType.HalfCourt) => (1, 5),
            _                                             => (0, 0)
        };

    public static int MinRequiredPlayers(SportType sport, BookingType bookingType) =>
        InviteRange(sport, bookingType).MinInvites + 1;
}
