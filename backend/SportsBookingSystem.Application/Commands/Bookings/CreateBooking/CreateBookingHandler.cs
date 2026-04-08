using SportsBookingSystem.Application.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Application.Commands.Bookings.CreateBooking;

public class CreateBookingHandler : ICommandHandler<CreateBookingCommand, ErrorOr<int>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateBookingCommand> _validator;

    public CreateBookingHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser, IValidator<CreateBookingCommand> validator)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<ErrorOr<int>> HandleAsync(CreateBookingCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        var organizerId = _currentUser.UserId;

        var field = await _dbContext.Fields
            .Include(f => f.Park)
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == command.FieldId, ct);

        if (field is null)
            return Error.NotFound("Field.NotFound", $"Field with ID {command.FieldId} was not found.");

        if ((command.BookingType == BookingType.Standard && field.SportType == SportType.Basketball)
            || (command.BookingType != BookingType.Standard && field.SportType != SportType.Basketball))
            return Error.Conflict("Field.Conflict",
                $"Booking type {command.BookingType} does not match field sport type {field.SportType}.");

        if ((field.SportType == SportType.Football && command.InvitedPlayersIds.Count != 11) ||
            (field.SportType == SportType.Basketball && command.BookingType == BookingType.FullCourt &&
             command.InvitedPlayersIds.Count != 9) ||
            (field.SportType == SportType.Basketball && command.BookingType == BookingType.HalfCourt &&
             command.InvitedPlayersIds.Count != 5) ||
            (field.SportType == SportType.Tennis &&
             (command.InvitedPlayersIds.Count != 1 && command.InvitedPlayersIds.Count != 3)))
            return Error.Validation("Booking.InvalidPlayerCount", "Number of invited players does not match the booking type.");

        var requiredPlayerCount = command.InvitedPlayersIds.Count + 1;

        var friendsCount = await _dbContext.Friendships.CountAsync(f =>
            f.Status == FriendshipStatus.Accepted &&
            ((f.RequesterId == organizerId && command.InvitedPlayersIds.Contains(f.AddresseeId)) ||
             (f.AddresseeId == organizerId && command.InvitedPlayersIds.Contains(f.RequesterId))), ct);

        if (friendsCount != command.InvitedPlayersIds.Count)
            return Error.Validation("Booking.NotFriends", "All invited players must be accepted friends.");

        if (field.SportType == SportType.Basketball)
        {
            var existingFull = await _dbContext.Bookings.CountAsync(b =>
                b.FieldId == command.FieldId &&
                b.StartTime == command.StartDate &&
                b.BookingType == BookingType.FullCourt &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.TimedOut, ct);

            var existingHalf = await _dbContext.Bookings.CountAsync(b =>
                b.FieldId == command.FieldId &&
                b.StartTime == command.StartDate &&
                b.BookingType == BookingType.HalfCourt &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.TimedOut, ct);

            if (existingFull >= 1)
                return Error.Conflict("Booking.SlotTaken", $"Field '{field.Name}' already has a full-court booking at {command.StartDate}.");

            if (command.BookingType == BookingType.FullCourt && existingHalf >= 1)
                return Error.Conflict("Booking.SlotTaken", $"Field '{field.Name}' already has a half-court booking at {command.StartDate}.");

            if (existingHalf >= 2)
                return Error.Conflict("Booking.SlotTaken", $"Field '{field.Name}' already has two half-court bookings at {command.StartDate}.");
        }
        else
        {
            var existingBooking = await _dbContext.Bookings.AnyAsync(b =>
                b.FieldId == command.FieldId &&
                b.StartTime == command.StartDate &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.TimedOut, ct);

            if (existingBooking)
                return Error.Conflict("Booking.SlotTaken", $"Field '{field.Name}' is already booked at {command.StartDate}.");
        }

        var totalPrice = command.BookingType == BookingType.HalfCourt
            ? field.BaseHourlyPrice * 0.75m
            : field.BaseHourlyPrice;

        var booking = new Booking
        {
            FieldId = command.FieldId,
            OrganizerId = organizerId,
            StartTime = command.StartDate,
            BookingType = command.BookingType,
            Status = BookingStatus.Requested,
            TotalPrice = totalPrice,
            RequiredPlayerCount = requiredPlayerCount,
            Invites = command.InvitedPlayersIds
                .Select(playerId => new BookingInvite { PlayerId = playerId, Status = InviteStatus.Pending })
                .ToList()
        };

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync(ct);

        return booking.Id;
    }
}