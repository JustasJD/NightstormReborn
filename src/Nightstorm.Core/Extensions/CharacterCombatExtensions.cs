using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Extensions;

/// <summary>
/// Extension methods for Character entity to simplify combat stat access.
/// </summary>
public static class CharacterCombatExtensions
{
    /// <summary>
    /// Gets all combat statistics for a character.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="statsService">The character stats service.</param>
    /// <returns>A CombatStats object containing all combat-related statistics.</returns>
    public static CombatStats GetCombatStats(this Character character, ICharacterStatsService statsService)
    {
        return new CombatStats
        {
            MeleeAttackPower = statsService.CalculateMeleeAttackPower(character.Strength, character.Dexterity),
            RangedAttackPower = statsService.CalculateRangedAttackPower(character.Dexterity, character.Strength),
            MagicPower = statsService.CalculateMagicPower(character.Class, character.Intelligence, character.Wisdom, character.Spirit),
            HeavyMeleeDefense = statsService.CalculateHeavyMeleeDefense(character.Class, character.Dexterity, character.Constitution),
            FastMeleeDefense = statsService.CalculateFastMeleeDefense(character.Class, character.Constitution, character.Strength),
            ElementalMagicDefense = statsService.CalculateElementalMagicDefense(character.Class, character.Wisdom, character.Spirit),
            SpiritualMagicDefense = statsService.CalculateSpiritualMagicDefense(character.Class, character.Spirit, character.Wisdom),
            AttackType = statsService.GetAttackType(character.Class),
            IsTank = statsService.IsTankClass(character.Class),
            IsPhysicalArchetype = statsService.IsPhysicalArchetype(character.Class)
        };
    }
}

/// <summary>
/// Value object containing all combat-related statistics for a character.
/// </summary>
public record CombatStats
{
    /// <summary>
    /// Melee Attack Power (STR-based).
    /// </summary>
    public int MeleeAttackPower { get; init; }

    /// <summary>
    /// Ranged Attack Power (DEX-based).
    /// </summary>
    public int RangedAttackPower { get; init; }

    /// <summary>
    /// Magic Power (INT/WIS-based depending on class).
    /// </summary>
    public int MagicPower { get; init; }

    /// <summary>
    /// Defense against Heavy Melee (STR-based) attacks.
    /// </summary>
    public int HeavyMeleeDefense { get; init; }

    /// <summary>
    /// Defense against Fast Melee (DEX-based) attacks.
    /// </summary>
    public int FastMeleeDefense { get; init; }

    /// <summary>
    /// Defense against Elemental Magic (INT-based) attacks.
    /// </summary>
    public int ElementalMagicDefense { get; init; }

    /// <summary>
    /// Defense against Spiritual Magic (WIS/SPR-based) attacks.
    /// </summary>
    public int SpiritualMagicDefense { get; init; }

    /// <summary>
    /// The type of attack this character uses.
    /// </summary>
    public AttackType AttackType { get; init; }

    /// <summary>
    /// Whether this character is a tank class (receives +15 bonus to all defenses).
    /// </summary>
    public bool IsTank { get; init; }

    /// <summary>
    /// Whether this character is a physical archetype (vs caster archetype).
    /// </summary>
    public bool IsPhysicalArchetype { get; init; }

    /// <summary>
    /// Gets the primary attack power based on the character's attack type.
    /// </summary>
    public int PrimaryAttackPower => AttackType switch
    {
        AttackType.HeavyMelee => MeleeAttackPower,
        AttackType.FastMelee => RangedAttackPower,
        AttackType.ElementalMagic => MagicPower,
        AttackType.SpiritualMagic => MagicPower,
        AttackType.MeleeHybrid => Math.Max(MeleeAttackPower, MagicPower),
        AttackType.RangedHybrid => Math.Max(RangedAttackPower, MagicPower),
        _ => MeleeAttackPower
    };

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
