using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Commands.Bookings.CancelBooking;
using SportsBookingSystem.Application.Commands.Bookings.CreateBooking;
using SportsBookingSystem.Application.Commands.Bookings.RespondToInvite;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Bookings.GetBookingById;
using SportsBookingSystem.Application.Queries.Bookings.GetMyBookings;
using SportsBookingSystem.Application.Queries.Bookings.GetMyInvites;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class BookingsController : BaseController
{
    private readonly ICommandHandler<CreateBookingCommand, ErrorOr<int>> _createBookingHandler;
    private readonly ICommandHandler<RespondToInviteCommand, ErrorOr<Updated>> _respondToInviteHandler;
    private readonly ICommandHandler<CancelBookingCommand, ErrorOr<Updated>> _cancelBookingHandler;
    private readonly IQueryHandler<GetBookingByIdQuery, ErrorOr<BookingDto>> _getBookingByIdHandler;
    private readonly IQueryHandler<GetMyBookingsQuery, ErrorOr<PagedResult<BookingSummaryDto>>> _getMyBookingsHandler;
    private readonly IQueryHandler<GetMyInvitesQuery, ErrorOr<List<InviteNotificationDto>>> _getMyInvitesHandler;

    public BookingsController(
        ICommandHandler<CreateBookingCommand, ErrorOr<int>> createBookingHandler,
        ICommandHandler<RespondToInviteCommand, ErrorOr<Updated>> respondToInviteHandler,
        ICommandHandler<CancelBookingCommand, ErrorOr<Updated>> cancelBookingHandler,
        IQueryHandler<GetBookingByIdQuery, ErrorOr<BookingDto>> getBookingByIdHandler,
        IQueryHandler<GetMyBookingsQuery, ErrorOr<PagedResult<BookingSummaryDto>>> getMyBookingsHandler,
        IQueryHandler<GetMyInvitesQuery, ErrorOr<List<InviteNotificationDto>>> getMyInvitesHandler)
    {
        _createBookingHandler = createBookingHandler;
        _respondToInviteHandler = respondToInviteHandler;
        _cancelBookingHandler = cancelBookingHandler;
        _getBookingByIdHandler = getBookingByIdHandler;
        _getMyBookingsHandler = getMyBookingsHandler;
        _getMyInvitesHandler = getMyInvitesHandler;
    }

    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command, CancellationToken ct = default)
    {
        var result = await _createBookingHandler.HandleAsync(command, ct);
        return result.Match(id => Created(string.Empty, new { id }), Problem);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetMyBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _getMyBookingsHandler.HandleAsync(new GetMyBookingsQuery(page,pageSize), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("invites")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetMyInvites(CancellationToken ct = default)
    {
        var result = await _getMyInvitesHandler.HandleAsync(new GetMyInvitesQuery(), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id, CancellationToken ct = default)
    {
        var result = await _getBookingByIdHandler.HandleAsync(new GetBookingByIdQuery(id), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPut("{id}/respond")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> RespondToInvite(int id, [FromBody] InviteStatus response, CancellationToken ct = default)
    {
        var result = await _respondToInviteHandler.HandleAsync(new RespondToInviteCommand(id, response), ct);
        return result.Match(_ => Ok(), Problem);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> CancelBooking(int id, CancellationToken ct = default)
    {
        var result = await _cancelBookingHandler.HandleAsync(new CancelBookingCommand(id), ct);
        return result.Match(_ => NoContent(), Problem);
    }
}