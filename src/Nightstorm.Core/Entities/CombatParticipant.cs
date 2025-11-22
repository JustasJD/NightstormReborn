using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a participant in combat (player or monster).
/// Tracks combat-specific stats separate from base entity stats.
/// </summary>
public class CombatParticipant : BaseEntity
{
    /// <summary>
    /// Gets or sets the combat instance this participant belongs to.
    /// </summary>
    public Guid CombatInstanceId { get; set; }

    /// <summary>
    /// Navigation property to the combat instance.
    /// </summary>
    public CombatInstance CombatInstance { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of participant.
    /// </summary>
    public ParticipantType Type { get; set; }

    /// <summary>
    /// Gets or sets the entity ID (CharacterId or MonsterId).
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Gets or sets the participant's name for display.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current health in combat.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// Gets or sets the maximum health in combat.
    /// </summary>
    public int MaxHealth { get; set; }

    /// <summary>
    /// Gets or sets whether the participant is still alive.
    /// </summary>
    public bool IsAlive { get; set; } = true;

    /// <summary>
    /// Gets or sets the initiative roll for turn order.
    /// Higher initiative goes first.
    /// </summary>
    public int InitiativeRoll { get; set; }

    /// <summary>
    /// Gets or sets the participant's position in turn order.
    /// </summary>
    public int TurnOrder { get; set; }

    /// <summary>
    /// Gets the health percentage (0-100).
    /// </summary>
    public int HealthPercentage => MaxHealth > 0 
        ? (int)((double)CurrentHealth / MaxHealth * 100) 
        : 0;

    /// <summary>
    /// Gets whether the participant is critically wounded (< 25% HP).
    /// </summary>
    public bool IsCritical => HealthPercentage < 25;

    /// <summary>
    /// Navigation property to Character if this is a player.
    /// </summary>
    public Character? Character { get; set; }

    /// <summary>
    /// Navigation property to Monster if this is a monster.
    /// </summary>
    public Monster? Monster { get; set; }
}
