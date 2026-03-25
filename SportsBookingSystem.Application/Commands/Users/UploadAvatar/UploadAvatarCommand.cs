using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Users.UploadAvatar;

public record UploadAvatarCommand(Stream fileStream, string fileName)
    : ICommand<ErrorOr<string>>;