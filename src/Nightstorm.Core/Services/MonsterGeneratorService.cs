using Nightstorm.Core.Configuration;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Services;

/// <summary>
/// Implementation of monster generation from templates.
/// Creates fully-stated Monster instances ready for combat.
/// </summary>
public class MonsterGeneratorService : IMonsterGeneratorService
{
    private readonly IMonsterScalingService _scalingService;
    private readonly Random _random;
    
    public MonsterGeneratorService(IMonsterScalingService scalingService)
    {
        _scalingService = scalingService;
        _random = new Random();
    }
    
    /// <inheritdoc/>
    public Monster GenerateMonster(string templateId, int? level = null)
    {
        // Find template
        var template = MonsterTemplateConfiguration.AllTemplates
            .FirstOrDefault(t => t.TemplateId == templateId)
            ?? throw new ArgumentException($"Monster template '{templateId}' not found", nameof(templateId));
        
        // Use provided level or template default
        int monsterLevel = level ?? template.Level;
        
        // Calculate stats using scaling service
        int maxHp = _scalingService.CalculateMaxHealth(
            monsterLevel, template.Difficulty, template.HpMultiplier);
        
        int attackPower = _scalingService.CalculateAttackPower(
            monsterLevel, template.Difficulty, template.DamageMultiplier);
        
        var (heavyDefense, fastDefense, elementalDefense, spiritualDefense) = 
            _scalingService.CalculateDefenses(monsterLevel, template.ArmorType);
        
        long expReward = _scalingService.CalculateExperienceReward(
            monsterLevel, template.Difficulty, template.ExpMultiplier);
        
        int goldDrop = _scalingService.CalculateGoldDrop(
            monsterLevel, template.Difficulty, template.GoldMultiplier);
        
        double dropRate = _scalingService.CalculateDropRate(template.Difficulty);
        
        // Create monster instance
        return new Monster
        {
            Id = Guid.NewGuid(),
            Name = template.Name,
            Type = template.Type,
            Difficulty = template.Difficulty,
            TemplateId = template.TemplateId,
            Level = monsterLevel,
            MaxHealth = maxHp,
            CurrentHealth = maxHp,
            AttackType = template.AttackType,
            AttackPower = attackPower,
            ArmorType = template.ArmorType,
            HeavyMeleeDefense = heavyDefense,
            FastMeleeDefense = fastDefense,
            ElementalMagicDefense = elementalDefense,
            SpiritualMagicDefense = spiritualDefense,
            ExperienceReward = expReward,
            GoldDrop = goldDrop,
            DropRate = dropRate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    /// <inheritdoc/>
    public Monster GenerateRandomMonster(int targetLevel, MonsterDifficulty? difficulty = null, ZoneType? zone = null)
    {
        // Get eligible templates
        var eligibleTemplates = GetTemplates(
            minLevel: targetLevel - 5,
            maxLevel: targetLevel + 5,
            difficulty: difficulty,
            zone: zone
        );
        
        if (!eligibleTemplates.Any())
        {
            throw new InvalidOperationException(
                $"No monsters found for level {targetLevel}, difficulty {difficulty}, zone {zone}");
        }
        
        // Pick random template
        var randomTemplate = eligibleTemplates[_random.Next(eligibleTemplates.Count)];
        
        // Generate with target level
        return GenerateMonster(randomTemplate.TemplateId, targetLevel);
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<MonsterTemplate> GetAllTemplates()
    {
        return MonsterTemplateConfiguration.AllTemplates;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<MonsterTemplate> GetTemplates(
        int? minLevel = null,
        int? maxLevel = null,
        MonsterDifficulty? difficulty = null,
        ZoneType? zone = null)
    {
        var query = MonsterTemplateConfiguration.AllTemplates.AsEnumerable();
        
        // Apply filters
        if (minLevel.HasValue)
            query = query.Where(t => t.Level >= minLevel.Value);
        
        if (maxLevel.HasValue)
            query = query.Where(t => t.Level <= maxLevel.Value);
        
        if (difficulty.HasValue)
            query = query.Where(t => t.Difficulty == difficulty.Value);
        
        if (zone.HasValue)
            query = query.Where(t => t.ValidZones.Contains(zone.Value));
        
        return query.ToList();
    }
}
