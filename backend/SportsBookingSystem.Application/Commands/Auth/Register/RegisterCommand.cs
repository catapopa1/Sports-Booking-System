using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Commands.Auth.Register;

using ErrorOr;

public record RegisterCommand
(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<ErrorOr<int>>;