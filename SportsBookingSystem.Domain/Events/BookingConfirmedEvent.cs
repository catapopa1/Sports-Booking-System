using SportsBookingSystem.Domain.Common;

namespace SportsBookingSystem.Domain.Events;

public record BookingConfirmedEvent(int BookingId) : IDomainEvent;