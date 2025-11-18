namespace Nightstorm.Core.Constants;

/// <summary>
/// Constants for Monster entity default values and constraints.
/// </summary>
public static class MonsterConstants
{
    /// <summary>
    /// Default starting level for monsters.
    /// </summary>
    public const int DefaultLevel = 1;

    /// <summary>
    /// Default drop rate for common monsters (10%).
    /// </summary>
    public const double DefaultDropRate = 0.1;

    /// <summary>
    /// Boss monster drop rate (50%).
    /// </summary>
    public const double BossDropRate = 0.5;

    /// <summary>
    /// Minimum drop rate allowed (0%).
    /// </summary>
    public const double MinDropRate = 0.0;

    /// <summary>
    /// Maximum drop rate allowed (100%).
    /// </summary>
    public const double MaxDropRate = 1.0;

    /// <summary>
    /// Maximum monster name length.
    /// </summary>
    public const int MaxNameLength = 100;
}
