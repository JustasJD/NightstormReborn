namespace Nightstorm.Core.Enums;

/// <summary>
/// Extension methods for ItemRarity enum.
/// </summary>
public static class ItemRarityExtensions
{
    /// <summary>
    /// Gets the display color hex code for the rarity.
    /// Used for UI coloring in web/mobile apps.
    /// </summary>
    public static string GetColorHex(this ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => "#9D9D9D",      // Gray
            ItemRarity.Uncommon => "#1EFF00",    // Green
            ItemRarity.Rare => "#0070DD",        // Blue
            ItemRarity.Epic => "#A335EE",        // Purple
            ItemRarity.Legendary => "#FF8000",   // Orange
            ItemRarity.Mythic => "#E6CC80",      // Red/Gold
            _ => "#FFFFFF"                       // White (fallback)
        };
    }

    /// <summary>
    /// Gets the Discord color code (decimal) for embeds.
    /// </summary>
    public static int GetDiscordColor(this ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => 0x9D9D9D,      // Gray
            ItemRarity.Uncommon => 0x1EFF00,    // Green
            ItemRarity.Rare => 0x0070DD,        // Blue
            ItemRarity.Epic => 0xA335EE,        // Purple
            ItemRarity.Legendary => 0xFF8000,   // Orange
            ItemRarity.Mythic => 0xE6CC80,      // Red/Gold
            _ => 0xFFFFFF
        };
    }

    /// <summary>
    /// Gets the stat multiplier for this rarity level.
    /// Used to calculate item stat bonuses.
    /// </summary>
    public static decimal GetStatMultiplier(this ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => 1.0m,          // Base stats
            ItemRarity.Uncommon => 1.15m,       // +15%
            ItemRarity.Rare => 1.35m,           // +35%
            ItemRarity.Epic => 1.65m,           // +65%
            ItemRarity.Legendary => 2.0m,       // +100%
            ItemRarity.Mythic => 2.5m,          // +150%
            _ => 1.0m
        };
    }

    /// <summary>
    /// Gets the vendor sell price multiplier for this rarity.
    /// </summary>
    public static decimal GetVendorPriceMultiplier(this ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => 1.0m,
            ItemRarity.Uncommon => 2.5m,
            ItemRarity.Rare => 5.0m,
            ItemRarity.Epic => 10.0m,
            ItemRarity.Legendary => 25.0m,
            ItemRarity.Mythic => 100.0m,
            _ => 1.0m
        };
    }

    /// <summary>
    /// Gets the display name with color indicator.
    /// </summary>
    public static string GetDisplayName(this ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Mythic => "? Mythic",
            ItemRarity.Legendary => "? Legendary",
            ItemRarity.Epic => "?? Epic",
            ItemRarity.Rare => "?? Rare",
            ItemRarity.Uncommon => "?? Uncommon",
            ItemRarity.Common => "? Common",
            _ => rarity.ToString()
        };
    }

    /// <summary>
    /// Gets the drop rate percentage (approximate).
    /// </summary>
    public static decimal GetDropRatePercentage(this ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => 65.0m,         // 65%
            ItemRarity.Uncommon => 25.0m,       // 25%
            ItemRarity.Rare => 8.0m,            // 8%
            ItemRarity.Epic => 1.8m,            // 1.8%
            ItemRarity.Legendary => 0.19m,      // 0.19%
            ItemRarity.Mythic => 0.01m,         // 0.01% (1 in 10,000)
            _ => 1.0m
        };
    }
}
