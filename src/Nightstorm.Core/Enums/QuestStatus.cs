namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the status of a quest.
/// </summary>
public enum QuestStatus
{
    /// <summary>
    /// Quest is available to be accepted.
    /// </summary>
    Available = 1,

    /// <summary>
    /// Quest has been accepted but not completed.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Quest objectives are complete, ready to turn in.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Quest has been turned in and rewards claimed.
    /// </summary>
    TurnedIn = 4,

    /// <summary>
    /// Quest has been abandoned by the player.
    /// </summary>
    Abandoned = 5,

    /// <summary>
    /// Quest has failed due to time limit or other conditions.
    /// </summary>
    Failed = 6
}
