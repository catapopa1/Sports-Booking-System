using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class ParkPhotoConfiguration : IEntityTypeConfiguration<ParkPhoto>
{
    public void Configure(EntityTypeBuilder<ParkPhoto> builder)
    {
        builder.ToTable("ParkPhotos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Url).HasMaxLength(512).IsRequired();
        builder.Property(p => p.IsMain).HasDefaultValue(false);
        builder.Property(p => p.OrderIndex).HasDefaultValue(0);

        builder.HasOne(p => p.Park)
            .WithMany(p => p.Photos)
            .HasForeignKey(p => p.ParkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.ParkId, p.IsMain });

        // At most one main photo per park.
        builder.HasIndex(p => p.ParkId)
            .HasFilter("[IsMain] = 1")
            .IsUnique();
    }
}
