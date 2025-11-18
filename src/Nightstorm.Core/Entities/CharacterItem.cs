namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents the many-to-many relationship between characters and items (inventory).
/// </summary>
public class CharacterItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the character ID.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Navigation property to the character.
    /// </summary>
    public Character Character { get; set; } = null!;

    /// <summary>
    /// Gets or sets the item ID.
    /// </summary>
    public Guid ItemId { get; set; }

    /// <summary>
    /// Navigation property to the item.
    /// </summary>
    public Item Item { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity of this item.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is equipped.
    /// </summary>
    public bool IsEquipped { get; set; }

    /// <summary>
    /// Gets or sets the inventory slot position.
    /// </summary>
    public int? SlotPosition { get; set; }

    public CharacterItem()
    {
        Quantity = 1;
        IsEquipped = false;
    }
}
