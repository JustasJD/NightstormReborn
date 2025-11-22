using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a single entry in the combat log.
/// Records all actions taken during combat for replay and display.
/// </summary>
public class CombatLogEntry : BaseEntity
{
    /// <summary>
    /// Gets or sets the combat instance this log entry belongs to.
    /// </summary>
    public Guid CombatInstanceId { get; set; }

    /// <summary>
    /// Navigation property to the combat instance.
    /// </summary>
    public CombatInstance CombatInstance { get; set; } = null!;

    /// <summary>
    /// Gets or sets the turn number when this action occurred.
    /// </summary>
    public int Turn { get; set; }

    /// <summary>
    /// Gets or sets the entity ID of the actor (CharacterId or MonsterId).
    /// </summary>
    public Guid ActorId { get; set; }

    /// <summary>
    /// Gets or sets the name of the actor for display.
    /// </summary>
    public string ActorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of action performed.
    /// </summary>
    public CombatActionType ActionType { get; set; }

    /// <summary>
    /// Gets or sets the target entity ID if applicable.
    /// Null for actions without a target (like area effects).
    /// </summary>
    public Guid? TargetId { get; set; }

    /// <summary>
    /// Gets or sets the name of the target for display.
    /// </summary>
    public string? TargetName { get; set; }

    /// <summary>
    /// Gets or sets the damage dealt (if applicable).
    /// </summary>
    public int? Damage { get; set; }

    /// <summary>
    /// Gets or sets whether this was a critical hit.
    /// </summary>
    public bool? IsCritical { get; set; }

    /// <summary>
    /// Gets or sets whether the attack missed.
    /// </summary>
    public bool? IsMiss { get; set; }

    /// <summary>
    /// Gets or sets the human-readable description of the action.
    /// Example: "Warrior attacks Goblin for 25 damage! (Critical Hit!)"
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the timestamp of when this action occurred.
    /// </summary>
    public DateTime Timestamp => CreatedAt;
}
