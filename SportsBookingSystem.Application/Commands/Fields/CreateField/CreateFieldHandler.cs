using SportsBookingSystem.Application.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Application.Commands.Fields.CreateField;

public class CreateFieldHandler : ICommandHandler<CreateFieldCommand,ErrorOr<int>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IValidator<CreateFieldCommand> _validator;

    public CreateFieldHandler(IApplicationDbContext dbContext, IValidator<CreateFieldCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<ErrorOr<int>> HandleAsync(CreateFieldCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command,ct);

        if (!validation.IsValid)
            return validation.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        var parkExists = await _dbContext.Parks.AnyAsync(p => p.Id == command.ParkId, ct);
        if (!parkExists)
            return Error.NotFound("Park.NotFound", $"Park with ID {command.ParkId} was not found.");

        var field = new Field
        {
            ParkId = command.ParkId,
            Name = command.Name,
            SportType = command.SportType,
            BaseHourlyPrice = command.BaseHourlyPrice
        };
        
        _dbContext.Fields.Add(field);
        await _dbContext.SaveChangesAsync(ct);
        
        return field.Id;
    }
    
}