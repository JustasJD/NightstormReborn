using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Services;

/// <summary>
/// Implementation of combat encounter generation.
/// Creates balanced encounters with normal monsters, bosses, and raid bosses.
/// </summary>
public class EncounterService : IEncounterService
{
    private readonly IMonsterGeneratorService _monsterGenerator;
    private readonly Random _random;
    
    public EncounterService(IMonsterGeneratorService monsterGenerator)
    {
        _monsterGenerator = monsterGenerator;
        _random = new Random();
    }
    
    /// <inheritdoc/>
    public Encounter GenerateEncounter(
        int characterLevel,
        ZoneType zone,
        bool isBossEncounter = false,
        bool isRaidEncounter = false)
    {
        // Raid encounters override boss encounters
        if (isRaidEncounter)
        {
            return GenerateRaidEncounterInternal(characterLevel, zone);
        }
        
        if (isBossEncounter)
        {
            return GenerateBossEncounterInternal(characterLevel, zone);
        }
        
        // Generate normal encounter (1-3 monsters)
        return GenerateNormalEncounter(characterLevel, zone);
    }
    
    /// <inheritdoc/>
    public Encounter GenerateCustomEncounter(IEnumerable<string> templateIds, ZoneType zone)
    {
        var monsters = templateIds.Select(id => _monsterGenerator.GenerateMonster(id)).ToList();
        
        if (!monsters.Any())
        {
            throw new ArgumentException("No valid template IDs provided", nameof(templateIds));
        }
        
        var avgLevel = (int)monsters.Average(m => m.Level);
        var hasBoss = monsters.Any(m => m.Difficulty == MonsterDifficulty.Boss);
        var hasRaid = monsters.Any(m => m.Difficulty == MonsterDifficulty.RaidBoss);
        
        var boss = monsters.FirstOrDefault(m => m.Difficulty == MonsterDifficulty.Boss);
        var raidBoss = monsters.FirstOrDefault(m => m.Difficulty == MonsterDifficulty.RaidBoss);
        
        return new Encounter
        {
            Name = hasRaid ? $"Raid: {raidBoss!.Name}" : hasBoss ? $"Boss: {boss!.Name}" : "Custom Encounter",
            Zone = zone,
            RecommendedLevel = avgLevel,
            Monsters = monsters,
            Boss = boss,
            RaidBoss = raidBoss,
            DifficultyRating = CalculateDifficulty(monsters),
            TotalExperience = monsters.Sum(m => m.ExperienceReward),
            TotalGold = monsters.Sum(m => m.GoldDrop)
        };
    }
    
    /// <inheritdoc/>
    public Encounter GenerateBossEncounter(string bossTemplateId, bool includeMinions = false)
    {
        var boss = _monsterGenerator.GenerateMonster(bossTemplateId);
        
        if (boss.Difficulty != MonsterDifficulty.Boss)
        {
            throw new ArgumentException($"Template '{bossTemplateId}' is not a boss", nameof(bossTemplateId));
        }
        
        var monsters = new List<Monster> { boss };
        
        // Add 1-2 minions if requested
        if (includeMinions)
        {
            int minionCount = _random.Next(1, 3);
            for (int i = 0; i < minionCount; i++)
            {
                var minion = _monsterGenerator.GenerateRandomMonster(
                    boss.Level - 5, MonsterDifficulty.Normal, ZoneType.BossLair);
                monsters.Add(minion);
            }
        }
        
        return new Encounter
        {
            Name = $"Boss: {boss.Name}",
            Zone = ZoneType.BossLair,
            RecommendedLevel = boss.Level,
            Monsters = monsters,
            Boss = boss,
            RaidBoss = null,
            DifficultyRating = CalculateDifficulty(monsters),
            TotalExperience = monsters.Sum(m => m.ExperienceReward),
            TotalGold = monsters.Sum(m => m.GoldDrop)
        };
    }
    
