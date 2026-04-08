using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Entities;

public class Field : BaseEntity,ISoftDeletable
{
    public int ParkId { get; set; }
    public Park Park { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public SportType SportType { get; set; }
    public decimal BaseHourlyPrice { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
    
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
