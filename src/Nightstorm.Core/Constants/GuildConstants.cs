namespace Nightstorm.Core.Constants;

/// <summary>
/// Constants for Guild entity default values and constraints.
/// </summary>
public static class GuildConstants
{
    /// <summary>
    /// Default maximum number of members a guild can have.
    /// </summary>
    public const int DefaultMaxMembers = 100;

    /// <summary>
    /// Starting level for a new guild.
    /// </summary>
    public const int DefaultLevel = 1;

    /// <summary>
    /// Starting treasury amount for a new guild.
    /// </summary>
    public const long DefaultTreasury = 0;

    /// <summary>
    /// Maximum guild name length.
    /// </summary>
    public const int MaxNameLength = 50;

    /// <summary>
    /// Maximum guild tag length.
    /// </summary>
    public const int MaxTagLength = 5;

    /// <summary>
    /// Maximum guild description length.
    /// </summary>
    public const int MaxDescriptionLength = 500;
}
