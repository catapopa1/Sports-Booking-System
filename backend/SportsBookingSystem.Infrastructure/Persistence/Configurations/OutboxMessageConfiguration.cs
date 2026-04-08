using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Outbox;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Type).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Payload).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(o => o.CreatedAt).IsRequired();
        
        builder.HasIndex(o => o.ProcessedAt);
    }
}