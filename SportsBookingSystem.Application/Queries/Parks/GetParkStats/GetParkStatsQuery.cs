using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
namespace SportsBookingSystem.Application.Queries.Parks.GetParkStats;

public record GetParkStatsQuery(int ParkId) : IQuery<ErrorOr<ParkStatsDto>>;