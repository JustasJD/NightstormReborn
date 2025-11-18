namespace Nightstorm.Core.Constants;

/// <summary>
/// Constants for Item entity default values and constraints.
/// </summary>
public static class ItemConstants
{
    /// <summary>
    /// Minimum required level to use any item.
    /// </summary>
    public const int MinimumLevel = 1;

    /// <summary>
    /// Default stack size for non-stackable items.
    /// </summary>
    public const int DefaultStackSize = 1;

    /// <summary>
    /// Maximum stack size for stackable items (e.g., potions, materials).
    /// </summary>
    public const int MaxStackSize = 999;

    /// <summary>
    /// Default tradeable status for items.
    /// </summary>
    public const bool DefaultTradeable = true;

    /// <summary>
    /// Maximum item name length.
    /// </summary>
    public const int MaxNameLength = 100;

    /// <summary>
    /// Maximum item description length.
    /// </summary>
    public const int MaxDescriptionLength = 500;
}
