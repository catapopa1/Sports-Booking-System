using SportsBookingSystem.Domain.Common;

namespace SportsBookingSystem.Domain.Events;

public record BookingCancelledEvent(int BookingId) : IDomainEvent;