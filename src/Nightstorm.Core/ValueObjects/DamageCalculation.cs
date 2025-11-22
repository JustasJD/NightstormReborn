namespace Nightstorm.Core.ValueObjects;

/// <summary>
/// Detailed breakdown of damage calculation for combat analysis and debugging.
/// </summary>
public record DamageCalculation
{
    /// <summary>
    /// Gets the raw attack power before defense.
    /// </summary>
    public int RawAttackPower { get; init; }
    
    /// <summary>
    /// Gets the defender's defense value.
    /// </summary>
    public int DefenseValue { get; init; }
    
    /// <summary>
    /// Gets the defense reduction percentage (0.0 to 1.0).
    /// </summary>
    public double DefenseReduction { get; init; }
    
    /// <summary>
    /// Gets the damage after defense reduction.
    /// </summary>
    public int DamageAfterDefense { get; init; }
    
    /// <summary>
    /// Gets the bonus damage from stat scaling.
    /// </summary>
    public int StatBonus { get; init; }
    
    /// <summary>
    /// Gets the bonus damage from level scaling.
    /// </summary>
    public int LevelBonus { get; init; }
    
    /// <summary>
    /// Gets the base damage before modifiers.
    /// </summary>
    public int BaseDamage { get; init; }
    
    /// <summary>
    /// Gets the type effectiveness multiplier (0.85, 1.0, or 1.15).
    /// </summary>
    public double TypeEffectiveness { get; init; }
    
    /// <summary>
    /// Gets the level difference modifier.
    /// </summary>
    public double LevelModifier { get; init; }
    
    /// <summary>
    /// Gets the critical hit multiplier (if applicable).
    /// </summary>
    public double CritMultiplier { get; init; }
    
    /// <summary>
    /// Gets the random damage variance (0.85 to 1.0).
    /// </summary>
    public double RandomVariance { get; init; }
    
    /// <summary>
    /// Gets the final calculated damage.
    /// </summary>
    public int FinalDamage { get; init; }
}
