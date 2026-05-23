using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Application.Commands.Parks.Photos.UploadParkPhoto;

public class UploadParkPhotoHandler : ICommandHandler<UploadParkPhotoCommand, ErrorOr<ParkPhotoDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadParkPhotoHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<ParkPhotoDto>> HandleAsync(UploadParkPhotoCommand command, CancellationToken ct = default)
    {
        var park = await _dbContext.Parks
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == command.ParkId, ct);

        if (park is null)
            return Error.NotFound("Park.NotFound", $"Park with id {command.ParkId} was not found.");

        if (_currentUser.Role != "Admin" && park.ManagerId != _currentUser.UserId)
            return Error.Forbidden("Park.Forbidden", "You are not allowed to manage photos for this park.");

        var url = await _fileStorage.SaveAsync(command.FileStream, command.FileName, ct);

        var photo = new ParkPhoto
        {
            ParkId = park.Id,
            Url = url,
            IsMain = park.Photos.Count == 0,
            OrderIndex = park.Photos.Count
        };

        await _dbContext.ParkPhotos.AddAsync(photo, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new ParkPhotoDto(photo.Id, photo.Url, photo.IsMain, photo.OrderIndex);
    }
}
