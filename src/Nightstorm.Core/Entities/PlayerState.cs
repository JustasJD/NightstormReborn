using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents the current state of a player in the game world.
/// Tracks location, travel status, and combat participation.
/// </summary>
public class PlayerState : BaseEntity
{
    /// <summary>
    /// Gets or sets the character ID this state belongs to.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Navigation property to the character.
    /// </summary>
    public Character Character { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current player location state.
    /// </summary>
    public PlayerLocation Location { get; set; }

    /// <summary>
    /// Gets or sets the current zone the player is in.
    /// Null if travelling or in combat in a different location.
    /// </summary>
    public Guid CurrentZoneId { get; set; }

    /// <summary>
    /// Navigation property to the current zone.
    /// </summary>
    public Zone CurrentZone { get; set; } = null!;

    /// <summary>
    /// Gets or sets the destination zone if player is travelling.
    /// Null if not travelling.
    /// </summary>
    public Guid? DestinationZoneId { get; set; }

    /// <summary>
    /// Navigation property to the destination zone.
    /// </summary>
    public Zone? DestinationZone { get; set; }

    /// <summary>
    /// Gets or sets when the player started travelling.
    /// Null if not travelling.
    /// </summary>
    public DateTime? TravelStartedAt { get; set; }

    /// <summary>
    /// Gets or sets when the travel will complete.
    /// Null if not travelling.
    /// </summary>
    public DateTime? TravelEndsAt { get; set; }

    /// <summary>
    /// Gets or sets the current combat instance the player is in.
    /// Null if not in combat.
    /// </summary>
    public Guid? CurrentCombatId { get; set; }

    /// <summary>
    /// Navigation property to the current combat.
    /// </summary>
    public CombatInstance? CurrentCombat { get; set; }

    /// <summary>
    /// Gets or sets the Nightstorm event the player is registered for.
    /// Null if not registered for any event.
    /// </summary>
    public Guid? RegisteredEventId { get; set; }

    /// <summary>
    /// Navigation property to the registered event.
    /// </summary>
    public NightstormEvent? RegisteredEvent { get; set; }

    /// <summary>
    /// Gets whether the player is currently travelling.
    /// </summary>
    public bool IsTravelling => Location == PlayerLocation.Travelling;

    /// <summary>
    /// Gets whether the player is currently in combat.
    /// </summary>
    public bool IsInCombat => Location == PlayerLocation.InCombat;

    /// <summary>
    /// Gets whether the player can perform actions (not travelling or in combat).
    /// </summary>
    public bool CanAct => Location == PlayerLocation.InZone;

    /// <summary>
    /// Gets the remaining travel time if travelling.
    /// </summary>
    public TimeSpan? RemainingTravelTime
    {
        get
        {
            if (!IsTravelling || !TravelEndsAt.HasValue)
                return null;

            var remaining = TravelEndsAt.Value - DateTime.UtcNow;
            return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        }
    }
}
