using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Domain.Enums;
using ErrorOr;

namespace SportsBookingSystem.Application.Commands.Fields.CreateField;

public record CreateFieldCommand(
    int ParkId,
    string Name,
    SportType SportType,
    decimal BaseHourlyPrice
) : ICommand<ErrorOr<int>>;