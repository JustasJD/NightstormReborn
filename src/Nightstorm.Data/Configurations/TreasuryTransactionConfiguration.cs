using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for TreasuryTransaction entity.
/// </summary>
public class TreasuryTransactionConfiguration : IEntityTypeConfiguration<TreasuryTransaction>
{
    public void Configure(EntityTypeBuilder<TreasuryTransaction> builder)
    {
        builder.ToTable("TreasuryTransactions");

        builder.HasKey(t => t.Id);

        // Zone treasury relationship (configured in ZoneTreasuryConfiguration)

        // Character relationship (optional - only for entry fees)
        builder.HasOne(t => t.Character)
            .WithMany()
            .HasForeignKey(t => t.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Guild relationship (optional - only for withdrawals)
        builder.HasOne(t => t.Guild)
            .WithMany()
            .HasForeignKey(t => t.GuildId)
            .OnDelete(DeleteBehavior.Restrict);

        // Properties
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Indexes for performance
        builder.HasIndex(t => t.ZoneTreasuryId);

        builder.HasIndex(t => new { t.ZoneTreasuryId, t.Type });

        builder.HasIndex(t => t.CharacterId);

        builder.HasIndex(t => t.GuildId);

        builder.HasIndex(t => t.CreatedAt);

        // Concurrency token
        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        // Ignore computed properties
        builder.Ignore(t => t.TransactionTime);
    }
}
