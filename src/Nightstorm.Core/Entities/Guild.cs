using Nightstorm.Core.Constants;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a guild that characters can join.
/// </summary>
public class Guild : BaseEntity
{
    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the guild tag/abbreviation.
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the guild description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the guild leader's character ID.
    /// </summary>
    public Guid LeaderId { get; set; }

    /// <summary>
    /// Gets or sets the guild level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the guild experience points.
    /// </summary>
    public long Experience { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of members allowed.
    /// </summary>
    public int MaxMembers { get; set; }

    /// <summary>
    /// Gets or sets the guild treasury (gold).
    /// </summary>
    public long Treasury { get; set; }

    /// <summary>
    /// Navigation property to guild members.
    /// </summary>
    public ICollection<Character> Members { get; set; } = new List<Character>();

    public Guild()
    {
        Level = GuildConstants.DefaultLevel;
        Experience = 0;
        MaxMembers = GuildConstants.DefaultMaxMembers;
        Treasury = GuildConstants.DefaultTreasury;
    }
}
