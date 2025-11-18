using Nightstorm.Core.Entities;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Extensions;

/// <summary>
/// Extension methods for Character entity to work with ICharacterStatsService.
/// </summary>
public static class CharacterExtensions
{
    /// <summary>
    /// Calculates and returns the Melee Attack Power for this character.
    /// </summary>
    public static int GetMeleeAttackPower(this Character character, ICharacterStatsService statsService)
    {
        return statsService.CalculateMeleeAttackPower(character.Strength, character.Dexterity);
    }

    /// <summary>
    /// Calculates and returns the Ranged Attack Power for this character.
    /// </summary>
    public static int GetRangedAttackPower(this Character character, ICharacterStatsService statsService)
    {
        return statsService.CalculateRangedAttackPower(character.Dexterity, character.Strength);
    }

    /// <summary>
    /// Calculates and returns the Magic Power for this character.
    /// </summary>
    public static int GetMagicPower(this Character character, ICharacterStatsService statsService)
    {
        return statsService.CalculateMagicPower(character.Class, character.Intelligence, character.Wisdom, character.Spirit);
    }

    /// <summary>
    /// Recalculates and updates the character's MaxHealth based on current stats.
    /// </summary>
    public static void RecalculateMaxHealth(this Character character, ICharacterStatsService statsService)
    {
        character.MaxHealth = statsService.CalculateMaxHealth(
            character.Class,
            character.Constitution,
            character.Strength,
            character.Dexterity
        );
    }

    /// <summary>
    /// Recalculates and updates the character's MaxMana based on current stats.
    /// </summary>
    public static void RecalculateMaxMana(this Character character, ICharacterStatsService statsService)
    {
        character.MaxMana = statsService.CalculateMaxMana(
            character.Class,
            character.Spirit,
            character.Intelligence,
            character.Wisdom
        );
    }
}
