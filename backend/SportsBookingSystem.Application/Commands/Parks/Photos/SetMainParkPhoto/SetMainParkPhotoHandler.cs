using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Parks.Photos.SetMainParkPhoto;

public class SetMainParkPhotoHandler : ICommandHandler<SetMainParkPhotoCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public SetMainParkPhotoHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(SetMainParkPhotoCommand command, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == command.ParkId, ct);

        if (park is null)
            return Error.NotFound("Park.NotFound", $"Park with id {command.ParkId} was not found.");

        if (_currentUser.Role != "Admin" && park.ManagerId != _currentUser.UserId)
            return Error.Forbidden("Park.Forbidden", "You are not allowed to manage photos for this park.");

        var target = park.Photos.FirstOrDefault(p => p.Id == command.PhotoId);
        if (target is null)
            return Error.NotFound("ParkPhoto.NotFound", $"Photo with id {command.PhotoId} was not found.");

        if (target.IsMain)
            return Result.Updated;

        // The filtered unique index allows only one row with IsMain=1 per park.
        // Save the unset-all step first so the new main doesn't conflict with an existing one.
        foreach (var photo in park.Photos)
            photo.IsMain = false;
        await _dbContext.SaveChangesAsync(ct);

        target.IsMain = true;
        await _dbContext.SaveChangesAsync(ct);

        return Result.Updated;
    }
}
