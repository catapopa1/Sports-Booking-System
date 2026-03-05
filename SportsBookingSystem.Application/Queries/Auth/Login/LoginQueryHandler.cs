using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Queries.Auth.Login;

public class LoginQueryHandler : IQueryHandler<LoginQuery,ErrorOr<LoginResult>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;


    public LoginQueryHandler(IApplicationDbContext dbContext, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<LoginResult>> HandleAsync(LoginQuery query, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == query.Email, ct);

        if (user is null || !_passwordHasher.Verify(query.Password, user.PasswordHash))
            return Error.Unauthorized("Auth.InvalidCredentials", "Email or password is incorrect");

        var token = _tokenService.GenerateToken(user);

        return new LoginResult(user.Id, user.Email, user.Role.ToString(), token);
    }
}