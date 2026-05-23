using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;

namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class UserSportProfileConfiguration : IEntityTypeConfiguration<UserSportProfile>
{
    public void Configure(EntityTypeBuilder<UserSportProfile> builder)
    {
        builder.ToTable("UserSportProfiles");
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.SportType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(sp => sp.Level)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sp => sp.FavoriteAthlete)
            .HasMaxLength(100);

        builder.HasOne(sp => sp.User)
            .WithMany(u => u.SportProfiles)
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sp => new { sp.UserId, sp.SportType }).IsUnique();
    }
}
