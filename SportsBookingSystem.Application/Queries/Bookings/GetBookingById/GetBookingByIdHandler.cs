using ErrorOr;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Bookings.GetBookingById;

public class GetBookingByIdHandler : IQueryHandler<GetBookingByIdQuery, ErrorOr<BookingDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    public GetBookingByIdHandler(IApplicationDbContext dbContext,ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<BookingDto>> HandleAsync(GetBookingByIdQuery query, CancellationToken ct = default)
    {
        var booking = await _dbContext.Bookings
            .Include(b => b.Field)
            .ThenInclude(f => f.Park)
            .Include(b => b.Invites)
            .ThenInclude(i => i.Player)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == query.BookingId, ct);

        if (booking is null)
            return Error.NotFound("Booking.NotFound", "Booking not found.");
        
        var userId = _currentUser.UserId;
        var isOrganizer   = booking.OrganizerId == userId;
        var isInvitee     = booking.Invites.Any(i => i.PlayerId == userId);
        var isParkManager = booking.Field.Park.ManagerId == userId;

        if (!isOrganizer && !isInvitee && !isParkManager)
            return Error.Forbidden("Booking.Forbidden", "You do not have access to this booking.");

        return new BookingDto(
            booking.Id,
            booking.FieldId,
            booking.Field.Name,
            booking.Field.Park.Name,
            booking.StartTime,
            booking.BookingType.ToString(),
            booking.Status.ToString(),
            booking.TotalPrice,
            booking.RequiredPlayerCount,
            booking.Invites.Select(i => new InviteDto(
                i.PlayerId,
                $"{i.Player.FirstName} {i.Player.LastName}",
                i.Status.ToString())).ToList());
    }
}