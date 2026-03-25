using SportsBookingSystem.Application.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;

namespace SportsBookingSystem.Application.Commands.Users.ChangePassword;

public class ChangePasswordHandler : ICommandHandler<ChangePasswordCommand,ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<ChangePasswordCommand> _validator;

    public ChangePasswordHandler(IApplicationDbContext dbContext, 
        ICurrentUserService currentUser, 
        IPasswordHasher passwordHasher,
        IValidator<ChangePasswordCommand> validator)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<ErrorOr<Updated>> HandleAsync(ChangePasswordCommand command, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            return validation.Errors
                .Select(er => Error.Validation(er.PropertyName, er.ErrorMessage))
                .ToList();
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return Error.NotFound("User.NotFound", "The specified user cannot be found");

        if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            return Error.Validation("Auth.WrongPassword", "Current Password is incorrect");
        
        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);

        await _dbContext.SaveChangesAsync(ct);

        return Result.Updated;

    }
}