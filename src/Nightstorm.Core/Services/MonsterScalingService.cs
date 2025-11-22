using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Services;

/// <summary>
/// Implementation of monster stat scaling calculations.
/// Formulas ensure proper progression from level 1 to 300.
/// </summary>
public class MonsterScalingService : IMonsterScalingService
{
    // Base formulas:
    // HP = (Level * 50) * HpMultiplier * DifficultyMod
    // Attack = ((Level * 10) + 20) * DamageMultiplier * DifficultyMod
    // Defense = (Level * 5) + BaseDefense + ArmorBonus
    
    /// <inheritdoc/>
    public int CalculateMaxHealth(int level, MonsterDifficulty difficulty, double hpMultiplier)
    {
        // Base HP scales with level
        int baseHp = level * 50;
        
        // Apply difficulty modifier
        double difficultyMod = difficulty switch
        {
            MonsterDifficulty.Normal => 1.0,
            MonsterDifficulty.Boss => 3.0,
            MonsterDifficulty.RaidBoss => 10.0,
            _ => 1.0
        };
        
        // Apply template multiplier and difficulty
        int finalHp = (int)(baseHp * hpMultiplier * difficultyMod);
        
        // Minimum 10 HP
        return Math.Max(10, finalHp);
    }
    
    /// <inheritdoc/>
    public int CalculateAttackPower(int level, MonsterDifficulty difficulty, double damageMultiplier)
    {
        // Base attack power: (Level * 10) + 20
        int baseAttack = (level * 10) + 20;
        
        // Apply difficulty modifier
        double difficultyMod = difficulty switch
        {
            MonsterDifficulty.Normal => 1.0,
            MonsterDifficulty.Boss => 1.5,
            MonsterDifficulty.RaidBoss => 2.0,
            _ => 1.0
        };
        
        // Apply template multiplier and difficulty
        int finalAttack = (int)(baseAttack * damageMultiplier * difficultyMod);
        
        // Minimum 5 attack
        return Math.Max(5, finalAttack);
    }
    
    /// <inheritdoc/>
    public (int HeavyMelee, int FastMelee, int Elemental, int Spiritual) CalculateDefenses(
        int level, ArmorType armorType)
    {
        // Base defense formula: (Level * 5) + 10
        int baseDefense = (level * 5) + 10;
        
        // Armor type modifiers (matches character system)
        var (heavyBonus, fastBonus, elementalBonus, spiritualBonus) = armorType switch
        {
            ArmorType.Heavy => (15, 15, 5, 5),      // Strong vs physical, weak vs magic
            ArmorType.Light => (5, 10, 10, 5),       // Balanced
            ArmorType.Cloth => (0, 0, 15, 15),       // Weak vs physical, strong vs magic
            _ => (0, 0, 0, 0)
        };
        
        return (
            HeavyMelee: baseDefense + heavyBonus,
            FastMelee: baseDefense + fastBonus,
            Elemental: baseDefense + elementalBonus,
            Spiritual: baseDefense + spiritualBonus
        );
    }
    
    /// <inheritdoc/>
    public long CalculateExperienceReward(int level, MonsterDifficulty difficulty, double expMultiplier)
    {
        // Base EXP: Level * 100
        long baseExp = level * 100;
        
        // Apply difficulty modifier
        double difficultyMod = difficulty switch
        {
            MonsterDifficulty.Normal => 1.0,
            MonsterDifficulty.Boss => 5.0,
            MonsterDifficulty.RaidBoss => 20.0,
            _ => 1.0
        };
        
        // Apply template multiplier and difficulty
        long finalExp = (long)(baseExp * expMultiplier * difficultyMod);
        
        // Minimum 10 EXP
        return Math.Max(10, finalExp);
    }
    
    /// <inheritdoc/>
    public int CalculateGoldDrop(int level, MonsterDifficulty difficulty, double goldMultiplier)
    {
        // Base gold: Level * 10
        int baseGold = level * 10;
        
        // Apply difficulty modifier
        double difficultyMod = difficulty switch
        {
            MonsterDifficulty.Normal => 1.0,
            MonsterDifficulty.Boss => 3.0,
            MonsterDifficulty.RaidBoss => 10.0,
            _ => 1.0
        };
        
        // Apply template multiplier and difficulty
        int finalGold = (int)(baseGold * goldMultiplier * difficultyMod);
        
        // Minimum 1 gold
        return Math.Max(1, finalGold);
    }
    
    /// <inheritdoc/>
    public double CalculateDropRate(MonsterDifficulty difficulty)
    {
        return difficulty switch
        {
            MonsterDifficulty.Normal => 0.10,      // 10% drop rate
            MonsterDifficulty.Boss => 0.50,        // 50% drop rate
            MonsterDifficulty.RaidBoss => 1.00,    // 100% drop rate (guaranteed)
            _ => 0.10
        };
    }
}
