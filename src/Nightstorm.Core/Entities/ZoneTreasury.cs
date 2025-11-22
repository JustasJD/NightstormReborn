namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents the treasury of a zone that accumulates entry fees.
/// Guilds that own the zone can withdraw once per day.
/// </summary>
public class ZoneTreasury : BaseEntity
{
    /// <summary>
    /// Gets or sets the zone this treasury belongs to.
    /// </summary>
    public Guid ZoneId { get; set; }

    /// <summary>
    /// Navigation property to the zone.
    /// </summary>
    public Zone Zone { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current gold in the treasury.
    /// </summary>
    public long CurrentGold { get; set; }

    /// <summary>
    /// Gets or sets the total gold collected all-time.
    /// </summary>
    public long TotalCollected { get; set; }

    /// <summary>
    /// Gets or sets the total gold withdrawn all-time.
    /// </summary>
    public long TotalWithdrawn { get; set; }

    /// <summary>
    /// Gets or sets when the last withdrawal occurred.
    /// Null if never withdrawn.
    /// </summary>
    public DateTime? LastWithdrawalAt { get; set; }

    /// <summary>
    /// Gets or sets which guild performed the last withdrawal.
    /// Null if never withdrawn.
    /// </summary>
    public Guid? LastWithdrawnByGuildId { get; set; }

    /// <summary>
    /// Navigation property to the guild that last withdrew.
    /// </summary>
    public Guild? LastWithdrawnByGuild { get; set; }

    /// <summary>
    /// Gets or sets all treasury transactions.
    /// </summary>
    public ICollection<TreasuryTransaction> Transactions { get; set; } = new List<TreasuryTransaction>();

    /// <summary>
    /// Gets whether a withdrawal can be made today.
    /// </summary>
    public bool CanWithdrawToday =>
        !LastWithdrawalAt.HasValue ||
        LastWithdrawalAt.Value.Date < DateTime.UtcNow.Date;

    /// <summary>
    /// Gets when the next withdrawal will be available.
    /// </summary>
    public DateTime? NextWithdrawalAvailableAt
    {
        get
        {
            if (!LastWithdrawalAt.HasValue)
                return null;

            var nextDay = LastWithdrawalAt.Value.Date.AddDays(1);
            return DateTime.UtcNow < nextDay ? nextDay : null;
        }
    }

    /// <summary>
    /// Adds entry fee to the treasury.
    /// </summary>
    public void AddEntryFee(long amount, Guid characterId)
    {
        CurrentGold += amount;
        TotalCollected += amount;
    }

    /// <summary>
    /// Withdraws gold from the treasury.
    /// </summary>
    public void Withdraw(long amount, Guid guildId)
    {
        if (amount > CurrentGold)
            throw new InvalidOperationException("Insufficient funds in treasury");

        if (!CanWithdrawToday)
            throw new InvalidOperationException("Already withdrawn today");

        CurrentGold -= amount;
        TotalWithdrawn += amount;
        LastWithdrawalAt = DateTime.UtcNow;
        LastWithdrawnByGuildId = guildId;
    }
}
