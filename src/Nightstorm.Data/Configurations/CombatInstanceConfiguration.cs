using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CombatInstance entity.
/// </summary>
public class CombatInstanceConfiguration : IEntityTypeConfiguration<CombatInstance>
{
    public void Configure(EntityTypeBuilder<CombatInstance> builder)
    {
        builder.ToTable("CombatInstances");

        builder.HasKey(c => c.Id);

        // Nightstorm event relationship (configured in NightstormEventConfiguration)

        // Participants relationship
        builder.HasMany(c => c.Participants)
            .WithOne(p => p.CombatInstance)
            .HasForeignKey(p => p.CombatInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Combat log relationship
        builder.HasMany(c => c.CombatLog)
            .WithOne(l => l.CombatInstance)
            .HasForeignKey(l => l.CombatInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(c => c.NightstormEventId)
            .IsUnique();

        builder.HasIndex(c => c.Status);

        builder.HasIndex(c => new { c.Status, c.CurrentTurn });

        // Concurrency token
        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(c => c.AlivePlayerCount);
        builder.Ignore(c => c.AliveMonsterCount);
        builder.Ignore(c => c.IsFinished);
        builder.Ignore(c => c.AllPlayersDead);
        builder.Ignore(c => c.AllMonstersDead);
    }
}
