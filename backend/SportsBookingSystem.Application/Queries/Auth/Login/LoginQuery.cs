using ErrorOr;
using SportsBookingSystem.Application.Common;

namespace SportsBookingSystem.Application.Queries.Auth.Login;

public record  LoginQuery(string Email, string Password) : IQuery<ErrorOr<LoginResult>>;