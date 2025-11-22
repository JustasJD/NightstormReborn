namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the status of a travel action.
/// </summary>
public enum TravelStatus
{
    /// <summary>
    /// Travel is currently in progress.
    /// </summary>
    InProgress = 0,

    /// <summary>
    /// Travel completed successfully.
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Travel was cancelled by player or system.
    /// </summary>
    Cancelled = 2
}
