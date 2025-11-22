namespace Nightstorm.Core.Enums;

/// <summary>
/// Defines the grade/tier of equipment items.
/// Grade determines the level requirement and base power of items.
/// Inspired by Lineage 2's grade system.
/// </summary>
public enum ItemGrade
{
    /// <summary>
    /// No Grade (NG) - Starter equipment (Level 1-19).
    /// Basic items for new characters.
    /// Available in all rarities (Common to Rare).
    /// </summary>
    NG = 0,

    /// <summary>
    /// D-Grade - Early game equipment (Level 20-39).
    /// First significant tier of equipment.
    /// Available in all rarities (Common to Mythic).
    /// </summary>
    D = 1,

    /// <summary>
    /// C-Grade - Mid-early game equipment (Level 40-49).
    /// Second tier of equipment.
    /// Available in all rarities (Common to Mythic).
    /// </summary>
    C = 2,

    /// <summary>
    /// B-Grade - Mid game equipment (Level 50-69).
    /// Third tier of equipment.
    /// Available in all rarities (Common to Mythic).
    /// </summary>
    B = 3,

    /// <summary>
    /// A-Grade - Late game equipment (Level 70-84).
    /// High-tier equipment.
    /// Minimum rarity: Rare (no Common/Uncommon).
    /// </summary>
    A = 4,

    /// <summary>
    /// S-Grade - End-game equipment (Level 85+).
    /// Top-tier equipment.
    /// Minimum rarity: Rare (no Common/Uncommon).
    /// </summary>
    S = 5
}
