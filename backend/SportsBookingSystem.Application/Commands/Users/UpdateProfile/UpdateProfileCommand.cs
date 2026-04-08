using ErrorOr;
using SportsBookingSystem.Application.Common;
namespace SportsBookingSystem.Application.Commands.Users.UpdateProfile;

public record UpdateProfileCommand(string? bio) 
    : ICommand<ErrorOr<Updated>>;