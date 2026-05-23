using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsBookingSystem.Application.Commands.Auth.Register;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Auth.GoogleSignIn;
using SportsBookingSystem.Application.Queries.Auth.Login;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : BaseController
{
    private readonly ICommandHandler<RegisterCommand, ErrorOr<int>> _register;
    private readonly IQueryHandler<LoginQuery, ErrorOr<LoginResult>> _login;
    private readonly IQueryHandler<GoogleSignInQuery, ErrorOr<LoginResult>> _googleSignIn;

    public AuthController(
        ICommandHandler<RegisterCommand, ErrorOr<int>> register,
        IQueryHandler<LoginQuery, ErrorOr<LoginResult>> login,
        IQueryHandler<GoogleSignInQuery, ErrorOr<LoginResult>> googleSignIn)
    {
        _register = register;
        _login = login;
        _googleSignIn = googleSignIn;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
    {
        var result = await _register.HandleAsync(command, ct);

        return result.Match(
            id => CreatedAtAction(nameof(Register), new { id }, new { id }),
            Problem
        );
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery query, CancellationToken ct)
    {
        var result = await _login.HandleAsync(query, ct);

        return result.Match(
            loginResult => Ok(loginResult),
            Problem
        );
    }

    public record GoogleSignInRequest(string IdToken);

    [HttpPost("google")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request, CancellationToken ct)
    {
        var result = await _googleSignIn.HandleAsync(new GoogleSignInQuery(request.IdToken), ct);
        return result.Match(loginResult => Ok(loginResult), Problem);
    }
}