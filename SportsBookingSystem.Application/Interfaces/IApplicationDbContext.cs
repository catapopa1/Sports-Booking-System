using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Domain.Entities;


namespace SportsBookingSystem.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Park> Parks { get; }
    DbSet<Field> Fields { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<BookingInvite> BookingInvites { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

}