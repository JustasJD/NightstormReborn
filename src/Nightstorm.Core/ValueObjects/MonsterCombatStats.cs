using Nightstorm.Core.Enums;

namespace Nightstorm.Core.ValueObjects;

/// <summary>
/// Combat statistics for a monster, similar to CombatStats for characters.
/// </summary>
public record MonsterCombatStats
{
    /// <summary>
    /// Gets the monster's attack power.
    /// </summary>
    public int AttackPower { get; init; }
    
    /// <summary>
    /// Gets the type of attack this monster uses.
    /// </summary>
    public AttackType AttackType { get; init; }
    
    /// <summary>
    /// Gets the monster's defense against Heavy Melee (STR-based) attacks.
    /// </summary>
    public int HeavyMeleeDefense { get; init; }
    
    /// <summary>
    /// Gets the monster's defense against Fast Melee (DEX-based) attacks.
    /// </summary>
    public int FastMeleeDefense { get; init; }
    
    /// <summary>
    /// Gets the monster's defense against Elemental Magic (INT-based) attacks.
    /// </summary>
    public int ElementalMagicDefense { get; init; }
    
    /// <summary>
    /// Gets the monster's defense against Spiritual Magic (WIS/SPR-based) attacks.
    /// </summary>
    public int SpiritualMagicDefense { get; init; }
    
    /// <summary>
    /// Gets the monster's level.
    /// </summary>
    public int Level { get; init; }
    
    /// <summary>
    /// Gets the monster's armor type (affects mitigation).
    /// </summary>
    public ArmorType ArmorType { get; init; }
    
    /// <summary>
    /// Gets the defense value against a specific attack type.
    /// </summary>
    /// <param name="attackType">The type of attack to defend against.</param>
    /// <returns>The appropriate defense value.</returns>
    public int GetDefenseAgainst(AttackType attackType)
    {
        return attackType switch
        {
            AttackType.HeavyMelee => HeavyMeleeDefense,
            AttackType.FastMelee => FastMeleeDefense,
            AttackType.ElementalMagic => ElementalMagicDefense,
            AttackType.SpiritualMagic => SpiritualMagicDefense,
            AttackType.MeleeHybrid => Math.Max(HeavyMeleeDefense, SpiritualMagicDefense),
            AttackType.RangedHybrid => Math.Max(FastMeleeDefense, ElementalMagicDefense),
            _ => HeavyMeleeDefense
        };
    }
}
