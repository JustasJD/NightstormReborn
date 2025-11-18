namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the defensive archetype/armor type of a character class.
/// Determines HP/MP scaling and defense value bonuses.
/// </summary>
public enum ArmorType
{
    /// <summary>
    /// Heavy Armor - Tanks and heavy melee warriors.
    /// High HP scaling (CON * 20), low MP scaling (SPR * 10).
    /// Defense bonuses: Heavy Melee +15, Fast Melee +15, Elemental +5, Spiritual +5.
    /// Classes: Paladin, Warden, DarkKnight, Duelist, Dragoon
    /// </summary>
    Heavy = 1,

    /// <summary>
    /// Light Armor - Agile melee and ranged physical classes.
    /// Medium HP scaling (CON * 15), medium MP scaling (SPR * 15).
    /// Defense bonuses: Heavy Melee +5, Fast Melee +10, Elemental +10, Spiritual +5.
    /// Classes: Monk, Rogue, Ranger, Gunslinger, Alchemist
    /// </summary>
    Light = 2,

    /// <summary>
    /// Cloth/Magical - Pure casters with magical wards.
    /// Low HP scaling (CON * 10), high MP scaling (SPR * 20).
    /// Defense bonuses: Heavy Melee +0, Fast Melee +0, Elemental +15, Spiritual +15.
    /// Classes: Wizard, Sorcerer, Necromancer, Cleric, Druid, Bard
    /// </summary>
    Cloth = 3
}
