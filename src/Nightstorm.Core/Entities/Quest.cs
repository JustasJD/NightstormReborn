using Nightstorm.Core.Constants;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a quest in the game.
/// </summary>
public class Quest : BaseEntity
{
    /// <summary>
    /// Gets or sets the quest name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quest description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the required level to accept this quest.
    /// </summary>
    public int RequiredLevel { get; set; }

    /// <summary>
    /// Gets or sets the experience reward for completing the quest.
    /// </summary>
    public long ExperienceReward { get; set; }

    /// <summary>
    /// Gets or sets the gold reward for completing the quest.
    /// </summary>
    public int GoldReward { get; set; }

    /// <summary>
    /// Gets or sets the item reward ID (if any).
    /// </summary>
    public Guid? ItemRewardId { get; set; }

    /// <summary>
    /// Navigation property to the item reward.
    /// </summary>
    public Item? ItemReward { get; set; }

    /// <summary>
    /// Gets or sets the quest objective description.
    /// </summary>
    public string ObjectiveDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target objective count (e.g., kill 10 monsters).
    /// </summary>
    public int ObjectiveCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this quest is repeatable.
    /// </summary>
    public bool IsRepeatable { get; set; }

    /// <summary>
    /// Navigation property to character quests.
    /// </summary>
    public ICollection<CharacterQuest> CharacterQuests { get; set; } = new List<CharacterQuest>();

    public Quest()
    {
        RequiredLevel = CharacterConstants.DefaultLevel;
        ObjectiveCount = 1;
        IsRepeatable = false;
    }
}
