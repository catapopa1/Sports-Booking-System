using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Entities;

public class BookingInvite : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int PlayerId { get; set; }
    public User Player { get; set; } = null!;
    public InviteStatus Status { get; set; }
}
