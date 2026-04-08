using SportsBookingSystem.Application.Common;
using ErrorOr;


namespace SportsBookingSystem.Application.Queries.Parks.GetAllParks;

public record GetAllParksQuery : IQuery<ErrorOr<List<ParkSummaryDto>>>;