using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Fields.DeleteField;

public record DeleteFieldCommand(int FieldId)
    : ICommand<ErrorOr<Deleted>>;