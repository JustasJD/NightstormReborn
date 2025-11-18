namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the available character classes in the game.
/// </summary>
public enum CharacterClass
{
    // Tank & Heavy Melee (The Defenders)
    
    /// <summary>
    /// The Holy Knight; high defense, healing, and burst divine damage (Smites).
    /// </summary>
    Paladin = 1,

    /// <summary>
    /// The Nature Guard; high defense, battlefield control, and large weapon usage.
    /// </summary>
    Warden = 2,

    /// <summary>
    /// The Sacrificial Tank; uses own HP as a resource for powerful defensive and retaliatory attacks.
    /// </summary>
    DarkKnight = 3,

    // Melee DPS & Utility (The Strikers)
    
    /// <summary>
    /// The Fencer; high agility, evasion, and single-target precision/counter-attacks.
    /// </summary>
    Duelist = 4,

    /// <summary>
    /// The Lancer; uses polearms and the signature Jump ability for burst damage and evasion.
    /// </summary>
    Dragoon = 5,

    /// <summary>
    /// The Pugilist; high speed, unarmed combat, and rewarding attack combos/stacks.
    /// </summary>
    Monk = 6,

    /// <summary>
    /// The Assassin; stealth, critical hits via Sneak Attack, and utility like Steal and traps.
    /// </summary>
    Rogue = 7,

    // Arcane & Necrotic Casters (The Magic DPS)
    
    /// <summary>
    /// The Scholar; high versatility, broad spell list, excels at utility and AoE control.
    /// </summary>
    Wizard = 8,

    /// <summary>
    /// The Bloodline Caster; raw magical power, fewer spells but uses Sorcery Points to enhance/modify them.
    /// </summary>
    Sorcerer = 9,

    /// <summary>
    /// The Life-Force Manipulator; uses curses, drain effects, and simple undead servants (no complex summoning).
    /// </summary>
    Necromancer = 10,

    // Divine Casters (The Healers & Support)
    
    /// <summary>
    /// The High Priest; primary healer, status cleanser, and protective divine magic.
    /// </summary>
    Cleric = 11,

    /// <summary>
    /// The Nature Priest; healing, nature-based damage/control, and Wild Shape utility.
    /// </summary>
    Druid = 12,

    // Ranged DPS & Control (The Backline)
    
    /// <summary>
    /// The Hunter; consistent bow/cross-bow damage, tracking, and Favored Foe specialization.
    /// </summary>
    Ranger = 13,

    /// <summary>
    /// The Performer; grants persistent, area-of-effect Songs for party buffs and enemy debuffs.
    /// </summary>
    Bard = 14,

    /// <summary>
    /// The Marksman; focuses on high single-target burst damage and high-risk Trick Shots with firearms.
    /// </summary>
    Gunslinger = 15,

    /// <summary>
    /// The Grenadier; non-magical, focuses on thrown explosives for AoE damage and status effects (Poison, Blind).
    /// </summary>
    Alchemist = 16,
}
