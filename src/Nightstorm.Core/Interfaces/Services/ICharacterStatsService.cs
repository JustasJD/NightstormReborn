namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for calculating character combat statistics and derived values.
/// </summary>
public interface ICharacterStatsService
{
    /// <summary>
    /// Calculates the Melee Attack Power (MAP) for a character.
    /// Formula: (STR * 4) + (DEX * 2) + 10
    /// </summary>
    /// <param name="strength">The character's Strength stat.</param>
    /// <param name="dexterity">The character's Dexterity stat.</param>
    /// <returns>The calculated Melee Attack Power.</returns>
    int CalculateMeleeAttackPower(int strength, int dexterity);

    /// <summary>
    /// Calculates the Ranged Attack Power (RAP) for a character.
    /// Formula: (DEX * 4) + (STR * 2) + 10
    /// </summary>
    /// <param name="dexterity">The character's Dexterity stat.</param>
    /// <param name="strength">The character's Strength stat.</param>
    /// <returns>The calculated Ranged Attack Power.</returns>
    int CalculateRangedAttackPower(int dexterity, int strength);

    /// <summary>
    /// Calculates the Magic Power for a character based on their class.
    /// Formula: (Primary_Caster_Stat * 4) + (SPR * 2) + 10
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="intelligence">The character's Intelligence stat.</param>
    /// <param name="wisdom">The character's Wisdom stat.</param>
    /// <param name="spirit">The character's Spirit stat.</param>
    /// <returns>The calculated Magic Power.</returns>
    int CalculateMagicPower(Enums.CharacterClass characterClass, int intelligence, int wisdom, int spirit);

    /// <summary>
    /// Calculates the maximum health for a character based on their armor type and stats.
    /// Heavy: Base_HP + (CON * 20) + ((STR + DEX) * 3)
    /// Light: Base_HP + (CON * 15) + ((STR + DEX) * 3)
    /// Cloth: Base_HP + (CON * 10) + ((STR + DEX) * 3)
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="constitution">The character's Constitution stat.</param>
    /// <param name="strength">The character's Strength stat.</param>
    /// <param name="dexterity">The character's Dexterity stat.</param>
    /// <returns>The calculated maximum health.</returns>
    int CalculateMaxHealth(Enums.CharacterClass characterClass, int constitution, int strength, int dexterity);

    /// <summary>
    /// Calculates the maximum mana for a character based on their armor type and stats.
    /// Heavy: Base_MP + (SPR * 10) + ((INT + WIS) * 3)
    /// Light: Base_MP + (SPR * 15) + ((INT + WIS) * 3)
    /// Cloth: Base_MP + (SPR * 20) + ((INT + WIS) * 3)
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="spirit">The character's Spirit stat.</param>
    /// <param name="intelligence">The character's Intelligence stat.</param>
    /// <param name="wisdom">The character's Wisdom stat.</param>
    /// <returns>The calculated maximum mana.</returns>
    int CalculateMaxMana(Enums.CharacterClass characterClass, int spirit, int intelligence, int wisdom);

    /// <summary>
    /// Calculates Heavy Melee Defense Value (defends against STR-based attacks).
    /// Formula: Base + (DEX * 2) + (CON * 1) + [Armor Bonus]
    /// Armor Bonus: Heavy +15, Light +5, Cloth +0
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="dexterity">The character's Dexterity stat.</param>
    /// <param name="constitution">The character's Constitution stat.</param>
    /// <returns>The calculated Heavy Melee Defense Value.</returns>
    int CalculateHeavyMeleeDefense(Enums.CharacterClass characterClass, int dexterity, int constitution);

    /// <summary>
    /// Calculates Fast Melee Defense Value (defends against DEX-based attacks).
    /// Formula: Base + (CON * 2) + (STR * 1) + [Armor Bonus]
    /// Armor Bonus: Heavy +15, Light +10, Cloth +0
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="constitution">The character's Constitution stat.</param>
    /// <param name="strength">The character's Strength stat.</param>
    /// <returns>The calculated Fast Melee Defense Value.</returns>
    int CalculateFastMeleeDefense(Enums.CharacterClass characterClass, int constitution, int strength);

    /// <summary>
    /// Calculates Elemental Magic Defense Value (defends against INT-based magic).
    /// Formula: Base + (WIS * 2) + (SPR * 1) + [Armor Bonus]
    /// Armor Bonus: Heavy +5, Light +10, Cloth +15
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="wisdom">The character's Wisdom stat.</param>
    /// <param name="spirit">The character's Spirit stat.</param>
    /// <returns>The calculated Elemental Magic Defense Value.</returns>
    int CalculateElementalMagicDefense(Enums.CharacterClass characterClass, int wisdom, int spirit);

    /// <summary>
    /// Calculates Spiritual Magic Defense Value (defends against WIS/SPR-based magic).
    /// Formula: Base + (SPR * 2) + (WIS * 1) + [Armor Bonus]
    /// Armor Bonus: Heavy +5, Light +5, Cloth +15
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <param name="spirit">The character's Spirit stat.</param>
    /// <param name="wisdom">The character's Wisdom stat.</param>
    /// <returns>The calculated Spiritual Magic Defense Value.</returns>
    int CalculateSpiritualMagicDefense(Enums.CharacterClass characterClass, int spirit, int wisdom);

    /// <summary>
    /// Gets the armor type for a character class.
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <returns>The armor type (Heavy, Light, or Cloth).</returns>
    Enums.ArmorType GetArmorType(Enums.CharacterClass characterClass);

    /// <summary>
    /// Determines if a character class is a Physical archetype (Heavy or Light armor).
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <returns>True if the class is Heavy or Light armor, false if Cloth.</returns>
    bool IsPhysicalArchetype(Enums.CharacterClass characterClass);

    /// <summary>
    /// Determines if a character class is a Tank (Heavy armor, receives maximum physical defense bonuses).
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <returns>True if the class is Heavy armor type.</returns>
    bool IsTankClass(Enums.CharacterClass characterClass);

    /// <summary>
    /// Gets the attack type for a character class based on the combat strategy.
    /// </summary>
    /// <param name="characterClass">The character's class.</param>
    /// <returns>The attack type used by the character class.</returns>
    Enums.AttackType GetAttackType(Enums.CharacterClass characterClass);
}
