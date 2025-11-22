namespace Nightstorm.Core.Enums;

/// <summary>
/// Extension methods for ItemGrade enum.
/// Provides level requirements, power calculations, and display helpers.
/// </summary>
public static class ItemGradeExtensions
{
    /// <summary>
    /// Gets the minimum character level required to equip items of this grade.
    /// </summary>
    public static int GetRequiredLevel(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => 1,
            ItemGrade.D => 20,
            ItemGrade.C => 40,
            ItemGrade.B => 50,
            ItemGrade.A => 70,
            ItemGrade.S => 85,
            _ => 1
        };
    }

    /// <summary>
    /// Gets the base power value for this grade.
    /// Used as multiplier for item stats (damage, armor, etc.).
    /// Values are tuned to allow overlap: Mythic lower grade > Common higher grade.
    /// </summary>
    public static int GetBasePower(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => 5,    // Starter gear
            ItemGrade.D => 10,    // Early game
            ItemGrade.C => 18,    // Mid-early (tuned for overlap)
            ItemGrade.B => 30,    // Mid game
            ItemGrade.A => 50,    // Late game
            ItemGrade.S => 85,    // End game
            _ => 1
        };
    }

    /// <summary>
    /// Calculates the final item power based on grade and rarity.
    /// Formula: BasePower × RarityMultiplier
    /// 
    /// Example overlap (intended design):
    /// - Mythic D (10 × 2.5 = 25) > Common C (18 × 1.0 = 18)
    /// - Legendary D (10 × 2.0 = 20) > Common C (18)
    /// - But Rare C (18 × 1.35 = 24) > Legendary D (20)
    /// </summary>
    public static decimal CalculateItemPower(this ItemGrade grade, ItemRarity rarity)
    {
        return grade.GetBasePower() * rarity.GetStatMultiplier();
    }

    /// <summary>
    /// Checks if the specified rarity is valid for this grade.
    /// A-Grade and S-Grade cannot be Common or Uncommon.
    /// </summary>
    public static bool IsValidRarity(this ItemGrade grade, ItemRarity rarity)
    {
        // A and S grade must be at least Rare
        if (grade is ItemGrade.A or ItemGrade.S)
        {
            return rarity >= ItemRarity.Rare;
        }

        // NG, D, C, B can be any rarity
        return true;
    }

    /// <summary>
    /// Gets the display name for the grade.
    /// </summary>
    public static string GetDisplayName(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => "No Grade",
            ItemGrade.D => "D-Grade",
            ItemGrade.C => "C-Grade",
            ItemGrade.B => "B-Grade",
            ItemGrade.A => "A-Grade",
            ItemGrade.S => "S-Grade",
            _ => grade.ToString()
        };
    }

    /// <summary>
    /// Gets the short display name (just the letter).
    /// </summary>
    public static string GetShortName(this ItemGrade grade)
    {
        return grade.ToString();
    }

    /// <summary>
    /// Gets the color hex code for the grade (for UI display).
    /// </summary>
    public static string GetColorHex(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => "#808080",    // Gray - starter
            ItemGrade.D => "#FFFFFF",     // White - basic
            ItemGrade.C => "#00FF00",     // Green - advancing
            ItemGrade.B => "#0099FF",     // Blue - solid
            ItemGrade.A => "#9933FF",     // Purple - high-tier
            ItemGrade.S => "#FF6600",     // Orange - end-game
            _ => "#FFFFFF"
        };
    }

    /// <summary>
    /// Gets the Discord color code (decimal) for embeds.
    /// </summary>
    public static int GetDiscordColor(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => 0x808080,    // Gray
            ItemGrade.D => 0xFFFFFF,     // White
            ItemGrade.C => 0x00FF00,     // Green
            ItemGrade.B => 0x0099FF,     // Blue
            ItemGrade.A => 0x9933FF,     // Purple
            ItemGrade.S => 0xFF6600,     // Orange
            _ => 0xFFFFFF
        };
    }

    /// <summary>
    /// Gets the icon/emoji for the grade.
    /// </summary>
    public static string GetIcon(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => "?",    // White circle - starter
            ItemGrade.D => "??",     // Blue circle - basic
            ItemGrade.C => "??",     // Green circle - advancing
            ItemGrade.B => "??",     // Purple circle - solid
            ItemGrade.A => "??",     // Yellow circle - high-tier
            ItemGrade.S => "??",     // Red circle - end-game
            _ => "?"
        };
    }

    /// <summary>
    /// Gets the level range as a string (e.g., "20-39").
    /// </summary>
    public static string GetLevelRange(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => "1-19",
            ItemGrade.D => "20-39",
            ItemGrade.C => "40-49",
            ItemGrade.B => "50-69",
            ItemGrade.A => "70-84",
            ItemGrade.S => "85+",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Checks if a character of the specified level can equip items of this grade.
    /// </summary>
    public static bool CanEquip(this ItemGrade grade, int characterLevel)
    {
        return characterLevel >= grade.GetRequiredLevel();
    }

    /// <summary>
    /// Gets all available grades.
    /// </summary>
    public static IEnumerable<ItemGrade> GetAllGrades()
    {
        return Enum.GetValues<ItemGrade>();
    }

    /// <summary>
    /// Gets the next grade tier (returns null if already at max).
    /// </summary>
    public static ItemGrade? GetNextGrade(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => ItemGrade.D,
            ItemGrade.D => ItemGrade.C,
            ItemGrade.C => ItemGrade.B,
            ItemGrade.B => ItemGrade.A,
            ItemGrade.A => ItemGrade.S,
            ItemGrade.S => null,
            _ => null
        };
    }

    /// <summary>
    /// Gets the previous grade tier (returns null if already at minimum).
    /// </summary>
    public static ItemGrade? GetPreviousGrade(this ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.NG => null,
            ItemGrade.D => ItemGrade.NG,
            ItemGrade.C => ItemGrade.D,
            ItemGrade.B => ItemGrade.C,
            ItemGrade.A => ItemGrade.B,
            ItemGrade.S => ItemGrade.A,
            _ => null
        };
    }

    /// <summary>
    /// Gets the full display string combining grade and rarity.
    /// Example: "D-Grade Legendary"
    /// </summary>
    public static string GetFullDisplayName(this ItemGrade grade, ItemRarity rarity)
    {
        return $"{grade.GetDisplayName()} {rarity.GetDisplayName()}";
    }

    /// <summary>
    /// Gets a formatted display string for Discord with icons.
    /// Example: "?? D-Grade ? Legendary"
    /// </summary>
    public static string GetDiscordDisplayName(this ItemGrade grade, ItemRarity rarity)
    {
        return $"{grade.GetIcon()} {grade.GetShortName()} {rarity.GetDisplayName()}";
    }
}
