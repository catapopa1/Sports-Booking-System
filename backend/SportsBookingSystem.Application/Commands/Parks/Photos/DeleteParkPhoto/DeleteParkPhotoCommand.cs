using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Parks.Photos.DeleteParkPhoto;

public record DeleteParkPhotoCommand(int ParkId, int PhotoId)
    : ICommand<ErrorOr<Deleted>>;
