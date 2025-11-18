using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Item entity.
/// </summary>
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Description)
            .HasMaxLength(500);

        builder.Property(i => i.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Rarity)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Value)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.RequiredLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(i => i.MaxStackSize)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(i => i.IsTradeable)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(i => i.Name);

        builder.HasIndex(i => i.Type);

        builder.HasIndex(i => i.Rarity);

        builder.HasIndex(i => i.RequiredLevel);
    }
}
