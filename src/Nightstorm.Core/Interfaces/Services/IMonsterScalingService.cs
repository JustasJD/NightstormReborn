using Nightstorm.Core.Configuration;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for scaling monster stats based on level and difficulty.
/// </summary>
public interface IMonsterScalingService
{
    /// <summary>
    /// Calculates the maximum HP for a monster.
    /// </summary>
    /// <param name="level">Monster level.</param>
    /// <param name="difficulty">Monster difficulty tier.</param>
    /// <param name="hpMultiplier">Template-specific HP multiplier.</param>
    /// <returns>Calculated max HP.</returns>
    int CalculateMaxHealth(int level, MonsterDifficulty difficulty, double hpMultiplier);
    
    /// <summary>
    /// Calculates the attack power for a monster.
    /// </summary>
    /// <param name="level">Monster level.</param>
    /// <param name="difficulty">Monster difficulty tier.</param>
    /// <param name="damageMultiplier">Template-specific damage multiplier.</param>
    /// <returns>Calculated attack power.</returns>
    int CalculateAttackPower(int level, MonsterDifficulty difficulty, double damageMultiplier);
    
    /// <summary>
    /// Calculates defense values for a monster based on attack type.
    /// </summary>
    /// <param name="level">Monster level.</param>
    /// <param name="armorType">Monster armor type.</param>
    /// <returns>Tuple of (HeavyMelee, FastMelee, Elemental, Spiritual) defense values.</returns>
    (int HeavyMelee, int FastMelee, int Elemental, int Spiritual) CalculateDefenses(int level, ArmorType armorType);
    
    /// <summary>
    /// Calculates experience reward for defeating a monster.
    /// </summary>
    /// <param name="level">Monster level.</param>
    /// <param name="difficulty">Monster difficulty tier.</param>
    /// <param name="expMultiplier">Template-specific EXP multiplier.</param>
    /// <returns>Experience reward amount.</returns>
    long CalculateExperienceReward(int level, MonsterDifficulty difficulty, double expMultiplier);
    
    /// <summary>
    /// Calculates gold drop for defeating a monster.
    /// </summary>
    /// <param name="level">Monster level.</param>
    /// <param name="difficulty">Monster difficulty tier.</param>
    /// <param name="goldMultiplier">Template-specific gold multiplier.</param>
    /// <returns>Gold drop amount.</returns>
    int CalculateGoldDrop(int level, MonsterDifficulty difficulty, double goldMultiplier);
    
    /// <summary>
    /// Calculates drop rate based on difficulty.
    /// </summary>
    /// <param name="difficulty">Monster difficulty tier.</param>
    /// <returns>Drop rate percentage (0.0 to 1.0).</returns>
    double CalculateDropRate(MonsterDifficulty difficulty);
}
