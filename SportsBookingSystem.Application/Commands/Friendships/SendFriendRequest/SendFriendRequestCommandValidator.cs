using FluentValidation;
using ErrorOr;
namespace SportsBookingSystem.Application.Commands.Friendships.SendFriendRequest;

public class SendFriendRequestCommandValidator : AbstractValidator<SendFriendRequestCommand>
{
    public SendFriendRequestCommandValidator()
    {
        RuleFor(x => x.AddresseeId).GreaterThan(0).WithMessage("Addressee id must be valid");
    }
}