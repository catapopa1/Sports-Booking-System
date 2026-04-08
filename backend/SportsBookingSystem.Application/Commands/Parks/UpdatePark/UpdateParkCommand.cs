using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Parks.UpdatePark;
public record UpdateParkCommand(int ParkId, string Name, string Address, string City) 
    : ICommand<ErrorOr<Updated>>;