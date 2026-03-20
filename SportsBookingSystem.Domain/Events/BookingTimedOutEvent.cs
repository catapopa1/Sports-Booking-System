using SportsBookingSystem.Domain.Common;

namespace SportsBookingSystem.Domain.Events;

public record BookingTimedOutEvent(int BookingId) : IDomainEvent;