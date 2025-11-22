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

        // Basic Properties
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

        builder.Property(i => i.Grade)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.BaseValue)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.RequiredLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(i => i.RequiredClass)
            .HasConversion<int?>()
            .IsRequired(false);

        // Equipment Slot Properties
        builder.Property(i => i.EquipmentSlot)
            .HasConversion<int?>()
            .IsRequired(false);

        builder.Property(i => i.WeaponType)
            .HasConversion<int?>()
            .IsRequired(false);

        builder.Property(i => i.ArmorMaterial)
            .HasConversion<int?>()
            .IsRequired(false);

        // Weapon Properties
        builder.Property(i => i.MinDamage)
            .HasDefaultValue(0);

        builder.Property(i => i.MaxDamage)
            .HasDefaultValue(0);

        builder.Property(i => i.AttackSpeed)
            .HasPrecision(5, 2)
            .HasDefaultValue(1.0m);

        builder.Property(i => i.CriticalChance)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        // Armor Properties
        builder.Property(i => i.ArmorValue)
            .HasDefaultValue(0);

        builder.Property(i => i.MagicResistance)
            .HasDefaultValue(0);

        builder.Property(i => i.BlockChance)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        // Stat Bonuses
        builder.Property(i => i.BonusStrength)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusDexterity)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusConstitution)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusIntelligence)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusWisdom)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusSpirit)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusLuck)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusMaxHealth)
            .HasDefaultValue(0);

        builder.Property(i => i.BonusMaxMana)
            .HasDefaultValue(0);

        // Consumable Properties
        builder.Property(i => i.HealthRestore)
            .HasDefaultValue(0);

        builder.Property(i => i.ManaRestore)
            .HasDefaultValue(0);

        // Item Properties
        builder.Property(i => i.MaxStackSize)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(i => i.IsTradeable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.IsQuestItem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.IsSoulbound)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.IconId)
            .HasMaxLength(50)
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(i => i.Name)
            .HasDatabaseName("IX_Items_Name");

        builder.HasIndex(i => i.Type)
            .HasDatabaseName("IX_Items_Type");

        builder.HasIndex(i => i.Rarity)
            .HasDatabaseName("IX_Items_Rarity");

        builder.HasIndex(i => i.Grade)
            .HasDatabaseName("IX_Items_Grade");

        builder.HasIndex(i => i.RequiredLevel)
            .HasDatabaseName("IX_Items_RequiredLevel");

        builder.HasIndex(i => new { i.Type, i.Grade, i.Rarity })
            .HasDatabaseName("IX_Items_Type_Grade_Rarity");

        // Soft delete filter
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
