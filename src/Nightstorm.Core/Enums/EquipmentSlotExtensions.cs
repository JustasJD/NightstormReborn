namespace Nightstorm.Core.Enums;

/// <summary>
/// Extension methods and helper functions for EquipmentSlot enum.
/// </summary>
public static class EquipmentSlotExtensions
{
    /// <summary>
    /// Gets a human-readable display name for the equipment slot.
    /// </summary>
    public static string GetDisplayName(this EquipmentSlot slot)
    {
        return slot switch
        {
            EquipmentSlot.MainHand => "Main Hand",
            EquipmentSlot.OffHand => "Off Hand",
            EquipmentSlot.Head => "Head",
            EquipmentSlot.Shoulders => "Shoulders",
            EquipmentSlot.Chest => "Chest",
            EquipmentSlot.Hands => "Hands",
            EquipmentSlot.Belt => "Belt",
            EquipmentSlot.Legs => "Legs",
            EquipmentSlot.Feet => "Feet",
            EquipmentSlot.Cloak => "Cloak",
            EquipmentSlot.Amulet => "Amulet",
            EquipmentSlot.Ring1 => "Ring 1",
            EquipmentSlot.Ring2 => "Ring 2",
            _ => slot.ToString()
        };
    }

    /// <summary>
    /// Checks if the slot is a weapon slot (MainHand or OffHand).
    /// </summary>
    public static bool IsWeaponSlot(this EquipmentSlot slot)
    {
        return slot is EquipmentSlot.MainHand or EquipmentSlot.OffHand;
    }

    /// <summary>
    /// Checks if the slot is an armor slot (Head, Shoulders, Chest, Hands, Belt, Legs, Feet).
    /// </summary>
    public static bool IsArmorSlot(this EquipmentSlot slot)
    {
        return slot is EquipmentSlot.Head or EquipmentSlot.Shoulders or EquipmentSlot.Chest
            or EquipmentSlot.Hands or EquipmentSlot.Belt or EquipmentSlot.Legs or EquipmentSlot.Feet;
    }

    /// <summary>
    /// Checks if the slot is an accessory slot (Cloak, Amulet, Ring1, Ring2).
    /// </summary>
    public static bool IsAccessorySlot(this EquipmentSlot slot)
    {
        return slot is EquipmentSlot.Cloak or EquipmentSlot.Amulet 
            or EquipmentSlot.Ring1 or EquipmentSlot.Ring2;
    }

    /// <summary>
    /// Checks if the slot is a ring slot.
    /// </summary>
    public static bool IsRingSlot(this EquipmentSlot slot)
    {
        return slot is EquipmentSlot.Ring1 or EquipmentSlot.Ring2;
    }

    /// <summary>
    /// Gets all equipment slots.
    /// </summary>
    public static IEnumerable<EquipmentSlot> GetAllSlots()
    {
        return Enum.GetValues<EquipmentSlot>();
    }

    /// <summary>
    /// Gets all weapon slots.
    /// </summary>
    public static IEnumerable<EquipmentSlot> GetWeaponSlots()
    {
        yield return EquipmentSlot.MainHand;
        yield return EquipmentSlot.OffHand;
    }

    /// <summary>
    /// Gets all armor slots.
    /// </summary>
    public static IEnumerable<EquipmentSlot> GetArmorSlots()
    {
        yield return EquipmentSlot.Head;
        yield return EquipmentSlot.Shoulders;
        yield return EquipmentSlot.Chest;
        yield return EquipmentSlot.Hands;
        yield return EquipmentSlot.Belt;
        yield return EquipmentSlot.Legs;
        yield return EquipmentSlot.Feet;
    }

    /// <summary>
    /// Gets all accessory slots.
    /// </summary>
    public static IEnumerable<EquipmentSlot> GetAccessorySlots()
    {
        yield return EquipmentSlot.Cloak;
        yield return EquipmentSlot.Amulet;
        yield return EquipmentSlot.Ring1;
        yield return EquipmentSlot.Ring2;
    }

    /// <summary>
    /// Gets the icon/emoji representation of the equipment slot.
    /// Useful for Discord bot display.
    /// </summary>
    public static string GetIcon(this EquipmentSlot slot)
    {
        return slot switch
        {
            EquipmentSlot.MainHand => "??",
            EquipmentSlot.OffHand => "???",
            EquipmentSlot.Head => "??",
            EquipmentSlot.Shoulders => "???",
            EquipmentSlot.Chest => "???",
            EquipmentSlot.Hands => "??",
            EquipmentSlot.Belt => "??",
            EquipmentSlot.Legs => "??",
            EquipmentSlot.Feet => "??",
            EquipmentSlot.Cloak => "??",
            EquipmentSlot.Amulet => "??",
            EquipmentSlot.Ring1 => "??",
            EquipmentSlot.Ring2 => "??",
            _ => "?"
        };
    }
}
