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

        // Configure properties
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.Difficulty)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(m => m.TemplateId)
            .HasMaxLength(100);

        builder.Property(m => m.Level)
            .IsRequired();

        builder.Property(m => m.MaxHealth)
            .IsRequired();
        
        builder.Property(m => m.CurrentHealth)
            .IsRequired();

        builder.Property(m => m.AttackType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.AttackPower)
            .IsRequired();

        builder.Property(m => m.ArmorType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.HeavyMeleeDefense)
            .IsRequired();

        builder.Property(m => m.FastMeleeDefense)
            .IsRequired();

        builder.Property(m => m.ElementalMagicDefense)
            .IsRequired();

        builder.Property(m => m.SpiritualMagicDefense)
            .IsRequired();

        builder.Property(m => m.ExperienceReward)
            .IsRequired();

        builder.Property(m => m.GoldDrop)
            .IsRequired();

        builder.Property(m => m.DropRate)
            .IsRequired()
            .HasPrecision(5, 2);

        // Configure relationships
        builder.HasOne(m => m.Zone)
            .WithMany()
            .HasForeignKey(m => m.ZoneId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure indexes
        builder.HasIndex(m => m.Type);
        builder.HasIndex(m => m.Difficulty);
        builder.HasIndex(m => m.Level);
        builder.HasIndex(m => m.TemplateId);
        builder.HasIndex(m => m.ZoneId);
    }
}
