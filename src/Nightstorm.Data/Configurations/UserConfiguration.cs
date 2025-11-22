using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.DiscordUsername)
            .HasMaxLength(100);

        builder.Property(u => u.GoogleId)
            .HasMaxLength(255);

        builder.Property(u => u.AppleId)
            .HasMaxLength(255);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.LastLoginAt);

        // Indexes for performance
        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email")
            .HasFilter("\"Email\" IS NOT NULL");

        builder.HasIndex(u => u.DiscordId)
            .IsUnique()
            .HasDatabaseName("IX_Users_DiscordId")
            .HasFilter("\"DiscordId\" IS NOT NULL");

        builder.HasIndex(u => u.GoogleId)
            .IsUnique()
            .HasDatabaseName("IX_Users_GoogleId")
            .HasFilter("\"GoogleId\" IS NOT NULL");

        builder.HasIndex(u => u.AppleId)
            .IsUnique()
            .HasDatabaseName("IX_Users_AppleId")
            .HasFilter("\"AppleId\" IS NOT NULL");

        // Relationships
        builder.HasMany(u => u.Characters)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete filter
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
