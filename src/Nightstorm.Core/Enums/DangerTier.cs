namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the danger tier of a zone (columns-based progression).
/// </summary>
public enum DangerTier
{
    /// <summary>
    /// Safe zones with towns and cities (Columns 1-3).
    /// No PvP, beginner-friendly.
    /// </summary>
    Civilized = 1,
    
    /// <summary>
    /// Wilderness zones with monsters (Columns 4-6).
    /// PvP enabled, where most gameplay happens.
    /// </summary>
    Wilderness = 2,
    
    /// <summary>
    /// Cursed lands with raid-level content (Columns 7-9).
    /// PvP enabled, endgame zones.
    /// </summary>
    Ruined = 3
}
