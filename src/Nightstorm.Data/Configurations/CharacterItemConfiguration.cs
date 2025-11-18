using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CharacterItem entity.
/// </summary>
public class CharacterItemConfiguration : IEntityTypeConfiguration<CharacterItem>
{
    public void Configure(EntityTypeBuilder<CharacterItem> builder)
    {
        builder.ToTable("CharacterItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(ci => ci.IsEquipped)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(ci => ci.CharacterId);

        builder.HasIndex(ci => ci.ItemId);

        builder.HasIndex(ci => new { ci.CharacterId, ci.ItemId });

        builder.HasIndex(ci => ci.IsEquipped);

        // Relationships
        builder.HasOne(ci => ci.Character)
            .WithMany(c => c.Inventory)
            .HasForeignKey(ci => ci.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Item)
            .WithMany(i => i.CharacterItems)
            .HasForeignKey(ci => ci.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
