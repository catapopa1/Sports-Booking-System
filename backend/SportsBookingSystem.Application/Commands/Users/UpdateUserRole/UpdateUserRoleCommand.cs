using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Users.UpdateUserRole;
public record UpdateUserRoleCommand(int UserId,UserRole NewRole) 
    : ICommand<ErrorOr<Updated>>;