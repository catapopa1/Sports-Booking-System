using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Commands.Parks.Photos.UploadParkPhoto;

public record UploadParkPhotoCommand(int ParkId, Stream FileStream, string FileName)
    : ICommand<ErrorOr<ParkPhotoDto>>;
