namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the type of action performed in combat.
/// </summary>
public enum CombatActionType
{
    /// <summary>
    /// A basic attack action.
    /// </summary>
    Attack = 0,

    /// <summary>
    /// A special skill or ability.
    /// </summary>
    Skill = 1,

    /// <summary>
    /// A healing action.
    /// </summary>
    Heal = 2,

    /// <summary>
    /// A defensive action (reduces incoming damage).
    /// </summary>
    Defend = 3,

    /// <summary>
    /// An attempt to flee from combat.
    /// </summary>
    Flee = 4,

    /// <summary>
    /// A status effect was applied.
    /// </summary>
    StatusEffect = 5,

    /// <summary>
    /// Combat event (turn start, monster spawn, etc).
    /// </summary>
    Event = 6
}
