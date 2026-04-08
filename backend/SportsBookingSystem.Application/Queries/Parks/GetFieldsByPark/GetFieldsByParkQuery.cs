using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
using ErrorOr;
namespace SportsBookingSystem.Application.Queries.Parks.GetFieldsByPark;

public record GetFieldsByParkQuery(int ParkId) : IQuery<ErrorOr<List<FieldDto>>>;