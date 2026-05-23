using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Parks.Photos.DeleteParkPhoto;

public class DeleteParkPhotoHandler : ICommandHandler<DeleteParkPhotoCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public DeleteParkPhotoHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Deleted>> HandleAsync(DeleteParkPhotoCommand command, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == command.ParkId, ct);

        if (park is null)
            return Error.NotFound("Park.NotFound", $"Park with id {command.ParkId} was not found.");

        if (_currentUser.Role != "Admin" && park.ManagerId != _currentUser.UserId)
            return Error.Forbidden("Park.Forbidden", "You are not allowed to manage photos for this park.");

        var photo = park.Photos.FirstOrDefault(p => p.Id == command.PhotoId);
        if (photo is null)
            return Error.NotFound("ParkPhoto.NotFound", $"Photo with id {command.PhotoId} was not found.");

        var wasMain = photo.IsMain;
        _dbContext.ParkPhotos.Remove(photo);

        if (wasMain)
        {
            var promote = park.Photos
                .Where(p => p.Id != photo.Id)
                .OrderBy(p => p.OrderIndex)
                .FirstOrDefault();
            if (promote is not null)
                promote.IsMain = true;
        }

        await _dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}
