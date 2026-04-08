using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Domain.Enums;
using SportsBookingSystem.Domain.Events;

namespace SportsBookingSystem.Infrastructure.Jobs;

public class TimeoutStaleBookingsJob
{
    private readonly IApplicationDbContext _dbContext;
    private readonly int _timeoutHours;
    
    public TimeoutStaleBookingsJob(IApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _timeoutHours = configuration.GetValue<int>("BookingTimeoutHours", 24);
    }
    

    public async Task ExecuteAsync()
    {
        var cutoff = DateTimeOffset.UtcNow.AddHours(_timeoutHours);

        var staleBookings = await _dbContext.Bookings
            .Where(b => b.StartTime <= cutoff
                        && (b.Status == BookingStatus.Requested
                            || b.Status == BookingStatus.PendingPlayerConfirmations))
            .ToListAsync();
        
        foreach (var booking in staleBookings)
        {
            booking.Status = BookingStatus.TimedOut;
            booking.RaiseDomainEvent(new BookingTimedOutEvent(booking.Id));
        }

        await _dbContext.SaveChangesAsync();
    }
}