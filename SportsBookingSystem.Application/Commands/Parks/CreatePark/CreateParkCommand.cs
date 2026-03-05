namespace SportsBookingSystem.Application.Commands.Parks.CreatePark;

using ErrorOr;
using SportsBookingSystem.Application.Common;
public record CreateParkCommand(
    string Name,
    string Address,
    string City,
    int ManagerId
) : ICommand<ErrorOr<int>>;

