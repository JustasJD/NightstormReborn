using Nightstorm.Core.Constants;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents an item in the game.
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
    /// Gets or sets the item type.
    /// </summary>
    public ItemType Type { get; set; }

    /// <summary>
    /// Gets or sets the item rarity.
    /// </summary>
    public ItemRarity Rarity { get; set; }

    /// <summary>
    /// Gets or sets the item value in gold.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the required level to use this item.
    /// </summary>
    public int RequiredLevel { get; set; }

    /// <summary>
    /// Gets or sets the strength bonus provided by this item.
    /// </summary>
    public int StrengthBonus { get; set; }

    /// <summary>
    /// Gets or sets the dexterity bonus provided by this item.
    /// </summary>
    public int DexterityBonus { get; set; }

    /// <summary>
    /// Gets or sets the constitution bonus provided by this item.
    /// </summary>
    public int ConstitutionBonus { get; set; }

    /// <summary>
    /// Gets or sets the intelligence bonus provided by this item.
    /// </summary>
    public int IntelligenceBonus { get; set; }

    /// <summary>
    /// Gets or sets the wisdom bonus provided by this item.
    /// </summary>
    public int WisdomBonus { get; set; }

    /// <summary>
    /// Gets or sets the spirit bonus provided by this item.
    /// </summary>
    public int SpiritBonus { get; set; }

    /// <summary>
    /// Gets or sets the luck bonus provided by this item.
    /// </summary>
    public int LuckBonus { get; set; }

    /// <summary>
    /// Gets or sets the maximum stack size for this item.
    /// </summary>
    public int MaxStackSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is tradeable.
    /// </summary>
    public bool IsTradeable { get; set; }

    /// <summary>
    /// Navigation property to character inventories containing this item.
    /// </summary>
    public ICollection<CharacterItem> CharacterItems { get; set; } = new List<CharacterItem>();

    public Item()
    {
        RequiredLevel = ItemConstants.MinimumLevel;
        MaxStackSize = ItemConstants.DefaultStackSize;
        IsTradeable = ItemConstants.DefaultTradeable;
    }
}
