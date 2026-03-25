using SportsBookingSystem.Application.Common;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Users.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword,string NewPassword) 
    : ICommand<ErrorOr<Updated>>;