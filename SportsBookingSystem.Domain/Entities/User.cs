using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public ICollection<Booking> OrganizedBookings { get; set; } = [];
    public ICollection<BookingInvite> Invites { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}
