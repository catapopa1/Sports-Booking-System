using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Settings;

namespace SportsBookingSystem.Infrastructure.Services;

public class GoogleTokenVerifier : IGoogleTokenVerifier
{
    private readonly GoogleAuthSettings _settings;

    public GoogleTokenVerifier(IOptions<GoogleAuthSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<GoogleTokenPayload?> VerifyAsync(string idToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            return null;

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _settings.ClientId },
                });

            return new GoogleTokenPayload(
                payload.Subject,
                payload.Email,
                payload.GivenName ?? string.Empty,
                payload.FamilyName ?? string.Empty,
                payload.Picture);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
