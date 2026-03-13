namespace SportsBookingSystem.Application.Queries.Dtos;

public record InviteDto(int PlayerId, string PlayerName, string Status);

public record BookingDto(
    int Id,
    int FieldId,
    string FieldName,
    string ParkName,
    DateTimeOffset StartTime,
    string BookingType,
    string Status,
    decimal TotalPrice,
    int RequiredPlayerCount,
    List<InviteDto> Invites);

public record BookingSummaryDto(
    int Id,
    string FieldName,
    string ParkName,
    DateTimeOffset StartTime,
    string Status,
    decimal TotalPrice);

public record InviteNotificationDto(
    int BookingId,
    string OrganizerName,
    string FieldName,
    string ParkName,
    DateTimeOffset StartTime,
    string InviteStatus);