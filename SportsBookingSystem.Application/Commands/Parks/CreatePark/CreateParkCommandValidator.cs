namespace SportsBookingSystem.Application.Commands.Parks.CreatePark;

using FluentValidation;

public class CreateParkCommandValidator : AbstractValidator<CreateParkCommand>
{
    public CreateParkCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Park name is required.")
            .MaximumLength(200).WithMessage("Park name must not exceed 200 characters.");

        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.")
            .MaximumLength(256).WithMessage("Address must not exceed 256 characters.");

        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.ManagerId).GreaterThan(0).WithMessage("A valid manager must be assigned.");
    }
}