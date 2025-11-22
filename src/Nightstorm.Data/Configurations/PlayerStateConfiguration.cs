using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PlayerState entity.
/// </summary>
public class PlayerStateConfiguration : IEntityTypeConfiguration<PlayerState>
{
    public void Configure(EntityTypeBuilder<PlayerState> builder)
    {
        builder.ToTable("PlayerStates");

        builder.HasKey(p => p.Id);

        // Character relationship (one-to-one)
        builder.HasOne(p => p.Character)
            .WithMany()
            .HasForeignKey(p => p.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Current zone relationship
        builder.HasOne(p => p.CurrentZone)
            .WithMany()
            .HasForeignKey(p => p.CurrentZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Destination zone relationship
        builder.HasOne(p => p.DestinationZone)
            .WithMany()
            .HasForeignKey(p => p.DestinationZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Current combat relationship
        builder.HasOne(p => p.CurrentCombat)
            .WithMany()
            .HasForeignKey(p => p.CurrentCombatId)
            .OnDelete(DeleteBehavior.SetNull);

        // Registered event relationship
        builder.HasOne(p => p.RegisteredEvent)
            .WithMany(e => e.RegisteredPlayers)
            .HasForeignKey(p => p.RegisteredEventId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes for performance
        builder.HasIndex(p => p.CharacterId)
            .IsUnique();

        builder.HasIndex(p => p.Location);

        builder.HasIndex(p => new { p.CurrentZoneId, p.Location });

        // Concurrency token
        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(p => p.IsTravelling);
        builder.Ignore(p => p.IsInCombat);
        builder.Ignore(p => p.CanAct);
        builder.Ignore(p => p.RemainingTravelTime);
    }
}
