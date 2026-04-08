using FluentValidation;

namespace SportsBookingSystem.Application.Commands.Fields.CreateField;

public class CreateFieldCommandValidator : AbstractValidator<CreateFieldCommand>
{
    public CreateFieldCommandValidator()
    {
        RuleFor(x => x.ParkId).GreaterThan(0).WithMessage("A valid park must be specified.");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Field name is required.")
            .MaximumLength(200).WithMessage("Field name must not exceed 200 characters.");

        RuleFor(x => x.BaseHourlyPrice).GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
