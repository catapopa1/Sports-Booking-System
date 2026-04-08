using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Commands.Friendships.RemoveFriend;
using SportsBookingSystem.Application.Commands.Friendships.RespondToFriendRequestCommand;
using SportsBookingSystem.Application.Commands.Friendships.SendFriendRequest;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Friendships.GetMyFriends;
using SportsBookingSystem.Application.Queries.Friendships.GetPendingRequests;
using SportsBookingSystem.Application.Queries.Friendships.GetSentRequests;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.API.Controllers; 

[Route("api/[controller]")]
[Authorize]
public class FriendshipController : BaseController
{
    private readonly ICommandHandler<SendFriendRequestCommand,ErrorOr<int>> _sendFriendRequestHandler;
    private readonly ICommandHandler<RespondtoFriendRequestCommand, ErrorOr<Updated>> _respondFriendRequestHandler;
    private readonly ICommandHandler<RemoveFriendCommand, ErrorOr<Deleted>> _removeFriendHandler;

    private readonly IQueryHandler<GetMyFriendsQuery,ErrorOr<List<FriendDto>>> _getMyFriendsQueryHandler;
    private readonly IQueryHandler<GetPendingRequestsQuery,ErrorOr<List<FriendRequestDto>>> _getPendingRequestsQueryHandler;
    private readonly IQueryHandler<GetSentRequestsQuery,ErrorOr<List<FriendRequestDto>>> _getSentRequestsQueryHandler;

    public FriendshipController(
        ICommandHandler<SendFriendRequestCommand, ErrorOr<int>> sendFriendRequestHandler,
        ICommandHandler<RespondtoFriendRequestCommand, ErrorOr<Updated>> respondFriendRequestHandler,
        ICommandHandler<RemoveFriendCommand, ErrorOr<Deleted>> removeFriendHandler,
        IQueryHandler<GetMyFriendsQuery, ErrorOr<List<FriendDto>>> getMyFriendsQueryHandler,
        IQueryHandler<GetPendingRequestsQuery, ErrorOr<List<FriendRequestDto>>> getPendingRequestsQueryHandler,
        IQueryHandler<GetSentRequestsQuery, ErrorOr<List<FriendRequestDto>>> getSentRequestsQueryHandler)
    {
        _sendFriendRequestHandler = sendFriendRequestHandler;
        _respondFriendRequestHandler = respondFriendRequestHandler;
        _removeFriendHandler = removeFriendHandler;
        _getMyFriendsQueryHandler = getMyFriendsQueryHandler;
        _getPendingRequestsQueryHandler = getPendingRequestsQueryHandler;
        _getSentRequestsQueryHandler = getSentRequestsQueryHandler;
    }

    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> SendFriendRequest(SendFriendRequestCommand command, CancellationToken ct)
    {
        var result = await _sendFriendRequestHandler.HandleAsync(command, ct);
        return result.Match(id => Created(string.Empty, new { id }), Problem);
    }

    [HttpPut("{id}/respond")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> RespondToFriendRequest(int id, [FromBody] FriendshipStatus newStatus, CancellationToken ct)
    {
        var command = new RespondtoFriendRequestCommand(id, newStatus);
        var result = await _respondFriendRequestHandler.HandleAsync(command, ct);
        return result.Match(_ => Ok(), Problem);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> RemoveFriend(int id, CancellationToken ct)
    {
        var command = new RemoveFriendCommand(id);
        var result = await _removeFriendHandler.HandleAsync(command, ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("me")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetMyFriends(CancellationToken ct)
    {
        var result = await _getMyFriendsQueryHandler.HandleAsync(new GetMyFriendsQuery(), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetPendingRequests(CancellationToken ct)
    {
        var result = await _getPendingRequestsQueryHandler.HandleAsync(new GetPendingRequestsQuery(), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("sent")]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetSentRequests(CancellationToken ct)
    {
        var result = await _getSentRequestsQueryHandler.HandleAsync(new GetSentRequestsQuery(), ct);
        return result.Match(Ok, Problem);
    }
}



















