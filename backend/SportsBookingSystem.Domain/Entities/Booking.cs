using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Entities;

public class Booking : BaseEntity
{
    public int FieldId { get; set; }
    public Field Field { get; set; } = null!;
    public int OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;
    public DateTimeOffset StartTime { get; set; }
    public BookingType BookingType { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int RequiredPlayerCount { get; set; }

    public ICollection<BookingInvite> Invites { get; set; } = [];
}
