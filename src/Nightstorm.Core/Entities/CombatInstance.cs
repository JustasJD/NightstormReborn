using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents an instance of turn-based combat.
/// Tracks participants (players and monsters), turn order, and combat log.
/// </summary>
public class CombatInstance : BaseEntity
{
    /// <summary>
    /// Gets or sets the Nightstorm event this combat belongs to.
    /// </summary>
    public Guid NightstormEventId { get; set; }

    /// <summary>
    /// Navigation property to the Nightstorm event.
    /// </summary>
    public NightstormEvent NightstormEvent { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current combat status.
    /// </summary>
    public CombatStatus Status { get; set; }

    /// <summary>
    /// Gets or sets all participants in this combat.
    /// </summary>
    public ICollection<CombatParticipant> Participants { get; set; } = new List<CombatParticipant>();

    /// <summary>
    /// Gets or sets the current turn number.
    /// </summary>
    public int CurrentTurn { get; set; }

    /// <summary>
    /// Gets or sets the ID of the entity whose turn it currently is.
    /// Can be CharacterId or MonsterId depending on participant type.
    /// </summary>
    public Guid? CurrentActorId { get; set; }

    /// <summary>
    /// Gets or sets the combat log entries.
    /// </summary>
    public ICollection<CombatLogEntry> CombatLog { get; set; } = new List<CombatLogEntry>();

    /// <summary>
    /// Gets or sets whether players won the combat.
    /// Null if combat not completed.
    /// </summary>
    public bool? PlayerVictory { get; set; }

    /// <summary>
    /// Gets or sets when the combat completed.
    /// Null if still in progress.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets the number of alive players.
    /// </summary>
    public int AlivePlayerCount => Participants
        .Where(p => p.Type == ParticipantType.Player && p.IsAlive)
        .Count();

    /// <summary>
    /// Gets the number of alive monsters.
    /// </summary>
    public int AliveMonsterCount => Participants
        .Where(p => p.Type == ParticipantType.Monster && p.IsAlive)
        .Count();

    /// <summary>
    /// Gets whether the combat is finished.
    /// </summary>
    public bool IsFinished => Status == CombatStatus.Victory || Status == CombatStatus.Defeat;

    /// <summary>
    /// Gets whether all players are dead.
    /// </summary>
    public bool AllPlayersDead => AlivePlayerCount == 0;

    /// <summary>
    /// Gets whether all monsters are dead.
    /// </summary>
    public bool AllMonstersDead => AliveMonsterCount == 0;
}
