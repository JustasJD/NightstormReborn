namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the result of an attack attempt in combat.
/// </summary>
public enum HitResult
{
    /// <summary>
    /// Complete miss - 0% damage (attack failed to connect).
    /// </summary>
    Miss = 0,
    
    /// <summary>
    /// Mitigated hit - 25% damage (defender parried or blocked the attack).
    /// Only possible for physical melee attacks.
    /// </summary>
    Mitigated = 1,
    
    /// <summary>
    /// Normal hit - 100% damage (standard successful attack).
    /// </summary>
    Hit = 2,
    
    /// <summary>
    /// Critical hit - 200-300% damage (devastating strike).
    /// Cannot be mitigated.
    /// </summary>
    Critical = 3
}
