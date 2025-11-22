using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a record of player travel between zones.
/// Tracks travel history and entry fees paid.
/// </summary>
public class TravelLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the character who travelled.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Navigation property to the character.
    /// </summary>
    public Character Character { get; set; } = null!;

    /// <summary>
    /// Gets or sets the zone the player travelled from.
    /// </summary>
    public Guid OriginZoneId { get; set; }

    /// <summary>
    /// Navigation property to the origin zone.
    /// </summary>
    public Zone OriginZone { get; set; } = null!;

    /// <summary>
    /// Gets or sets the zone the player is travelling to.
    /// </summary>
    public Guid DestinationZoneId { get; set; }

    /// <summary>
    /// Navigation property to the destination zone.
    /// </summary>
    public Zone DestinationZone { get; set; } = null!;

    /// <summary>
    /// Gets or sets when the travel started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets when the travel completed.
    /// Null if still in progress or cancelled.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets when the travel was cancelled.
    /// Null if not cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the entry fee paid to the destination zone's treasury.
    /// </summary>
    public long EntryFeePaid { get; set; }

    /// <summary>
    /// Gets or sets the travel status.
    /// </summary>
    public TravelStatus Status { get; set; }

    /// <summary>
    /// Gets the travel duration (90 seconds per zone).
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromSeconds(90);

    /// <summary>
    /// Gets when the travel is expected to complete.
    /// </summary>
    public DateTime ExpectedCompletionTime => StartedAt.Add(Duration);

    /// <summary>
    /// Gets whether the travel should be complete now.
    /// </summary>
    public bool ShouldBeComplete => 
        Status == TravelStatus.InProgress && 
        DateTime.UtcNow >= ExpectedCompletionTime;
}
