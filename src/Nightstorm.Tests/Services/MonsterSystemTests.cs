using Nightstorm.Core.Enums;
using Nightstorm.Core.Services;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Tests.Services;

/// <summary>
/// Tests for monster generation and encounter system.
/// </summary>
[TestFixture]
public class MonsterSystemTests
{
    private MonsterScalingService _scalingService = null!;
    private MonsterGeneratorService _generatorService = null!;
    private EncounterService _encounterService = null!;

    [SetUp]
    public void Setup()
    {
        _scalingService = new MonsterScalingService();
        _generatorService = new MonsterGeneratorService(_scalingService);
        _encounterService = new EncounterService(_generatorService);
    }

    [Test]
    public void MonsterScaling_Level1Goblin_HasCorrectStats()
    {
        // Arrange & Act
        int hp = _scalingService.CalculateMaxHealth(1, MonsterDifficulty.Normal, 1.0);
        int attack = _scalingService.CalculateAttackPower(1, MonsterDifficulty.Normal, 1.0);
        long exp = _scalingService.CalculateExperienceReward(1, MonsterDifficulty.Normal, 1.0);
        int gold = _scalingService.CalculateGoldDrop(1, MonsterDifficulty.Normal, 1.0);

        // Assert
        Assert.That(hp, Is.EqualTo(50), "Level 1 normal monster should have 50 HP");
        Assert.That(attack, Is.EqualTo(30), "Level 1 normal monster should have 30 attack");
        Assert.That(exp, Is.EqualTo(100), "Level 1 normal monster should give 100 EXP");
        Assert.That(gold, Is.EqualTo(10), "Level 1 normal monster should drop 10 gold");
    }

    [Test]
    public void MonsterScaling_Level50Boss_HasTripleHp()
    {
        // Arrange & Act
        int normalHp = _scalingService.CalculateMaxHealth(50, MonsterDifficulty.Normal, 1.0);
        int bossHp = _scalingService.CalculateMaxHealth(50, MonsterDifficulty.Boss, 1.0);
        int raidHp = _scalingService.CalculateMaxHealth(50, MonsterDifficulty.RaidBoss, 1.0);

        // Assert
        Assert.That(bossHp, Is.EqualTo(normalHp * 3), "Boss should have 3x HP");
        Assert.That(raidHp, Is.EqualTo(normalHp * 10), "Raid boss should have 10x HP");
    }

    [Test]
    public void MonsterGenerator_GenerateSpecificMonster_ReturnsCorrectMonster()
    {
        // Arrange & Act
        var goblin = _generatorService.GenerateMonster("cave-goblin");

        // Assert
        Assert.That(goblin.Name, Is.EqualTo("Cave Goblin"));
        Assert.That(goblin.Type, Is.EqualTo(MonsterType.Humanoid));
        Assert.That(goblin.Difficulty, Is.EqualTo(MonsterDifficulty.Normal));
        Assert.That(goblin.Level, Is.EqualTo(1));
        Assert.That(goblin.MaxHealth, Is.GreaterThan(0));
        Assert.That(goblin.AttackPower, Is.GreaterThan(0));
    }

    [Test]
    public void MonsterGenerator_GenerateWithLevelOverride_UsesProvidedLevel()
    {
        // Arrange & Act
        var goblin = _generatorService.GenerateMonster("cave-goblin", level: 10);

        // Assert
        Assert.That(goblin.Level, Is.EqualTo(10));
        Assert.That(goblin.MaxHealth, Is.GreaterThan(50), "Level 10 should have more HP than level 1");
    }

    [Test]
    public void MonsterGenerator_GetTemplates_FiltersCorrectly()
    {
        // Act
        var allTemplates = _generatorService.GetAllTemplates();
        var normalMonsters = _generatorService.GetTemplates(difficulty: MonsterDifficulty.Normal);
        var bosses = _generatorService.GetTemplates(difficulty: MonsterDifficulty.Boss);
        var raids = _generatorService.GetTemplates(difficulty: MonsterDifficulty.RaidBoss);
        var lowLevel = _generatorService.GetTemplates(minLevel: 1, maxLevel: 10);

        // Assert
        Assert.That(allTemplates.Count, Is.EqualTo(145), "Should have 145 total templates");
        Assert.That(normalMonsters.Count, Is.EqualTo(85), "Should have 85 normal monsters");
        Assert.That(bosses.Count, Is.EqualTo(50), "Should have 50 bosses");
        Assert.That(raids.Count, Is.EqualTo(10), "Should have 10 raid bosses");
        Assert.That(lowLevel.Count, Is.GreaterThan(0), "Should have monsters in level 1-10 range");
    }

