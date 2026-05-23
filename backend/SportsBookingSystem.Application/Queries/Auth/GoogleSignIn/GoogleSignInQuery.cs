using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Auth.Login;

namespace SportsBookingSystem.Application.Queries.Auth.GoogleSignIn;

public record GoogleSignInQuery(string IdToken) : IQuery<ErrorOr<LoginResult>>;
