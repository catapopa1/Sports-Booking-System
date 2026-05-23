using FluentValidation;

namespace SportsBookingSystem.Application.Commands.Users.UpsertSportProfile;

public class UpsertSportProfileValidator : AbstractValidator<UpsertSportProfileCommand>
{
    public UpsertSportProfileValidator()
    {
        RuleFor(x => x.FavoriteAthlete)
            .MaximumLength(100)
            .WithMessage("Favorite athlete name must be at most 100 characters.");
    }
}
