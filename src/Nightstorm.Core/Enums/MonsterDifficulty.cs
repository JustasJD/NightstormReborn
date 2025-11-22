namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the difficulty tier of a monster encounter.
/// </summary>
public enum MonsterDifficulty
{
    /// <summary>
    /// Standard monster - can appear in groups of 1-3.
    /// Moderate challenge for solo players.
    /// </summary>
    Normal = 1,
    
    /// <summary>
    /// Boss monster - typically appears alone or with minions.
    /// Significantly stronger than normal monsters, requires strategy.
    /// Drops better loot and gives more experience.
    /// </summary>
    Boss = 2,
    
    /// <summary>
    /// Raid boss - endgame content requiring groups or high-level characters.
    /// Extremely powerful with massive HP pools and special mechanics.
    /// Drops legendary loot and massive experience.
    /// </summary>
    RaidBoss = 3
}
