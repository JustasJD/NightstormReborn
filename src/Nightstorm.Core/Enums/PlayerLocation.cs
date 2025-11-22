namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the current location state of a player.
/// </summary>
public enum PlayerLocation
{
    /// <summary>
    /// Player is in a zone and can perform actions.
    /// </summary>
    InZone = 0,

    /// <summary>
    /// Player is travelling between zones.
    /// </summary>
    Travelling = 1,

    /// <summary>
    /// Player is participating in combat.
    /// </summary>
    InCombat = 2
}
