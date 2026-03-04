using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class BookingInviteConfigurations : IEntityTypeConfiguration<BookingInvite>
{
    public void Configure(EntityTypeBuilder<BookingInvite> builder)
    {
        builder.ToTable("BookingInvites");
        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(bi => new { bi.BookingId, bi.PlayerId }).IsUnique();

        builder.HasOne(bi => bi.Booking)
            .WithMany(b => b.Invites)
            .HasForeignKey(bi => bi.BookingId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(bi => bi.Player)
            .WithMany(u => u.Invites)
            .HasForeignKey(bi => bi.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(bi => bi.PlayerId);
    }
}