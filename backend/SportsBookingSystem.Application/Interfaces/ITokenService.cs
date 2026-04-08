using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}