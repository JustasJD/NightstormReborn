using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a single transaction in a zone treasury.
/// Tracks both entry fees (deposits) and guild withdrawals.
/// </summary>
public class TreasuryTransaction : BaseEntity
{
    /// <summary>
    /// Gets or sets the zone treasury this transaction belongs to.
    /// </summary>
    public Guid ZoneTreasuryId { get; set; }

    /// <summary>
    /// Navigation property to the zone treasury.
    /// </summary>
    public ZoneTreasury ZoneTreasury { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of transaction.
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Gets or sets the amount of gold involved.
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// Gets or sets the character involved (for entry fees).
    /// Null for guild withdrawals.
    /// </summary>
    public Guid? CharacterId { get; set; }

    /// <summary>
    /// Navigation property to the character.
    /// </summary>
    public Character? Character { get; set; }

    /// <summary>
    /// Gets or sets the guild involved (for withdrawals).
    /// Null for entry fees.
    /// </summary>
    public Guid? GuildId { get; set; }

    /// <summary>
    /// Navigation property to the guild.
    /// </summary>
    public Guild? Guild { get; set; }

    /// <summary>
    /// Gets or sets a description of the transaction.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the transaction timestamp.
    /// </summary>
    public DateTime TransactionTime => CreatedAt;
}
