using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a Nightstorm event - a randomly scheduled PvE combat event in a zone.
/// Up to 10 players can register to fight against dynamically spawned monsters.
/// If no players defend, monsters raid the town and treasury loses gold.
/// </summary>
public class NightstormEvent : BaseEntity
{
    /// <summary>
    /// Gets or sets the zone where this event occurs.
    /// </summary>
    public Guid ZoneId { get; set; }

    /// <summary>
    /// Navigation property to the zone.
    /// </summary>
    public Zone Zone { get; set; } = null!;

    /// <summary>
    /// Gets or sets when this event is scheduled to occur.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// Gets or sets the current status of the event.
    /// </summary>
    public EventStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when registration opened for this event.
    /// Null if registration hasn't opened yet.
    /// </summary>
    public DateTime? RegistrationOpenedAt { get; set; }

    /// <summary>
    /// Gets or sets when registration closes.
    /// Event starts after this time or when max participants reached.
    /// </summary>
    public DateTime? RegistrationClosesAt { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of players allowed.
    /// Default: 10 players.
    /// </summary>
    public int MaxParticipants { get; set; } = 10;

    /// <summary>
    /// Gets or sets the players registered for this event.
    /// </summary>
    public ICollection<PlayerState> RegisteredPlayers { get; set; } = new List<PlayerState>();

    /// <summary>
    /// Gets or sets the combat instance for this event.
    /// Created when event starts.
    /// </summary>
    public Guid? CombatInstanceId { get; set; }

    /// <summary>
    /// Navigation property to the combat instance.
    /// </summary>
    public CombatInstance? CombatInstance { get; set; }

    /// <summary>
    /// Gets or sets whether players won the combat.
    /// Null if combat not completed or was raided.
    /// </summary>
    public bool? PlayerVictory { get; set; }

    /// <summary>
    /// Gets or sets when the event completed.
    /// Null if not completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the amount of gold removed from treasury as raid penalty.
    /// Only applies if Status = Raid (no players defended).
    /// </summary>
    public long? RaidPenaltyAmount { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation if Status = Cancelled.
    /// Examples: "Server shutdown", "Event error", "Invalid state"
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets the current number of registered players.
    /// </summary>
    public int RegisteredCount => RegisteredPlayers?.Count ?? 0;

    /// <summary>
    /// Gets whether registration is full.
    /// </summary>
    public bool IsFull => RegisteredCount >= MaxParticipants;

    /// <summary>
    /// Gets whether registration is currently open.
    /// </summary>
    public bool IsRegistrationOpen => 
        Status == EventStatus.Registration && 
        !IsFull && 
        RegistrationClosesAt.HasValue && 
        DateTime.UtcNow < RegistrationClosesAt.Value;

    /// <summary>
    /// Gets whether the event can start (enough players or time expired).
    /// </summary>
    public bool CanStart =>
        Status == EventStatus.Registration &&
        RegisteredCount > 0 &&
        (IsFull || (RegistrationClosesAt.HasValue && DateTime.UtcNow >= RegistrationClosesAt.Value));

    /// <summary>
    /// Gets whether the event should be marked as raided (no defenders when registration closes).
    /// </summary>
    public bool ShouldBeRaided =>
        Status == EventStatus.Registration &&
        RegisteredCount == 0 &&
        RegistrationClosesAt.HasValue &&
        DateTime.UtcNow >= RegistrationClosesAt.Value;
}
