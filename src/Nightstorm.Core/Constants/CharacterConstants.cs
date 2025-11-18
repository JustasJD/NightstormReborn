namespace Nightstorm.Core.Constants;

/// <summary>
/// Constants for Character entity default values and constraints.
/// </summary>
public static class CharacterConstants
{
    /// <summary>
    /// Starting level for new characters.
    /// </summary>
    public const int DefaultLevel = 1;

    /// <summary>
    /// Starting experience for new characters.
    /// </summary>
    public const long DefaultExperience = 0;

    /// <summary>
    /// Starting gold for new characters.
    /// </summary>
    public const long DefaultGold = 0;

    /// <summary>
    /// Maximum character name length.
    /// </summary>
    public const int MaxNameLength = 80;

    /// <summary>
    /// Minimum character name length.
    /// </summary>
    public const int MinNameLength = 2;

    /// <summary>
    /// Minimum stat value.
    /// </summary>
    public const int MinStatValue = 1;

    /// <summary>
    /// Maximum stat value before modifiers.
    /// </summary>
    public const int MaxBaseStatValue = 100;
}
