using SportsBookingSystem.Application.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Users.UploadAvatar;

public class UploadAvatarHandler : ICommandHandler<UploadAvatarCommand,ErrorOr<string>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorageService;

    public UploadAvatarHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser,IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _fileStorageService = fileStorageService;
    }
    

    public async Task<ErrorOr<string>> HandleAsync(UploadAvatarCommand command, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if (user is null)
            return Error.NotFound("User.NotFound", "The specified user does not exist.");
        
        var url = await _fileStorageService.SaveAsync(command.fileStream,command.fileName,ct);

        user.ProfilePictureUrl = url;

        await _dbContext.SaveChangesAsync(ct);

        return url;
    }
}