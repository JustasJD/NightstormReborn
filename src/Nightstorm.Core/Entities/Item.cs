using Nightstorm.Core.Constants;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents an item in the game.
/// Items can be equipment (weapons, armor, accessories), consumables, materials, or quest items.
/// </summary>
public class Item : BaseEntity
{
    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item type (Weapon, Armor, Accessory, Consumable, etc.).
    /// </summary>
    public ItemType Type { get; set; }

    /// <summary>
    /// Gets or sets the item rarity (Common, Uncommon, Rare, Epic, Legendary, Mythic).
    /// Affects stat bonuses and drop rates.
    /// </summary>
    public ItemRarity Rarity { get; set; }

    /// <summary>
    /// Gets or sets the item grade (NG, D, C, B, A, S).
    /// Determines level requirement and base power.
    /// </summary>
    public ItemGrade Grade { get; set; }

    /// <summary>
    /// Gets or sets the base vendor value in gold.
    /// Final sell price is affected by rarity multiplier.
    /// </summary>
    public int BaseValue { get; set; }

    /// <summary>
    /// Gets or sets the required character level to use/equip this item.
    /// Usually auto-calculated from Grade.
    /// </summary>
    public int RequiredLevel { get; set; }

    /// <summary>
    /// Gets or sets the required character class to use/equip this item (optional).
    /// Null means any class can use it.
    /// </summary>
    public CharacterClass? RequiredClass { get; set; }

    /// <summary>
    /// Gets or sets the equipment slot this item can be equipped in (null for non-equipment).
    /// </summary>
    public EquipmentSlot? EquipmentSlot { get; set; }

    /// <summary>
    /// Gets or sets the weapon type (null if not a weapon).
    /// </summary>
    public WeaponType? WeaponType { get; set; }

    /// <summary>
    /// Gets or sets the armor material (null if not armor).
    /// </summary>
    public ArmorMaterial? ArmorMaterial { get; set; }

    /// <summary>
    /// Gets or sets the minimum weapon damage (for weapons only).
    /// </summary>
    public int MinDamage { get; set; }

    /// <summary>
    /// Gets or sets the maximum weapon damage (for weapons only).
    /// </summary>
    public int MaxDamage { get; set; }

    /// <summary>
    /// Gets or sets the weapon attack speed multiplier.
    /// Default: 1.0 (normal speed), higher = faster attacks.
    /// </summary>
    public decimal AttackSpeed { get; set; } = 1.0m;

    /// <summary>
    /// Gets or sets the critical hit chance bonus (percentage).
    /// </summary>
    public decimal CriticalChance { get; set; }

    /// <summary>
    /// Gets or sets the physical armor/defense value (for armor only).
    /// </summary>
    public int ArmorValue { get; set; }

    /// <summary>
    /// Gets or sets the magic resistance value.
    /// </summary>
    public int MagicResistance { get; set; }

    /// <summary>
    /// Gets or sets the block chance (percentage, mainly for shields).
    /// </summary>
    public decimal BlockChance { get; set; }

    /// <summary>
    /// Gets or sets the Strength bonus provided by this item.
    /// Increases physical damage and carry capacity.
    /// </summary>
    public int BonusStrength { get; set; }

    /// <summary>
    /// Gets or sets the Dexterity bonus provided by this item.
    /// Increases attack speed, accuracy, and evasion.
    /// </summary>
    public int BonusDexterity { get; set; }

    /// <summary>
    /// Gets or sets the Constitution bonus provided by this item.
    /// Increases max health and physical defense.
    /// </summary>
    public int BonusConstitution { get; set; }

    /// <summary>
    /// Gets or sets the Intelligence bonus provided by this item.
    /// Increases magical damage and spell effectiveness.
    /// </summary>
    public int BonusIntelligence { get; set; }

    /// <summary>
    /// Gets or sets the Wisdom bonus provided by this item.
    /// Increases magical defense and mana regeneration.
    /// </summary>
    public int BonusWisdom { get; set; }

