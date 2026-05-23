namespace SportsBookingSystem.Application.Interfaces;

public record GoogleTokenPayload(
    string Subject,
    string Email,
    string GivenName,
    string FamilyName,
    string? Picture);

public interface IGoogleTokenVerifier
{
    Task<GoogleTokenPayload?> VerifyAsync(string idToken, CancellationToken ct = default);
}
