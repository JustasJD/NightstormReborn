using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.ValueObjects;

namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for combat calculations and attack resolution.
/// </summary>
public interface ICombatService
{
    /// <summary>
    /// Calculates the result of a character attacking a monster.
    /// </summary>
    /// <param name="attacker">The attacking character.</param>
    /// <param name="defender">The defending monster.</param>
    /// <returns>The complete attack result including damage and combat log.</returns>
    AttackResult CalculateAttack(Character attacker, Monster defender);
    
    /// <summary>
    /// Calculates the result of a monster attacking a character.
    /// </summary>
    /// <param name="attacker">The attacking monster.</param>
    /// <param name="defender">The defending character.</param>
    /// <returns>The complete attack result including damage and combat log.</returns>
    AttackResult CalculateAttack(Monster attacker, Character defender);
    
    /// <summary>
    /// Calculates the hit chance for an attack.
    /// </summary>
    /// <param name="attackerDex">Attacker's Dexterity.</param>
    /// <param name="defenderDex">Defender's Dexterity.</param>
    /// <param name="attackerLevel">Attacker's level.</param>
    /// <param name="defenderLevel">Defender's level.</param>
    /// <param name="attackerWisdom">Attacker's Wisdom.</param>
    /// <param name="attackType">Type of attack being made.</param>
    /// <param name="defenderArmor">Defender's armor type.</param>
    /// <param name="isPhysical">Whether the attack is physical.</param>
    /// <returns>Hit chance as a percentage (0-100).</returns>
    double CalculateHitChance(
        int attackerDex,
        int defenderDex,
        int attackerLevel,
        int defenderLevel,
        int attackerWisdom,
        AttackType attackType,
        ArmorType defenderArmor,
        bool isPhysical);
    
    /// <summary>
    /// Calculates the critical hit chance for an attacker.
    /// </summary>
    /// <param name="luck">Attacker's Luck stat.</param>
    /// <param name="dexterity">Attacker's Dexterity stat.</param>
    /// <param name="characterClass">Attacker's character class.</param>
    /// <param name="isPhysical">Whether the attacker is a physical archetype.</param>
    /// <returns>Critical chance as a percentage (0-100).</returns>
    double CalculateCritChance(
        int luck,
        int dexterity,
        CharacterClass characterClass,
        bool isPhysical);
    
    /// <summary>
    /// Calculates the mitigation chance for a defender (parry/block).
    /// </summary>
    /// <param name="defender">The defending character.</param>
    /// <returns>Mitigation chance as a percentage (0-100).</returns>
    double CalculateMitigationChance(Character defender);
    
    /// <summary>
    /// Calculates the type effectiveness multiplier.
    /// </summary>
    /// <param name="attackType">The attacker's attack type.</param>
    /// <param name="defenderType">The defender's attack type (for determining weakness).</param>
    /// <returns>Effectiveness multiplier (0.85, 1.0, or 1.15).</returns>
    double CalculateTypeEffectiveness(AttackType attackType, AttackType defenderType);
}
