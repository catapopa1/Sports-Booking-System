using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Fields.UpdateField;

public record UpdateFieldCommand(int FieldId, string Name, decimal BaseHourlyPrice)
    : ICommand<ErrorOr<Updated>>;