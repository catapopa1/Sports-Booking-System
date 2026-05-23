using SportsBookingSystem.Domain.Common;

namespace SportsBookingSystem.Domain.Entities;

public class ParkPhoto : BaseEntity
{
    public int ParkId { get; set; }
    public Park Park { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int OrderIndex { get; set; }
}
