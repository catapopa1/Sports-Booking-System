using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Auth.Login;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Queries.Auth.GoogleSignIn;

public class GoogleSignInHandler : IQueryHandler<GoogleSignInQuery, ErrorOr<LoginResult>>
{
    private const string Provider = "Google";

    private readonly IApplicationDbContext _dbContext;
    private readonly IGoogleTokenVerifier _verifier;
    private readonly ITokenService _tokenService;

    public GoogleSignInHandler(
        IApplicationDbContext dbContext,
        IGoogleTokenVerifier verifier,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _verifier = verifier;
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<LoginResult>> HandleAsync(GoogleSignInQuery query, CancellationToken ct = default)
    {
        var payload = await _verifier.VerifyAsync(query.IdToken, ct);
        if (payload is null)
            return Error.Unauthorized("Auth.InvalidGoogleToken", "The Google sign-in token is invalid or expired.");

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u =>
                u.ExternalProvider == Provider && u.ExternalId == payload.Subject, ct);

        if (user is null)
        {
            user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == payload.Email, ct);

            if (user is null)
            {
                user = new User
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    PasswordHash = null,
                    Role = UserRole.Player,
                    ProfilePictureUrl = payload.Picture,
                    ExternalProvider = Provider,
                    ExternalId = payload.Subject,
                };
                await _dbContext.Users.AddAsync(user, ct);
            }
            else
            {
                user.ExternalProvider = Provider;
                user.ExternalId = payload.Subject;
                if (string.IsNullOrEmpty(user.ProfilePictureUrl))
                    user.ProfilePictureUrl = payload.Picture;
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        var token = _tokenService.GenerateToken(user);
        return new LoginResult(user.Id, user.Email, user.Role.ToString(), token);
    }
}
