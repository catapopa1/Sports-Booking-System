using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Outbox;

namespace SportsBookingSystem.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Park> Parks { get; }
    DbSet<Field> Fields { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<BookingInvite> BookingInvites { get; }
    DbSet<Notification> Notifications { get; }
    
    DbSet<Friendship>  Friendships { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);

}