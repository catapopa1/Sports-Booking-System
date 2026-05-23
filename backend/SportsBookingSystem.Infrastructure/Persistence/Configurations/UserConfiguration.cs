using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsBookingSystem.Domain.Entities;
namespace SportsBookingSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(512);

        builder.Property(u => u.Bio).HasMaxLength(500);
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(512);

        builder.Property(u => u.ExternalProvider).HasMaxLength(30);
        builder.Property(u => u.ExternalId).HasMaxLength(256);

        // One external-account row per (provider, externalId).
        builder.HasIndex(u => new { u.ExternalProvider, u.ExternalId })
            .IsUnique()
            .HasFilter("[ExternalProvider] IS NOT NULL AND [ExternalId] IS NOT NULL");

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}