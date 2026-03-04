using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class ParkConfiguration: IEntityTypeConfiguration<Park>
{
    public void Configure(EntityTypeBuilder<Park> builder)
    {
        builder.ToTable("Parks");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Address).HasMaxLength(256).IsRequired();
        builder.Property(p => p.City).HasMaxLength(100).IsRequired();

        builder.HasIndex(p => p.ManagerId);

        builder.HasOne(p => p.Manager)
            .WithMany()                      
            .HasForeignKey(p => p.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}