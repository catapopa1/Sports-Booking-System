using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Parks.GetParkStats;

public class GetParkStatsQueryHandler : IQueryHandler<GetParkStatsQuery,ErrorOr<ParkStatsDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetParkStatsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ParkStatsDto>> HandleAsync(GetParkStatsQuery query, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .Where(p => p.Id == query.ParkId)
            .Select(p => new { p.Id, p.Name })
            .FirstOrDefaultAsync(ct);
        
        if (park is null)
            return Error.NotFound("Park.NotFound", "Park does not exist.");


        var confirmedBookings = await _dbContext.Bookings
            .Where(b => b.Field.ParkId == query.ParkId && b.Status == BookingStatus.Confirmed)
            .Select(b => new
            {
                b.Id,
                b.OrganizerId,
                OrganizerName = b.Organizer.FirstName + " " + b.Organizer.LastName,
                b.StartTime,
                SportType = b.Field.SportType
            })
            .ToListAsync(ct);

        var bookingIds = confirmedBookings.Select(b => b.Id).ToList();

        var invitedPlayersIds = await _dbContext.BookingInvites
            .Where(bi => bookingIds.Contains(bi.BookingId) && bi.Status == InviteStatus.Accepted)
            .Select(bi => bi.PlayerId)
            .ToListAsync(ct);


        if (confirmedBookings.Count == 0)
            return new ParkStatsDto
            (
                park.Id,
                park.Name,
                TotalConfirmedBookings: 0,
                TotalUniquePlayers: 0,
                MostPlayedSport: null,
                BusiestHour: null,
                BusiestWeekday: null,
                TopOrganizers: []
            );

        var totalUniquePlayers = confirmedBookings
            .Select(b => b.OrganizerId)
            .Concat(invitedPlayersIds)
            .Distinct()
            .Count();

        var mostPlayedSport = confirmedBookings
            .GroupBy(b => b.SportType)
            .OrderByDescending(g => g.Count())
            .First()
            .Key
            .ToString();

        var busiestHour = confirmedBookings
            .GroupBy(b => b.StartTime.LocalDateTime.Hour)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;

        var busiestWeekday = confirmedBookings
            .GroupBy(b => b.StartTime.LocalDateTime.DayOfWeek)
            .OrderByDescending(g => g.Count())
            .First()
            .Key
            .ToString();

        var topOrganizers = confirmedBookings
            .GroupBy(b => new { b.OrganizerId, b.OrganizerName })
            .Select(g => new TopOrganizerDto(g.Key.OrganizerId, g.Key.OrganizerName, g.Count()))
            .OrderByDescending(to => to.BookingCount)
            .Take(5)
            .ToList();

        return new ParkStatsDto(
            park.Id,
            park.Name,
            TotalConfirmedBookings: confirmedBookings.Count,
            TotalUniquePlayers: totalUniquePlayers,
            MostPlayedSport: mostPlayedSport,
            BusiestHour: busiestHour,
            BusiestWeekday: busiestWeekday,
            TopOrganizers: topOrganizers
        );
    }
}