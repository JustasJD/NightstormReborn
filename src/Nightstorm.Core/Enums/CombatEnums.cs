namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the status of a combat instance.
/// </summary>
public enum CombatStatus
{
    /// <summary>
    /// Combat is created but not yet started.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Combat is in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Combat completed - players won.
    /// </summary>
    Victory = 2,

    /// <summary>
    /// Combat completed - players lost (all players dead).
    /// </summary>
    Defeat = 3
}

/// <summary>
/// Represents the type of combat participant.
/// </summary>
public enum ParticipantType
{
    /// <summary>
    /// A player character.
    /// </summary>
    Player = 0,

    /// <summary>
    /// A monster/enemy.
    /// </summary>
    Monster = 1
}
