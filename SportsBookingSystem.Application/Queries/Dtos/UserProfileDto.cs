namespace SportsBookingSystem.Application.Queries.Dtos;

public record UserProfileDto(
    int UserId,
    string FullName,
    string? Bio,
    string? ProfilePictureUrl,
    string Role);