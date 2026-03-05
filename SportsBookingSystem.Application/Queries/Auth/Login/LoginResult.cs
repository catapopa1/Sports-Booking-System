namespace SportsBookingSystem.Application.Queries.Auth.Login;

public record LoginResult(
    int UserId,
    string Email,
    string Role,
    string Token
);