using Nightstorm.Core.Enums;

namespace Nightstorm.Core.ValueObjects;

/// <summary>
/// Represents the complete result of a combat attack.
/// </summary>
public record AttackResult
{
    /// <summary>
    /// Gets the outcome of the attack (Miss, Mitigated, Hit, Critical).
    /// </summary>
    public HitResult HitResult { get; init; }
    
    /// <summary>
    /// Gets the final damage dealt to the defender.
    /// </summary>
    public int FinalDamage { get; init; }
    
    /// <summary>
    /// Gets the calculated hit chance percentage.
    /// </summary>
    public double HitChance { get; init; }
    
    /// <summary>
    /// Gets the calculated critical chance percentage.
    /// </summary>
    public double CritChance { get; init; }
    
    /// <summary>
    /// Gets the calculated mitigation chance percentage (if applicable).
    /// </summary>
    public double MitigationChance { get; init; }
    
    /// <summary>
    /// Gets whether the attack was super effective (1.15x damage).
    /// </summary>
    public bool WasEffective { get; init; }
    
    /// <summary>
    /// Gets whether the attack was resisted (0.85x damage).
    /// </summary>
    public bool WasResisted { get; init; }
    
    /// <summary>
    /// Gets the detailed damage calculation breakdown.
    /// </summary>
    public DamageCalculation Calculation { get; init; } = null!;
    
    /// <summary>
    /// Gets a human-readable combat log message.
    /// </summary>
    public string CombatLog { get; init; } = string.Empty;
}
