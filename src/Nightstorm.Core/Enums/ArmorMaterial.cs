namespace Nightstorm.Core.Enums;

/// <summary>
/// Defines the material/quality of armor equipment pieces.
/// Determines base defense values and appearance.
/// </summary>
public enum ArmorMaterial
{
    /// <summary>
    /// Cloth material (lowest defense).
    /// Worn by mages and spellcasters.
    /// Light, comfortable, enchantable.
    /// </summary>
    Cloth = 0,

    /// <summary>
    /// Leather material (light defense).
    /// Worn by rogues, rangers, and scouts.
    /// Flexible, quiet, good mobility.
    /// </summary>
    Leather = 1,

    /// <summary>
    /// Chainmail material (medium defense).
    /// Worn by hunters and hybrid classes.
    /// Good protection with decent mobility.
    /// </summary>
    Chain = 2,

    /// <summary>
    /// Plate material (highest defense).
    /// Worn by warriors and paladins.
    /// Heavy protection, reduced mobility.
    /// </summary>
    Plate = 3,

    /// <summary>
    /// Dragonscale material (special high-tier).
    /// Rare material with exceptional properties.
    /// High defense with magical resistance.
    /// </summary>
    Dragonscale = 4
}
