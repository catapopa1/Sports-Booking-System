using ErrorOr;
using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Users.UpdateProfile;

public record UpdateProfileCommand(string? bio) 
    : ICommand<ErrorOr<Updated>>;