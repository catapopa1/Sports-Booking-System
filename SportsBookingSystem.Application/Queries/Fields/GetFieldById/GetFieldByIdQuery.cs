using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Queries.Fields.GetFieldById;

public record GetFieldByIdQuery(int FieldId) : IQuery<ErrorOr<FieldDto>>;