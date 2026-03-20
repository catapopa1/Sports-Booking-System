using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Common;
using SportsBookingSystem.Domain.Entities;
using SportsBookingSystem.Domain.Events;

namespace SportsBookingSystem.Infrastructure.Jobs;

public class ProcessOutboxMessagesJob
{
    private readonly IApplicationDbContext _dbContext;
    
    
    public ProcessOutboxMessagesJob(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync()
    {
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(20)
            .ToListAsync();

        foreach (var message in messages)
        {
            try
            {
                IDomainEvent? domainEvent = message.Type switch
                {
                    nameof(AllPlayersAcceptedEvent) => JsonSerializer.Deserialize<AllPlayersAcceptedEvent>(
                        message.Payload),
                    nameof(BookingConfirmedEvent) => JsonSerializer.Deserialize<BookingConfirmedEvent>(message.Payload),
                    nameof(BookingCancelledEvent) => JsonSerializer.Deserialize<BookingCancelledEvent>(message.Payload),
                    nameof(BookingTimedOutEvent)     => JsonSerializer.Deserialize<BookingTimedOutEvent>(message.Payload), 
                    _ => null
                };

                if (domainEvent is null) continue;

                await DispatchAsync(domainEvent);

                message.ProcessedAt = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;
                message.ProcessedAt = DateTimeOffset.UtcNow;
            }
        }
        
        await _dbContext.SaveChangesAsync();
    }


    private async Task DispatchAsync(IDomainEvent domainEvent)
    {
        if (domainEvent is AllPlayersAcceptedEvent allPlayersAccepted)
            await HandleAsync(allPlayersAccepted);
        else if (domainEvent is BookingConfirmedEvent bookingConfirmed)
            await HandleAsync(bookingConfirmed);
        else if (domainEvent is BookingCancelledEvent bookingCancelled)
            await HandleAsync(bookingCancelled);
        else if (domainEvent is BookingTimedOutEvent bookingTimedOut)
            await HandleAsync(bookingTimedOut);
    }


    private async Task HandleAsync(AllPlayersAcceptedEvent e)
    {
        var booking = await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Field)
            .ThenInclude(f => f.Park)
            .FirstOrDefaultAsync(b => b.Id == e.BookingId);

        if (booking is null) return;
        
        var recipientId = booking.Field.Park.ManagerId;

        await _dbContext.Notifications.AddAsync(new Notification
        {
            UserId = recipientId,
            Title = "Booking Awaiting Approval",
            Message = $"Booking #{e.BookingId} for field '{booking.Field.Name}' is awaiting your approval.",
            IsRead = false,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    private async Task HandleAsync(BookingConfirmedEvent e)
    {
        var booking = await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Field)
            .Include(b => b.Invites)
            .FirstOrDefaultAsync(b => b.Id == e.BookingId);

        if (booking is null) return;
        
        var recipientsIds = booking.Invites.Select(i => i.PlayerId).ToList();
        recipientsIds.Add(booking.OrganizerId);

        foreach (var recipientId in recipientsIds)
        {
            await _dbContext.Notifications.AddAsync(new Notification
                {
                    UserId = recipientId,
                    Title = "Booking Confirmed",
                    Message = $"Your booking #{e.BookingId} at '{booking.Field.Name}' has been confirmed!",
                    IsRead = false,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            );
        }
        
    }

    private async Task HandleAsync(BookingCancelledEvent e)
    {
        var booking = await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Field)
            .Include(b => b.Invites)
            .FirstOrDefaultAsync(b => b.Id == e.BookingId);
        
        if (booking is null) return;
        
        var recipientsIds = booking.Invites.Select(i => i.PlayerId).ToList();
        recipientsIds.Add(booking.OrganizerId);

        foreach (var recipientId in recipientsIds)
        {
            await _dbContext.Notifications.AddAsync(new Notification
            {
                UserId = recipientId,
                Title = "Booking Cancelled",
                Message = $"Booking #{e.BookingId} at '{booking.Field.Name}' has been cancelled.",
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            }
            );
        }
    }

    private async Task HandleAsync(BookingTimedOutEvent e)
    {
        var booking = await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Field)
            .Include(b => b.Invites)
            .FirstOrDefaultAsync(b => b.Id == e.BookingId);
        if (booking is null) return;

        var recipientIds = booking.Invites.Select(i => i.PlayerId).ToList();
        recipientIds.Add(booking.OrganizerId);

        foreach (var recipientId in recipientIds)
        {
            await _dbContext.Notifications.AddAsync(new Notification
            {
                UserId = recipientId,
                Title = "Booking Timed Out",
                Message = $"Booking #{e.BookingId} at '{booking.Field.Name}' was automatically cancelled because not all players responded in time.",
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }
    }
}