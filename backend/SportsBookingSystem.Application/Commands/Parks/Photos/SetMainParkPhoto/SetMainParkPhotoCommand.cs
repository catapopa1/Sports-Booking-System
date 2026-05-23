using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Parks.Photos.SetMainParkPhoto;

public record SetMainParkPhotoCommand(int ParkId, int PhotoId)
    : ICommand<ErrorOr<Updated>>;
