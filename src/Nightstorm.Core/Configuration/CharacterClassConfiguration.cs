using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Configuration;

/// <summary>
/// Configuration for character class base statistics.
/// Provides a centralized, data-driven approach to class stat initialization.
/// </summary>
public static class CharacterClassConfiguration
{
    /// <summary>
    /// Base statistics for each character class.
    /// </summary>
    public static readonly IReadOnlyDictionary<CharacterClass, CharacterBaseStats> BaseStats =
        new Dictionary<CharacterClass, CharacterBaseStats>
        {
            // Tank & Heavy Melee (The Defenders) - Physical Archetype
            [CharacterClass.Paladin] = new(Strength: 14, Dexterity: 10, Constitution: 14, Intelligence: 8, Wisdom: 12, Spirit: 12, Luck: 10),
            [CharacterClass.Warden] = new(Strength: 14, Dexterity: 12, Constitution: 14, Intelligence: 8, Wisdom: 10, Spirit: 12, Luck: 10),
            [CharacterClass.DarkKnight] = new(Strength: 14, Dexterity: 10, Constitution: 16, Intelligence: 8, Wisdom: 8, Spirit: 12, Luck: 12),

            // Melee DPS & Utility (The Strikers) - Physical Archetype
            [CharacterClass.Duelist] = new(Strength: 10, Dexterity: 16, Constitution: 10, Intelligence: 8, Wisdom: 8, Spirit: 10, Luck: 18),
            [CharacterClass.Dragoon] = new(Strength: 14, Dexterity: 14, Constitution: 12, Intelligence: 8, Wisdom: 8, Spirit: 12, Luck: 12),
            [CharacterClass.Monk] = new(Strength: 8, Dexterity: 16, Constitution: 14, Intelligence: 8, Wisdom: 10, Spirit: 14, Luck: 10),
            [CharacterClass.Rogue] = new(Strength: 8, Dexterity: 16, Constitution: 10, Intelligence: 10, Wisdom: 8, Spirit: 12, Luck: 16),

            // Arcane & Necrotic Casters (The Magic DPS) - Caster Archetype
            [CharacterClass.Wizard] = new(Strength: 8, Dexterity: 10, Constitution: 10, Intelligence: 18, Wisdom: 8, Spirit: 14, Luck: 12),
            [CharacterClass.Sorcerer] = new(Strength: 8, Dexterity: 10, Constitution: 12, Intelligence: 16, Wisdom: 8, Spirit: 14, Luck: 12),
            [CharacterClass.Necromancer] = new(Strength: 8, Dexterity: 10, Constitution: 12, Intelligence: 16, Wisdom: 10, Spirit: 12, Luck: 12),

            // Divine Casters (The Healers & Support) - Caster Archetype
            [CharacterClass.Cleric] = new(Strength: 10, Dexterity: 10, Constitution: 12, Intelligence: 8, Wisdom: 16, Spirit: 14, Luck: 10),
            [CharacterClass.Druid] = new(Strength: 8, Dexterity: 12, Constitution: 12, Intelligence: 10, Wisdom: 16, Spirit: 12, Luck: 10),

            // Ranged DPS & Control (The Backline) - Physical Archetype
            [CharacterClass.Ranger] = new(Strength: 12, Dexterity: 16, Constitution: 12, Intelligence: 8, Wisdom: 12, Spirit: 10, Luck: 10),
            [CharacterClass.Bard] = new(Strength: 8, Dexterity: 12, Constitution: 10, Intelligence: 12, Wisdom: 8, Spirit: 16, Luck: 14),
            [CharacterClass.Gunslinger] = new(Strength: 10, Dexterity: 16, Constitution: 10, Intelligence: 8, Wisdom: 8, Spirit: 10, Luck: 18),
            [CharacterClass.Alchemist] = new(Strength: 8, Dexterity: 14, Constitution: 10, Intelligence: 16, Wisdom: 8, Spirit: 10, Luck: 14),
        };

    /// <summary>
    /// Default balanced stats for unknown or new classes.
    /// </summary>
    public static readonly CharacterBaseStats DefaultStats = new(
        Strength: 10, Dexterity: 10, Constitution: 10, Intelligence: 10, Wisdom: 10, Spirit: 10, Luck: 10);
}

/// <summary>
/// Record representing base statistics for a character class.
/// </summary>
/// <param name="Strength">Strength stat (Physical Might).</param>
/// <param name="Dexterity">Dexterity stat (Agility, Speed, Accuracy).</param>
/// <param name="Constitution">Constitution stat (Physical Endurance & Health).</param>
/// <param name="Intelligence">Intelligence stat (Knowledge & Arcane Power).</param>
/// <param name="Wisdom">Wisdom stat (Magical Endurance).</param>
/// <param name="Spirit">Spirit stat (Mental Fortitude & Mana Amount).</param>
/// <param name="Luck">Luck stat (Fortune & Drop Chance).</param>
public record CharacterBaseStats(
    int Strength,
    int Dexterity,
    int Constitution,
    int Intelligence,
    int Wisdom,
    int Spirit,
    int Luck);
