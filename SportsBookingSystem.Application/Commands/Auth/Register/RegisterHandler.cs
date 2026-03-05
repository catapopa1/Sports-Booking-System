using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Auth.Register;

public class RegisterHandler : ICommandHandler<RegisterCommand,ErrorOr<int>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterCommand> _validator;


    public RegisterHandler(IApplicationDbContext dbContext, IPasswordHasher passwordHasher, IValidator<RegisterCommand> validator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<ErrorOr<int>> HandleAsync(RegisterCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);

        if (!validation.IsValid)
        {
            return validation.Errors
                .Select(er => Error.Validation(er.PropertyName, er.ErrorMessage))
                .ToList();
        }

        var emailValidation = await _dbContext.Users.AnyAsync(e => e.Email == command.Email, ct);
        if (emailValidation)
            return Error.Conflict("User.EmailTaken", "A user with this email already exists");

        var user = new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = _passwordHasher.Hash(command.Password),
            Role = UserRole.Player
        };
        
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        return user.Id; 
    }
}