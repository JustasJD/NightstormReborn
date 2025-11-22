using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for TravelLog entity.
/// </summary>
public class TravelLogConfiguration : IEntityTypeConfiguration<TravelLog>
{
    public void Configure(EntityTypeBuilder<TravelLog> builder)
    {
        builder.ToTable("TravelLogs");

        builder.HasKey(t => t.Id);

        // Character relationship
        builder.HasOne(t => t.Character)
            .WithMany()
            .HasForeignKey(t => t.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Origin zone relationship
        builder.HasOne(t => t.OriginZone)
            .WithMany()
            .HasForeignKey(t => t.OriginZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Destination zone relationship
        builder.HasOne(t => t.DestinationZone)
            .WithMany()
            .HasForeignKey(t => t.DestinationZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(t => t.CharacterId);

        builder.HasIndex(t => new { t.CharacterId, t.Status });

        builder.HasIndex(t => new { t.Status, t.StartedAt });

        builder.HasIndex(t => t.DestinationZoneId);

        // Concurrency token
        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(t => t.Duration);
        builder.Ignore(t => t.ExpectedCompletionTime);
        builder.Ignore(t => t.ShouldBeComplete);
    }
}
