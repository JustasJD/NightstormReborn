using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CombatLogEntry entity.
/// </summary>
public class CombatLogEntryConfiguration : IEntityTypeConfiguration<CombatLogEntry>
{
    public void Configure(EntityTypeBuilder<CombatLogEntry> builder)
    {
        builder.ToTable("CombatLogEntries");

        builder.HasKey(l => l.Id);

        // Combat instance relationship (configured in CombatInstanceConfiguration)

        // Properties
        builder.Property(l => l.ActorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.TargetName)
            .HasMaxLength(100);

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Indexes for performance
        builder.HasIndex(l => l.CombatInstanceId);

        builder.HasIndex(l => new { l.CombatInstanceId, l.Turn });

        builder.HasIndex(l => l.ActorId);

        // Concurrency token
        builder.Property(l => l.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(l => l.Timestamp);
    }
}
