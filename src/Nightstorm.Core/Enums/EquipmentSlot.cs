namespace Nightstorm.Core.Enums;

/// <summary>
/// Defines all available equipment slots for characters.
/// Each character can equip one item per slot (except rings - 2 slots).
/// </summary>
public enum EquipmentSlot
{
    /// <summary>
    /// Main hand weapon slot.
    /// Can hold one-handed weapon (allows off-hand) or two-handed weapon (blocks off-hand).
    /// </summary>
    MainHand = 0,

    /// <summary>
    /// Off-hand weapon or shield slot.
    /// Cannot be used when wielding a two-handed weapon in main hand.
    /// </summary>
    OffHand = 1,

    /// <summary>
    /// Head/Helmet slot.
    /// Provides armor and stat bonuses.
    /// </summary>
    Head = 2,

    /// <summary>
    /// Shoulder/Pauldrons slot.
    /// Provides armor and stat bonuses.
    /// </summary>
    Shoulders = 3,

    /// <summary>
    /// Chest/Armor slot.
    /// Primary armor piece with highest defense values.
    /// </summary>
    Chest = 4,

    /// <summary>
    /// Hands/Gloves slot.
    /// Provides armor and dexterity bonuses.
    /// </summary>
    Hands = 5,

    /// <summary>
    /// Waist/Belt slot.
    /// Provides strength and constitution bonuses.
    /// </summary>
    Belt = 6,

    /// <summary>
    /// Legs/Pants slot.
    /// Provides armor and movement bonuses.
    /// </summary>
    Legs = 7,

    /// <summary>
    /// Feet/Boots slot.
    /// Provides armor and movement bonuses.
    /// </summary>
    Feet = 8,

    /// <summary>
    /// Back/Cloak slot.
    /// Provides resistances and special effects.
    /// </summary>
    Cloak = 9,

    /// <summary>
    /// Neck/Amulet slot.
    /// Provides magical bonuses and special effects.
    /// </summary>
    Amulet = 10,

    /// <summary>
    /// Ring slot 1 (left hand).
    /// Provides magical bonuses and special effects.
    /// </summary>
    Ring1 = 11,

    /// <summary>
    /// Ring slot 2 (right hand).
    /// Provides magical bonuses and special effects.
    /// </summary>
    Ring2 = 12
}
