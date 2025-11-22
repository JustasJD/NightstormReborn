using Nightstorm.Core.Entities;
using Nightstorm.Core.ValueObjects;

namespace Nightstorm.Core.Extensions;

/// <summary>
/// Extension methods for Monster entity to provide combat statistics.
/// </summary>
public static class MonsterCombatExtensions
{
    /// <summary>
    /// Gets all combat statistics for a monster.
    /// </summary>
    /// <param name="monster">The monster.</param>
    /// <returns>A MonsterCombatStats object containing all combat-related statistics.</returns>
    public static MonsterCombatStats GetCombatStats(this Monster monster)
    {
        return new MonsterCombatStats
        {
            AttackPower = monster.AttackPower,
            AttackType = monster.AttackType,
            HeavyMeleeDefense = monster.HeavyMeleeDefense,
            FastMeleeDefense = monster.FastMeleeDefense,
            ElementalMagicDefense = monster.ElementalMagicDefense,
            SpiritualMagicDefense = monster.SpiritualMagicDefense,
            Level = monster.Level,
            ArmorType = monster.ArmorType
        };
    }
}
