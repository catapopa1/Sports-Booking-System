using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
namespace SportsBookingSystem.Application.Queries.Users.GetUserProfile;

public record GetUserProfileQuery (int UserId) : IQuery<ErrorOr<UserProfileDto>>;