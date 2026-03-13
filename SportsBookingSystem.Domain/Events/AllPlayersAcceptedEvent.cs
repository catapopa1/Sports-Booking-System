using SportsBookingSystem.Domain.Common;

namespace SportsBookingSystem.Domain.Events;

public record AllPlayersAcceptedEvent(int BookingId) : IDomainEvent;