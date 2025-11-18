using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Guild entity.
/// </summary>
public class GuildConfiguration : IEntityTypeConfiguration<Guild>
{
    public void Configure(EntityTypeBuilder<Guild> builder)
    {
        builder.ToTable("Guilds");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Tag)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.LeaderId)
            .IsRequired();

        builder.Property(g => g.Level)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(g => g.Experience)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(g => g.MaxMembers)
            .IsRequired()
            .HasDefaultValue(50);

        builder.Property(g => g.Treasury)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(g => g.Name)
            .IsUnique();

        builder.HasIndex(g => g.Tag)
            .IsUnique();

        builder.HasIndex(g => g.LeaderId);
    }
}
