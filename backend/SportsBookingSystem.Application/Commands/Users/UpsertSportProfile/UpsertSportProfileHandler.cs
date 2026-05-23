using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Application.Commands.Users.UpsertSportProfile;

public class UpsertSportProfileHandler : ICommandHandler<UpsertSportProfileCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<UpsertSportProfileCommand> _validator;

    public UpsertSportProfileHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IValidator<UpsertSportProfileCommand> validator)
    {
        _context = context;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(UpsertSportProfileCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        var userId = _currentUser.UserId;
        var trimmedAthlete = string.IsNullOrWhiteSpace(command.FavoriteAthlete)
            ? null
            : command.FavoriteAthlete.Trim();
        var clearBoth = command.Level is null && trimmedAthlete is null;

        var existing = await _context.UserSportProfiles
            .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.SportType == command.Sport, ct);

        if (clearBoth)
        {
            if (existing is not null)
            {
                _context.UserSportProfiles.Remove(existing);
                await _context.SaveChangesAsync(ct);
            }
            return Result.Updated;
        }

        if (existing is null)
        {
            await _context.UserSportProfiles.AddAsync(new UserSportProfile
            {
                UserId = userId,
                SportType = command.Sport,
                Level = command.Level,
                FavoriteAthlete = trimmedAthlete,
            }, ct);
        }
        else
        {
            existing.Level = command.Level;
            existing.FavoriteAthlete = trimmedAthlete;
        }

        await _context.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
