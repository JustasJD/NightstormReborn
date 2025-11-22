using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CombatParticipant entity.
/// </summary>
public class CombatParticipantConfiguration : IEntityTypeConfiguration<CombatParticipant>
{
    public void Configure(EntityTypeBuilder<CombatParticipant> builder)
    {
        builder.ToTable("CombatParticipants");

        builder.HasKey(p => p.Id);

        // Combat instance relationship (configured in CombatInstanceConfiguration)

        // Character relationship (optional - only for players)
        builder.HasOne(p => p.Character)
            .WithMany()
            .HasForeignKey(p => p.EntityId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Monster relationship (optional - only for monsters)
        builder.HasOne(p => p.Monster)
            .WithMany()
            .HasForeignKey(p => p.EntityId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.IsAlive)
            .HasDefaultValue(true);

        // Indexes for performance
        builder.HasIndex(p => p.CombatInstanceId);

        builder.HasIndex(p => new { p.CombatInstanceId, p.Type });

        builder.HasIndex(p => new { p.CombatInstanceId, p.TurnOrder });

        builder.HasIndex(p => p.EntityId);

        // Concurrency token
        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(p => p.HealthPercentage);
        builder.Ignore(p => p.IsCritical);
    }
}
