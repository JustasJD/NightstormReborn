namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the status of a Nightstorm event.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Event is scheduled but registration has not opened yet.
    /// </summary>
    Scheduled = 0,

    /// <summary>
    /// Registration is open for players to join.
    /// </summary>
    Registration = 1,

    /// <summary>
    /// Combat is in progress.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Combat completed - players won and defended the town.
    /// </summary>
    Victory = 3,

    /// <summary>
    /// Combat completed - players lost (all players died).
    /// </summary>
    Defeat = 4,

    /// <summary>
    /// No players registered to defend - monsters raided the town.
    /// Treasury loses gold as penalty.
    /// </summary>
    Raid = 5,

    /// <summary>
    /// Event was cancelled by the system (server shutdown, error, etc).
    /// No penalties applied - non-punishable cancellation.
    /// </summary>
    Cancelled = 6
}
