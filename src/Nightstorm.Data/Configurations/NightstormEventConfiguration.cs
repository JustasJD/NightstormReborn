using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for NightstormEvent entity.
/// </summary>
public class NightstormEventConfiguration : IEntityTypeConfiguration<NightstormEvent>
{
    public void Configure(EntityTypeBuilder<NightstormEvent> builder)
    {
        builder.ToTable("NightstormEvents");

        builder.HasKey(e => e.Id);

        // Zone relationship
        builder.HasOne(e => e.Zone)
            .WithMany()
            .HasForeignKey(e => e.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Combat instance relationship (one-to-one)
        builder.HasOne(e => e.CombatInstance)
            .WithOne(c => c.NightstormEvent)
            .HasForeignKey<NightstormEvent>(e => e.CombatInstanceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Registered players relationship (configured in PlayerStateConfiguration)

        // Properties
        builder.Property(e => e.MaxParticipants)
            .HasDefaultValue(10);

        builder.Property(e => e.CancellationReason)
            .HasMaxLength(500);

        // Indexes for performance
        builder.HasIndex(e => e.ZoneId);

        builder.HasIndex(e => e.Status);

        builder.HasIndex(e => new { e.ZoneId, e.Status });

        builder.HasIndex(e => e.ScheduledAt);

        builder.HasIndex(e => new { e.Status, e.ScheduledAt });

        // Concurrency token
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(e => e.RegisteredCount);
        builder.Ignore(e => e.IsFull);
        builder.Ignore(e => e.IsRegistrationOpen);
        builder.Ignore(e => e.CanStart);
        builder.Ignore(e => e.ShouldBeRaided);
    }
}
