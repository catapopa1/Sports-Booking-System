namespace SportsBookingSystem.Domain.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string Payload { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; } = null;
    public string? Error { get; set; }
    public int RetryCount { get; set; } = 0;
}