using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents the many-to-many relationship between characters and quests.
/// </summary>
public class CharacterQuest : BaseEntity
{
    /// <summary>
    /// Gets or sets the character ID.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Navigation property to the character.
    /// </summary>
    public Character Character { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quest ID.
    /// </summary>
    public Guid QuestId { get; set; }

    /// <summary>
    /// Navigation property to the quest.
    /// </summary>
    public Quest Quest { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quest status.
    /// </summary>
    public QuestStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the current progress count.
    /// </summary>
    public int CurrentProgress { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the quest was accepted.
    /// </summary>
    public DateTime AcceptedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the quest was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    public CharacterQuest()
    {
        Status = QuestStatus.InProgress;
        CurrentProgress = 0;
        AcceptedAt = DateTime.UtcNow;
    }
}