    /// <inheritdoc/>
    public Encounter GenerateRaidEncounter(string raidBossTemplateId, bool includeMinions = false)
    {
        var raidBoss = _monsterGenerator.GenerateMonster(raidBossTemplateId);
        
        if (raidBoss.Difficulty != MonsterDifficulty.RaidBoss)
        {
            throw new ArgumentException($"Template '{raidBossTemplateId}' is not a raid boss", nameof(raidBossTemplateId));
        }
        
        var monsters = new List<Monster> { raidBoss };
        
        // Add 2-4 elite minions if requested
        if (includeMinions)
        {
            int minionCount = _random.Next(2, 5);
            for (int i = 0; i < minionCount; i++)
            {
                // Mix of normal and boss-tier minions
                var minionDifficulty = _random.Next(100) < 30 
                    ? MonsterDifficulty.Boss 
                    : MonsterDifficulty.Normal;
                
                var minion = _monsterGenerator.GenerateRandomMonster(
                    raidBoss.Level - 10, minionDifficulty, ZoneType.RaidNexus);
                monsters.Add(minion);
            }
        }
        
        return new Encounter
        {
            Name = $"Raid: {raidBoss.Name}",
            Zone = ZoneType.RaidNexus,
            RecommendedLevel = raidBoss.Level,
            Monsters = monsters,
            Boss = null,
            RaidBoss = raidBoss,
            DifficultyRating = CalculateDifficulty(monsters),
            TotalExperience = monsters.Sum(m => m.ExperienceReward),
            TotalGold = monsters.Sum(m => m.GoldDrop)
        };
    }
    
    #region Private Methods
    
    private Encounter GenerateNormalEncounter(int characterLevel, ZoneType zone)
    {
        // 1-3 normal monsters around character level
        int monsterCount = _random.Next(1, 4);
        var monsters = new List<Monster>();
        
        for (int i = 0; i < monsterCount; i++)
        {
            // Level variance: ±2 levels
            int monsterLevel = characterLevel + _random.Next(-2, 3);
            monsterLevel = Math.Max(1, monsterLevel);
            
            var monster = _monsterGenerator.GenerateRandomMonster(
                monsterLevel, MonsterDifficulty.Normal, zone);
            monsters.Add(monster);
        }
        
        return new Encounter
        {
            Name = $"{monsters.First().Name} Group",
            Zone = zone,
            RecommendedLevel = characterLevel,
            Monsters = monsters,
            Boss = null,
            RaidBoss = null,
            DifficultyRating = CalculateDifficulty(monsters),
            TotalExperience = monsters.Sum(m => m.ExperienceReward),
            TotalGold = monsters.Sum(m => m.GoldDrop)
        };
    }
    
    private Encounter GenerateBossEncounterInternal(int characterLevel, ZoneType zone)
    {
        // Get boss template near character level
        var bossTemplates = _monsterGenerator.GetTemplates(
            minLevel: characterLevel - 5,
            maxLevel: characterLevel + 5,
            difficulty: MonsterDifficulty.Boss,
            zone: null // Bosses are in BossLair, not zone-restricted
        );
        
        if (!bossTemplates.Any())
        {
            // Fall back to any boss within 10 levels
            bossTemplates = _monsterGenerator.GetTemplates(
                minLevel: characterLevel - 10,
                maxLevel: characterLevel + 10,
                difficulty: MonsterDifficulty.Boss,
                zone: null
            );
        }
        
        if (!bossTemplates.Any())
        {
            throw new InvalidOperationException($"No bosses found near level {characterLevel}");
        }
        
        var randomBoss = bossTemplates[_random.Next(bossTemplates.Count)];
        
        // 30% chance to include 1-2 minions
        bool includeMinions = _random.Next(100) < 30;
        
        return GenerateBossEncounter(randomBoss.TemplateId, includeMinions);
    }
    
    private Encounter GenerateRaidEncounterInternal(int characterLevel, ZoneType zone)
    {
        // Get raid boss template near character level
        var raidTemplates = _monsterGenerator.GetTemplates(
            minLevel: characterLevel - 10,
            maxLevel: characterLevel + 10,
            difficulty: MonsterDifficulty.RaidBoss,
            zone: null // Raid bosses are in RaidNexus
        );
        
        if (!raidTemplates.Any())
        {
            throw new InvalidOperationException($"No raid bosses found near level {characterLevel}");
        }
        
        var randomRaid = raidTemplates[_random.Next(raidTemplates.Count)];
        
        // 50% chance to include minions
        bool includeMinions = _random.Next(100) < 50;
        
        return GenerateRaidEncounter(randomRaid.TemplateId, includeMinions);
    }
    
    private int CalculateDifficulty(IEnumerable<Monster> monsters)
    {
        // Simple difficulty rating based on HP, level, and count
        return monsters.Sum(m => 
        {
            int baseRating = m.Level + (m.MaxHealth / 100);
            
            return m.Difficulty switch
            {
                MonsterDifficulty.Normal => baseRating,
                MonsterDifficulty.Boss => baseRating * 3,
                MonsterDifficulty.RaidBoss => baseRating * 10,
                _ => baseRating
            };
        });
    }
    
    #endregion
}
