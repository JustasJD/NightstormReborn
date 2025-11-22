namespace Nightstorm.Core.Enums;

/// <summary>
/// Defines the specific type/category of weapon.
/// Determines weapon behavior, damage type, and animations.
/// </summary>
public enum WeaponType
{
    /// <summary>
    /// One-handed sword (slashing damage).
    /// Can be dual-wielded or used with shield.
    /// Classes: Paladin, DarkKnight, Duelist, Bard
    /// </summary>
    Sword = 0,

    /// <summary>
    /// One-handed axe (slashing/crushing damage).
    /// Can be dual-wielded or used with shield.
    /// Classes: Warden, Warrior
    /// </summary>
    Axe = 1,

    /// <summary>
    /// One-handed mace/hammer (crushing damage).
    /// Effective against armored enemies.
    /// Classes: Paladin, Cleric
    /// </summary>
    Mace = 2,

    /// <summary>
    /// One-handed dagger (piercing damage).
    /// Fast attacks, can be dual-wielded.
    /// Classes: Rogue, Assassin
    /// </summary>
    Dagger = 3,

    /// <summary>
    /// Fist weapons (knuckles, brass knuckles, claws).
    /// Worn on hands, allows unarmed combat with weapon stats.
    /// Classes: Monk (primary)
    /// </summary>
    Knuckles = 4,

    /// <summary>
    /// Two-handed greatsword (high slashing damage).
    /// Requires both hands, higher damage than one-handed.
    /// Classes: Warrior, Paladin, DarkKnight
    /// </summary>
    Greatsword = 10,

    /// <summary>
    /// Two-handed greataxe (high slashing/crushing damage).
    /// Requires both hands, slower but powerful.
    /// Classes: Warden, Warrior
    /// </summary>
    Greataxe = 11,

    /// <summary>
    /// Two-handed warhammer (high crushing damage).
    /// Requires both hands, devastating impact.
    /// Classes: Paladin, Cleric
    /// </summary>
    Warhammer = 12,

    /// <summary>
    /// Two-handed polearm (spear, pike, lance).
    /// Medium-long reach, piercing damage.
    /// Classes: Dragoon (primary), Warden
    /// </summary>
    Spear = 13,

    /// <summary>
    /// Two-handed polearm (halberd, glaive).
    /// Long reach, slashing and piercing damage.
    /// Classes: Dragoon, Warden (primary)
    /// </summary>
    Polearm = 14,

    /// <summary>
    /// One-handed wand (magical damage).
    /// Can be used with orb/tome in off-hand.
    /// Classes: Wizard, Sorcerer, Necromancer
    /// </summary>
    Wand = 20,

    /// <summary>
    /// Two-handed staff (magical damage).
    /// Increases spell power, used by mages.
    /// Classes: Wizard, Sorcerer, Necromancer, Druid
    /// </summary>
    Staff = 25,

    /// <summary>
    /// Two-handed bow (ranged physical damage).
    /// Requires ammunition (arrows).
    /// Classes: Ranger (primary)
    /// </summary>
    Bow = 30,

    /// <summary>
    /// Two-handed crossbow (ranged physical damage).
    /// Higher damage than bow, slower reload.
    /// Classes: Ranger
    /// </summary>
    Crossbow = 31,

    /// <summary>
    /// One-handed pistol (ranged physical damage).
    /// Can be dual-wielded, fast attacks.
    /// Classes: Gunslinger (primary)
    /// </summary>
    Pistol = 35,

    /// <summary>
    /// Two-handed rifle (ranged physical damage).
    /// High damage, long range, precision shots.
    /// Classes: Gunslinger (primary)
    /// </summary>
    Rifle = 36,

    /// <summary>
    /// Two-handed flask thrower.
    /// Launches explosive grenades and alchemical flasks.
    /// Classes: Alchemist (primary)
    /// </summary>
    FlaskThrower = 40,

    /// <summary>
    /// Two-handed lute (magical/support).
    /// Bard's primary weapon for spellcasting and buffs.
    /// Classes: Bard (primary)
    /// </summary>
    Lute = 45,

    /// <summary>
    /// Two-handed harp (magical/support).
    /// Bard's weapon for healing and support spells.
    /// Classes: Bard
    /// </summary>
    Harp = 46,

    /// <summary>
    /// One-handed flute (magical/support).
    /// Bard's weapon for buffs, can use with shield.
    /// Classes: Bard
    /// </summary>
    Flute = 47,

    /// <summary>
    /// Two-handed guitar (magical/support).
    /// Bard's weapon for area-of-effect buffs.
    /// Classes: Bard
    /// </summary>
    Guitar = 48,

    /// <summary>
    /// Shield (defensive item).
    /// Increases armor and block chance.
    /// Goes in off-hand slot.
    /// Classes: Paladin, Warden, Warrior, Cleric
    /// </summary>
    Shield = 50,

    /// <summary>
    /// Magical orb (spell power item).
    /// Held in off-hand with wand, increases spell damage.
    /// Classes: Wizard, Sorcerer, Necromancer
    /// </summary>
    Orb = 51,

    /// <summary>
    /// Spellbook/Tome (spell power item).
    /// Held in off-hand with wand, increases spell utility.
    /// Classes: Wizard, Necromancer
    /// </summary>
    Tome = 52,

    /// <summary>
    /// Talisman/Focus (magical item).
    /// Held in off-hand, provides magical bonuses.
    /// Classes: Druid, Shaman, Cleric
    /// </summary>
    Talisman = 53,

    /// <summary>
    /// Magical Symbol (magical item).
    /// Held in off-hand, provides magical bonuses.
    /// Classes: Wizard, Cleric
    /// </summary>
    Symbol = 54
}
