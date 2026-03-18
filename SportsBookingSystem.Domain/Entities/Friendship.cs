using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Enums;

namespace SportsBookingSystem.Domain.Entities;

public class Friendship : BaseEntity
{
    public int RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    public int AddresseeId { get; set; }
    public User Addressee { get; set; } = null!;
    
    public FriendshipStatus Status { get; set; }
}