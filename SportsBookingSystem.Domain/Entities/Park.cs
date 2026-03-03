using SportsBookingSystem.Domain.Common;

namespace SportsBookingSystem.Domain.Entities;

public class Park : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public User Manager { get; set; } = null!;

    public ICollection<Field> Fields { get; set; } = [];
}
