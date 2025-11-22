using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ZoneTreasury entity.
/// </summary>
public class ZoneTreasuryConfiguration : IEntityTypeConfiguration<ZoneTreasury>
{
    public void Configure(EntityTypeBuilder<ZoneTreasury> builder)
    {
        builder.ToTable("ZoneTreasuries");

        builder.HasKey(z => z.Id);

        // Zone relationship (one-to-one)
        builder.HasOne(z => z.Zone)
            .WithMany()
            .HasForeignKey(z => z.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Last withdrawn by guild relationship
        builder.HasOne(z => z.LastWithdrawnByGuild)
            .WithMany()
            .HasForeignKey(z => z.LastWithdrawnByGuildId)
            .OnDelete(DeleteBehavior.SetNull);

        // Transactions relationship
        builder.HasMany(z => z.Transactions)
            .WithOne(t => t.ZoneTreasury)
            .HasForeignKey(t => t.ZoneTreasuryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties
        builder.Property(z => z.CurrentGold)
            .HasDefaultValue(0);

        builder.Property(z => z.TotalCollected)
            .HasDefaultValue(0);

        builder.Property(z => z.TotalWithdrawn)
            .HasDefaultValue(0);

        // Indexes for performance
        builder.HasIndex(z => z.ZoneId)
            .IsUnique();

        builder.HasIndex(z => z.LastWithdrawalAt);

        // Concurrency token
        builder.Property(z => z.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(z => z.CanWithdrawToday);
        builder.Ignore(z => z.NextWithdrawalAvailableAt);
    }
}
