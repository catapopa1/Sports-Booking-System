namespace SportsBookingSystem.Application.Queries.Dtos;

public record NotificationDto(
    int Id,
    string Title,
    string Message,
    bool IsRead,
    DateTimeOffset CreatedAt);