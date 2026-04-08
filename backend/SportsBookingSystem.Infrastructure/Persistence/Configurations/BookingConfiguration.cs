using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.StartTime).IsRequired();

        builder.Property(b => b.BookingType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.RequiredPlayerCount).IsRequired();

        builder.HasIndex(b => new { b.FieldId, b.StartTime });
        builder.HasIndex(b => b.OrganizerId);

        builder.HasOne(b => b.Field)
            .WithMany(f => f.Bookings)
            .HasForeignKey(b => b.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Organizer)
            .WithMany(u => u.OrganizedBookings)
            .HasForeignKey(b => b.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}