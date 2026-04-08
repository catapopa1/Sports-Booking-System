using FluentValidation;

namespace SportsBookingSystem.Application.Commands.Bookings.CreateBooking;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(c => c.FieldId).GreaterThan(0);
        RuleFor(c => c.InvitedPlayersIds).NotEmpty();
        RuleFor(c => c.StartDate).GreaterThan(DateTimeOffset.UtcNow);
    }
}