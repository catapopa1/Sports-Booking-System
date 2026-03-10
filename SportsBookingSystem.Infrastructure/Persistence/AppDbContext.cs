using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Entities;
namespace SportsBookingSystem.Infrastructure.Persistence;

public class AppDbContext: DbContext,IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }


    public DbSet<User> Users => Set<User>();
    public DbSet<Park> Parks => Set<Park>();
    public DbSet<Field> Fields => Set<Field>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingInvite> BookingInvites => Set<BookingInvite>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
}