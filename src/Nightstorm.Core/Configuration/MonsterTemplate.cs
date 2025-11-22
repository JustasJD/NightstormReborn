using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Configuration;

/// <summary>
/// Represents a static monster template definition.
/// Used to generate Monster instances with appropriate stats for their level.
/// </summary>
public record MonsterTemplate
{
    /// <summary>
    /// Gets the unique identifier for this monster template.
    /// </summary>
    public required string TemplateId { get; init; }
    
    /// <summary>
    /// Gets the display name of the monster.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the monster type category.
    /// </summary>
    public required MonsterType Type { get; init; }
    
    /// <summary>
    /// Gets the difficulty tier (Normal, Boss, RaidBoss).
    /// </summary>
    public required MonsterDifficulty Difficulty { get; init; }
    
    /// <summary>
    /// Gets the base level for this monster.
    /// </summary>
    public required int Level { get; init; }
    
    /// <summary>
    /// Gets the zones where this monster can spawn.
    /// </summary>
    public required ZoneType[] ValidZones { get; init; }
    
    /// <summary>
    /// Gets the attack type this monster uses.
    /// </summary>
    public required AttackType AttackType { get; init; }
    
    /// <summary>
    /// Gets the armor type this monster has.
    /// </summary>
    public required ArmorType ArmorType { get; init; }
    
    /// <summary>
    /// Gets the base HP multiplier (scales with difficulty).
    /// Normal: 1.0, Boss: 3.0, RaidBoss: 10.0
    /// </summary>
    public double HpMultiplier { get; init; } = 1.0;
    
    /// <summary>
    /// Gets the base damage multiplier.
    /// Normal: 1.0, Boss: 1.5, RaidBoss: 2.0
    /// </summary>
    public double DamageMultiplier { get; init; } = 1.0;
    
    /// <summary>
    /// Gets the experience reward multiplier.
    /// Normal: 1.0, Boss: 5.0, RaidBoss: 20.0
    /// </summary>
    public double ExpMultiplier { get; init; } = 1.0;
    
    /// <summary>
    /// Gets the gold drop multiplier.
    /// Normal: 1.0, Boss: 3.0, RaidBoss: 10.0
    /// </summary>
    public double GoldMultiplier { get; init; } = 1.0;
}
