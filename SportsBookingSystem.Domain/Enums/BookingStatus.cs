namespace SportsBookingSystem.Domain.Enums;

public enum BookingStatus
{
    Requested,
    PendingPlayerConfirmations,
    PendingManagerApproval,
    Confirmed,
    Cancelled,
    TimedOut
}