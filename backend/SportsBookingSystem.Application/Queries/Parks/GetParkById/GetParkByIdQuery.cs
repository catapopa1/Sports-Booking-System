using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Queries.Parks.GetParkById;


public record GetParkByIdQuery(int ParkId) : IQuery<ErrorOr<ParkDto>>;