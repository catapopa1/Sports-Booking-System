using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Queries.Users.GetUserProfile;

public class GetUserProfileHandler : IQueryHandler<GetUserProfileQuery,ErrorOr<UserProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserProfileHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<UserProfileDto>> HandleAsync(GetUserProfileQuery query, CancellationToken ct)
    {
        var user = await _context.Users
            .Where(u => u.Id == query.UserId)
            .Select(u => new UserProfileDto(
                u.Id,
                u.FirstName + " " + u.LastName,
                u.Bio,
                u.ProfilePictureUrl,
                u.Role.ToString()))
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found.");

        return user;
    }

}