    [Test]
    public void EncounterService_GenerateNormalEncounter_Creates1To3Monsters()
    {
        // Arrange & Act
        var encounter = _encounterService.GenerateEncounter(
            characterLevel: 5,
            zone: ZoneType.WhisperingWoods,
            isBossEncounter: false,
            isRaidEncounter: false
        );

        // Assert
        Assert.That(encounter.Monsters.Count, Is.InRange(1, 3), "Normal encounter should have 1-3 monsters");
        Assert.That(encounter.Boss, Is.Null, "Normal encounter should not have a boss");
        Assert.That(encounter.RaidBoss, Is.Null, "Normal encounter should not have a raid boss");
        Assert.That(encounter.TotalExperience, Is.GreaterThan(0));
        Assert.That(encounter.TotalGold, Is.GreaterThan(0));
    }

    [Test]
    public void EncounterService_GenerateBossEncounter_HasBossMonster()
    {
        // Arrange & Act
        var encounter = _encounterService.GenerateBossEncounter("fallen-knight", includeMinions: false);

        // Assert
        Assert.That(encounter.Boss, Is.Not.Null, "Boss encounter must have a boss");
        Assert.That(encounter.Boss!.Name, Is.EqualTo("The Fallen Knight"));
        Assert.That(encounter.Boss.Difficulty, Is.EqualTo(MonsterDifficulty.Boss));
        Assert.That(encounter.Monsters.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void EncounterService_GenerateRaidEncounter_HasRaidBoss()
    {
        // Arrange & Act
        var encounter = _encounterService.GenerateRaidEncounter("void-touched-titan", includeMinions: false);

        // Assert
        Assert.That(encounter.RaidBoss, Is.Not.Null, "Raid encounter must have a raid boss");
        Assert.That(encounter.RaidBoss!.Name, Is.EqualTo("The Void-Touched Titan"));
        Assert.That(encounter.RaidBoss.Difficulty, Is.EqualTo(MonsterDifficulty.RaidBoss));
        Assert.That(encounter.RaidBoss.MaxHealth, Is.GreaterThan(10000), "Raid boss should have massive HP");
    }

    [Test]
    public void MonsterSystem_Level300RaidBoss_HasAppropriateStats()
    {
        // Arrange & Act
        var raidBoss = _generatorService.GenerateMonster("absolution-arbiter");

        // Assert
        Assert.That(raidBoss.Level, Is.EqualTo(300));
        Assert.That(raidBoss.MaxHealth, Is.GreaterThan(100000), "Level 300 raid should have 100k+ HP");
        Assert.That(raidBoss.AttackPower, Is.GreaterThan(5000), "Level 300 raid should have 5k+ attack");
        Assert.That(raidBoss.ExperienceReward, Is.GreaterThan(500000), "Level 300 raid should give 500k+ EXP");
    }

    [Test]
    public void MonsterSystem_AllTemplates_AreAccessible()
    {
        // Arrange
        var allTemplates = _generatorService.GetAllTemplates();

        // Act - Try to generate each monster
        var failures = new List<string>();
        var successes = 0;
        
        foreach (var template in allTemplates)
        {
            try
            {
                var monster = _generatorService.GenerateMonster(template.TemplateId);
                Assert.That(monster.Name, Is.EqualTo(template.Name), 
                    $"Monster name mismatch for template {template.TemplateId}");
                Assert.That(monster.MaxHealth, Is.GreaterThan(0), 
                    $"Monster {template.TemplateId} has invalid HP");
                Assert.That(monster.AttackPower, Is.GreaterThan(0), 
                    $"Monster {template.TemplateId} has invalid attack power");
                successes++;
            }
            catch (Exception ex)
            {
                failures.Add($"{template.TemplateId} (Level {template.Level}, {template.Difficulty}): {ex.Message}");
            }
        }

        // Output summary
        TestContext.WriteLine($"Successfully generated {successes}/{allTemplates.Count} monsters");
        if (failures.Any())
        {
            TestContext.WriteLine($"\nFailed templates ({failures.Count}):");
            foreach (var failure in failures)
            {
                TestContext.WriteLine($"  - {failure}");
            }
        }

        // Assert
        Assert.That(failures, Is.Empty, 
            $"All {allTemplates.Count} templates should generate successfully. " +
            $"Failures: {failures.Count}\n" + string.Join("\n", failures.Take(5)));
    }
}
