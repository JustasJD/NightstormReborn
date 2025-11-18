using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Monster entity.
/// </summary>
public class MonsterConfiguration : IEntityTypeConfiguration<Monster>
{
    public void Configure(EntityTypeBuilder<Monster> builder)
    {
        builder.ToTable("Monsters");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Level)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(m => m.MaxHealth)
            .IsRequired();

        builder.Property(m => m.AttackType)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(1); // HeavyMelee

        builder.Property(m => m.AttackPower)
            .IsRequired();

        builder.Property(m => m.ArmorType)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(1); // Heavy

        builder.Property(m => m.HeavyMeleeDefense)
            .IsRequired();

        builder.Property(m => m.FastMeleeDefense)
            .IsRequired();

        builder.Property(m => m.ElementalMagicDefense)
            .IsRequired();

        builder.Property(m => m.SpiritualMagicDefense)
            .IsRequired();

        builder.Property(m => m.ExperienceReward)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.GoldDrop)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.IsBoss)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.DropRate)
            .IsRequired()
            .HasDefaultValue(0.1)
            .HasPrecision(3, 2);

        // Indexes
        builder.HasIndex(m => m.Name);

        builder.HasIndex(m => m.Type);

        builder.HasIndex(m => m.Level);

        builder.HasIndex(m => m.IsBoss);

        builder.HasIndex(m => m.AttackType);

        builder.HasIndex(m => m.ArmorType);
    }
}
