namespace SportsBookingSystem.Application.Queries.Dtos;

public record FriendRequestDto(int FriendshipId, int UserId, string FullName, string? ProfilePictureUrl, DateTimeOffset CreatedAt);
