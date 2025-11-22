namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the type of treasury transaction.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Entry fee paid by a character entering the zone.
    /// </summary>
    EntryFee = 0,

    /// <summary>
    /// Gold withdrawn by the owning guild.
    /// </summary>
    Withdrawal = 1
}
