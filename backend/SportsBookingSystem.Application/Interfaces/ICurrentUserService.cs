namespace SportsBookingSystem.Application.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    string? Role { get; }
}