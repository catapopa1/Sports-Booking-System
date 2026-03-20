using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Parks.DeletePark;

public record DeleteParkCommand(int ParkId)
    : ICommand<ErrorOr<Deleted>>;