namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the attack type used by a character class in the four-quadrant combat system.
/// </summary>
public enum AttackType
{
    /// <summary>
    /// Heavy Melee - STR-based attacks that deal crushing damage.
    /// Countered by DEX Defense. Weak against Elemental Magic. Strong against Spiritual Magic.
    /// </summary>
    HeavyMelee = 1,

    /// <summary>
    /// Fast Melee - DEX-based attacks that deal piercing damage.
    /// Countered by CON Defense. Weak against Elemental Magic. Strong against Fast Melee classes.
    /// </summary>
    FastMelee = 2,

    /// <summary>
    /// Elemental Magic - INT/WIS-based attacks that deal force damage.
    /// Countered by WIS Defense. Weak against Fast Melee. Strong against Heavy Melee.
    /// </summary>
    ElementalMagic = 3,

    /// <summary>
    /// Spiritual Magic - WIS/SPR-based attacks that deal drain/holy damage.
    /// Countered by SPR Defense. Weak against Heavy Melee. Strong against Fast Melee.
    /// </summary>
    SpiritualMagic = 4,

    /// <summary>
    /// Melee Hybrid - Combination of Heavy Melee and Spiritual Magic (e.g., Paladin's Holy Smite).
    /// Uses both physical and magical properties.
    /// </summary>
    MeleeHybrid = 5,

    /// <summary>
    /// Ranged Hybrid - Combination of Ranged Physical and Elemental effects (e.g., Alchemist's Explosive Grenades).
    /// Uses DEX for accuracy and has elemental damage components.
    /// </summary>
    RangedHybrid = 6
}
