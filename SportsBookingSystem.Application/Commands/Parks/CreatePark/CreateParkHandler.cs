namespace SportsBookingSystem.Application.Commands.Parks.CreatePark;

using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Enums;


public class CreateParkHandler : ICommandHandler<CreateParkCommand,ErrorOr<int>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IValidator<CreateParkCommand> _validator;

    public CreateParkHandler(IApplicationDbContext dbContext, IValidator<CreateParkCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<ErrorOr<int>> HandleAsync(CreateParkCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);

        if (!validation.IsValid)
            return validation.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        var manager = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == command.ManagerId, ct);
        if (manager is null)
            return Error.NotFound("User.NotFound", "The specified manager does not exist.");
        if (manager.Role != UserRole.ParkManager)
            return Error.Validation("User.InvalidRole", "The specified user is not a ParkManager.");
        
        var park = new Park
        {
            Name =  command.Name,
            Address = command.Address,
            City = command.City,
            ManagerId = command.ManagerId
        };
        
        await _dbContext.Parks.AddAsync(park,ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return park.Id;
    }
}