using System.Diagnostics.Contracts;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Outbox;

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
    public DbSet<OutboxMessage>  OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now =  DateTimeOffset.UtcNow;
        
        var trackedEntities = ChangeTracker.Entries<BaseEntity>().ToList();
        List<IDomainEvent> domainEvents = new List<IDomainEvent>();

        foreach (var entry in trackedEntities)
        {
            domainEvents.AddRange(entry.Entity.DomainEvents.ToList());
            
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
            }
        }

        foreach (var domainEvent in domainEvents)
        {
            OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }
        
        foreach (var entity in trackedEntities)
            entity.Entity.ClearDomainEvents();
        
        return await base.SaveChangesAsync(ct);
    }
}