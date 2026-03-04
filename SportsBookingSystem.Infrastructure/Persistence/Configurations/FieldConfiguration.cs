using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.ToTable("Fields");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name).HasMaxLength(200).IsRequired();

        builder.Property(f => f.SportType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(f => f.BaseHourlyPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasIndex(f => f.ParkId);

        builder.HasOne(f => f.Park)
            .WithMany(p => p.Fields)
            .HasForeignKey(f => f.ParkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}