    /// <summary>
    /// Gets or sets the Spirit bonus provided by this item.
    /// Increases max mana and spell power.
    /// </summary>
    public int BonusSpirit { get; set; }

    /// <summary>
    /// Gets or sets the Luck bonus provided by this item.
    /// Increases drop rates and critical chance.
    /// </summary>
    public int BonusLuck { get; set; }

    /// <summary>
    /// Gets or sets the bonus max health provided by this item.
    /// </summary>
    public int BonusMaxHealth { get; set; }

    /// <summary>
    /// Gets or sets the bonus max mana provided by this item.
    /// </summary>
    public int BonusMaxMana { get; set; }

    /// <summary>
    /// Gets or sets the health restored by this consumable.
    /// </summary>
    public int HealthRestore { get; set; }

    /// <summary>
    /// Gets or sets the mana restored by this consumable.
    /// </summary>
    public int ManaRestore { get; set; }

    /// <summary>
    /// Gets or sets the maximum stack size for this item (1 for equipment).
    /// </summary>
    public int MaxStackSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is tradeable between players.
    /// Quest items and some special items are not tradeable.
    /// </summary>
    public bool IsTradeable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is quest-related.
    /// Quest items cannot be dropped or sold.
    /// </summary>
    public bool IsQuestItem { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is bound to the character.
    /// Bound items cannot be traded or given to other characters.
    /// </summary>
    public bool IsSoulbound { get; set; }

    /// <summary>
    /// Gets or sets the item icon/image identifier (for UI display).
    /// </summary>
    public string? IconId { get; set; }

    /// <summary>
    /// Navigation property to character inventories containing this item.
    /// </summary>
    public ICollection<CharacterItem> CharacterItems { get; set; } = new List<CharacterItem>();

    /// <summary>
    /// Calculates the item's total power based on grade and rarity.
    /// Used for comparing items and determining effectiveness.
    /// </summary>
    public decimal GetItemPower()
    {
        return Grade.CalculateItemPower(Rarity);
    }

    /// <summary>
    /// Calculates the vendor sell price including rarity multiplier.
    /// </summary>
    public int GetVendorSellPrice()
    {
        return (int)(BaseValue * Rarity.GetVendorPriceMultiplier());
    }

    /// <summary>
    /// Gets whether this item is equipment (can be equipped in a slot).
    /// </summary>
    public bool IsEquipment()
    {
        return Type is ItemType.Weapon or ItemType.Armor or ItemType.Accessory;
    }

    /// <summary>
    /// Gets whether this item is a weapon.
    /// </summary>
    public bool IsWeapon()
    {
        return Type == ItemType.Weapon && WeaponType.HasValue;
    }

    /// <summary>
    /// Gets whether this item is armor.
    /// </summary>
    public bool IsArmor()
    {
        return Type == ItemType.Armor && ArmorMaterial.HasValue;
    }

    /// <summary>
    /// Gets whether this item is a consumable.
    /// </summary>
    public bool IsConsumable()
    {
        return Type == ItemType.Consumable;
    }

    /// <summary>
    /// Checks if a character can equip/use this item.
    /// </summary>
    /// <param name="characterLevel">Character's current level</param>
    /// <param name="characterClass">Character's class</param>
    /// <returns>True if character meets requirements</returns>
    public bool CanBeUsedBy(int characterLevel, CharacterClass characterClass)
    {
        // Check level requirement
        if (characterLevel < RequiredLevel)
        {
            return false;
        }

        // Check class requirement (if specified)
        if (RequiredClass.HasValue && RequiredClass.Value != characterClass)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a formatted display name with grade and rarity.
    /// Example: "🔵 D-Grade ⭐ Legendary Steel Sword"
    /// </summary>
    public string GetFullDisplayName()
    {
        return $"{Grade.GetDiscordDisplayName(Rarity)} {Name}";
    }

    public Item()
    {
        Grade = ItemGrade.NG;
        RequiredLevel = ItemConstants.MinimumLevel;
        MaxStackSize = ItemConstants.DefaultStackSize;
        IsTradeable = ItemConstants.DefaultTradeable;
        AttackSpeed = 1.0m;
    }
}
