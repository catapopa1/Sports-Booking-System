namespace SportsBookingSystem.Application.Queries.Dtos;

public record UserSearchResultDto(int UserId, string FullName, string? ProfilePictureUrl, bool AlreadyFriends);