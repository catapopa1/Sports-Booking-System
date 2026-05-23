using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Users.GetUserProfile;

public class GetUserProfileHandler : IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>>
{
    private static readonly SportType[] AllSports =
        [SportType.Football, SportType.Tennis, SportType.Basketball];

    private readonly IApplicationDbContext _context;

    public GetUserProfileHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<UserProfileDto>> HandleAsync(GetUserProfileQuery query, CancellationToken ct)
    {
        var userId = query.UserId;

        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.Id,
                FullName = u.FirstName + " " + u.LastName,
                u.Bio,
                u.ProfilePictureUrl,
                u.Role,
                SportProfiles = u.SportProfiles
                    .Select(sp => new { sp.SportType, sp.Level, sp.FavoriteAthlete })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found.");

        var matchesBySport = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.OrganizerId == userId
                     || b.Invites.Any(i => i.PlayerId == userId && i.Status == InviteStatus.Accepted))
            .GroupBy(b => b.Field.SportType)
            .Select(g => new { Sport = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Sport, x => x.Count, ct);

        var profilesBySport = user.SportProfiles.ToDictionary(sp => sp.SportType);

        var sports = AllSports.Select(sport =>
        {
            var profile = profilesBySport.GetValueOrDefault(sport);
            return new UserSportSummaryDto(
                sport,
                profile?.Level,
                profile?.FavoriteAthlete,
                matchesBySport.GetValueOrDefault(sport, 0));
        }).ToList();

        return new UserProfileDto(
            user.Id,
            user.FullName,
            user.Bio,
            user.ProfilePictureUrl,
            user.Role.ToString(),
            sports);
    }
}
