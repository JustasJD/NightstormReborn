using Nightstorm.Core.Constants;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a monster in the game.
/// </summary>
public class Monster : BaseEntity
{
    /// <summary>
    /// Gets or sets the monster name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the monster type.
    /// </summary>
    public MonsterType Type { get; set; }

    /// <summary>
    /// Gets or sets the monster level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the maximum health points.
    /// </summary>
    public int MaxHealth { get; set; }

    /// <summary>
    /// Gets or sets the attack type used by this monster.
    /// </summary>
    public AttackType AttackType { get; set; }

    /// <summary>
    /// Gets or sets the attack power.
    /// </summary>
    public int AttackPower { get; set; }

    /// <summary>
    /// Gets or sets the armor type (determines defense bonuses).
    /// </summary>
    public ArmorType ArmorType { get; set; }

    /// <summary>
    /// Gets or sets the Heavy Melee defense value (defends against STR-based attacks).
    /// </summary>
    public int HeavyMeleeDefense { get; set; }

    /// <summary>
    /// Gets or sets the Fast Melee defense value (defends against DEX-based attacks).
    /// </summary>
    public int FastMeleeDefense { get; set; }

    /// <summary>
    /// Gets or sets the Elemental Magic defense value (defends against INT-based magic).
    /// </summary>
    public int ElementalMagicDefense { get; set; }

    /// <summary>
    /// Gets or sets the Spiritual Magic defense value (defends against WIS/SPR-based magic).
    /// </summary>
    public int SpiritualMagicDefense { get; set; }

    /// <summary>
    /// Gets or sets the experience awarded upon defeat.
    /// </summary>
    public long ExperienceReward { get; set; }

    /// <summary>
    /// Gets or sets the gold dropped upon defeat.
    /// </summary>
    public int GoldDrop { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a boss monster.
    /// </summary>
    public bool IsBoss { get; set; }

    /// <summary>
    /// Gets or sets the drop rate multiplier for items (0.0 to 1.0).
    /// </summary>
    public double DropRate { get; set; }

    public Monster()
    {
        Level = MonsterConstants.DefaultLevel;
        IsBoss = false;
        DropRate = MonsterConstants.DefaultDropRate;
        AttackType = AttackType.HeavyMelee;
        ArmorType = ArmorType.Heavy;
    }
}
