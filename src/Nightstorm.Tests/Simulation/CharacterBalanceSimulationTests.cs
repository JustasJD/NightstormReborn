using FluentAssertions;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Services;
using System.Text;

namespace Nightstorm.Tests.Simulation;

/// <summary>
/// Class Balance Simulation Tests - Tests all character classes attacking training dummies.
/// This is NOT a unit test - it's an integration/simulation test to evaluate game balance.
/// </summary>
[TestFixture]
public class CharacterBalanceSimulationTests
{
    private CharacterStatsService _statsService = null!;

    [SetUp]
    public void Setup()
    {
        _statsService = new CharacterStatsService();
    }

    /// <summary>
    /// Simulates all 16 character classes attacking 4 different dummy types (tank, dps, caster, ranged).
    /// Each class attacks each dummy 20 times and we measure time-to-kill.
    /// </summary>
    [Test]
    public void SimulateAllClassesVsAllDummies_GeneratesBalanceReport()
    {
        // Arrange
        var allClasses = Enum.GetValues<CharacterClass>();
        var dummyTypes = new[]
        {
            ("Tank Dummy", 50),      // High defense
            ("DPS Dummy", 20),       // Medium defense
            ("Caster Dummy", 10),    // Low defense
            ("Ranged Dummy", 15)     // Low-medium defense
        };

        const int dummyHp = 5000;
        const int attacksPerTest = 20;

        var results = new List<SimulationResult>();

        // Act - Simulate combat for each class against each dummy
        foreach (var characterClass in allClasses)
        {
            var character = CreateCharacterWithClass(characterClass);

            foreach (var (dummyName, defense) in dummyTypes)
            {
                var result = SimulateCombat(character, dummyName, dummyHp, defense, attacksPerTest);
                results.Add(result);
            }
        }

        // Assert - Generate comprehensive balance report
        GenerateBalanceReport(results);

        // Basic assertion - all classes should be able to deal damage
        results.Should().NotBeEmpty();
        results.All(r => r.TotalDamageDealt > 0).Should().BeTrue("All classes should deal damage");
    }

    private Character CreateCharacterWithClass(CharacterClass characterClass)
    {
        var character = new Character
        {
            Class = characterClass,
            Name = $"Test{characterClass}"
        };

        character.InitializeStats(_statsService); // Pass the stats service
        return character;
    }

    private SimulationResult SimulateCombat(
        Character attacker,
        string dummyName,
        int dummyHp,
        int dummyDefense,
        int numberOfAttacks)
    {
        var remainingHp = dummyHp;
        var totalDamage = 0;
        var attackLog = new List<int>();

        // Determine which attack power to use based on class archetype
        var isPhysical = _statsService.IsPhysicalArchetype(attacker.Class);
        var attackPower = DetermineAttackPower(attacker, isPhysical);

        for (int i = 0; i < numberOfAttacks && remainingHp > 0; i++)
        {
            // Simple damage formula: AttackPower - (Defense / 2), minimum 1
            var damageDealt = Math.Max(1, attackPower - (dummyDefense / 2));

            totalDamage += damageDealt;
            remainingHp -= damageDealt;
            attackLog.Add(damageDealt);
        }

        var attacksNeededToKill = remainingHp <= 0
            ? attackLog.Count
            : (int)Math.Ceiling((double)dummyHp / (totalDamage / (double)numberOfAttacks));

        return new SimulationResult
        {
            CharacterClass = attacker.Class,
            CharacterName = attacker.Name,
            DummyName = dummyName,
            DummyDefense = dummyDefense,
            AttackPower = attackPower,
            TotalDamageDealt = totalDamage,
            AttacksExecuted = numberOfAttacks,
            AverageDamagePerHit = totalDamage / (double)numberOfAttacks,
            EstimatedAttacksToKill = attacksNeededToKill,
            EstimatedTimeToKill = attacksNeededToKill, // Assuming 1 turn = 1 time unit
            DummyKilled = remainingHp <= 0,
            AttackLog = attackLog
        };
    }

    private int DetermineAttackPower(Character character, bool isPhysical)
    {
        if (isPhysical)
        {
            // Physical classes use their highest attack power (melee or ranged)
            var meleeAttack = _statsService.CalculateMeleeAttackPower(
                character.Strength,
                character.Dexterity
            );
            var rangedAttack = _statsService.CalculateRangedAttackPower(
                character.Dexterity,
                character.Strength
            );
            return Math.Max(meleeAttack, rangedAttack);
        }
        else
        {
            // Casters use magic power
            return _statsService.CalculateMagicPower(
                character.Class,
                character.Intelligence,
                character.Wisdom,
                character.Spirit
            );
        }
    }

    private void GenerateBalanceReport(List<SimulationResult> results)
    {
        var report = new StringBuilder();
        report.AppendLine("\n" + new string('=', 100));
        report.AppendLine("CHARACTER CLASS BALANCE SIMULATION REPORT");
        report.AppendLine(new string('=', 100));

        // Group by dummy type
        var resultsByDummy = results.GroupBy(r => r.DummyName);

        foreach (var dummyGroup in resultsByDummy)
        {
            report.AppendLine($"\n### {dummyGroup.Key} (Defense: {dummyGroup.First().DummyDefense}) ###\n");

            // Sort by time to kill (fastest first)
            var sortedResults = dummyGroup.OrderBy(r => r.EstimatedTimeToKill).ToList();

            report.AppendLine($"{"Rank",-6} {"Class",-15} {"Attack Power",-15} {"Avg Dmg/Hit",-15} {"Attacks to Kill",-18} {"Performance",-15}");
            report.AppendLine(new string('-', 100));

            var fastestTime = sortedResults.First().EstimatedTimeToKill;
            var slowestTime = sortedResults.Last().EstimatedTimeToKill;

            for (int i = 0; i < sortedResults.Count; i++)
            {
                var result = sortedResults[i];
                var rank = i + 1;

                // Calculate performance relative to fastest
                var performanceRatio = (double)result.EstimatedTimeToKill / fastestTime;
                var performancePercent = (1.0 / performanceRatio) * 100;
                var performanceLabel = performanceRatio switch
                {
                    <= 1.1 => "Excellent",
                    <= 1.25 => "Good",
                    <= 1.5 => "Average",
                    <= 1.75 => "Below Avg",
                    _ => "Weak"
                };

                report.AppendLine($"{rank,-6} {result.CharacterClass,-15} {result.AttackPower,-15} " +
                                $"{result.AverageDamagePerHit,-15:F2} {result.EstimatedAttacksToKill,-18} " +
                                $"{performanceLabel,-15} ({performancePercent:F1}%)");
            }

            // Summary statistics
            report.AppendLine(new string('-', 100));
            report.AppendLine($"Fastest Kill: {sortedResults.First().CharacterClass} ({sortedResults.First().EstimatedAttacksToKill} attacks)");
            report.AppendLine($"Slowest Kill: {sortedResults.Last().CharacterClass} ({sortedResults.Last().EstimatedAttacksToKill} attacks)");
            report.AppendLine($"Average: {sortedResults.Average(r => r.EstimatedAttacksToKill):F1} attacks");
            report.AppendLine($"Balance Spread: {((slowestTime - fastestTime) / (double)fastestTime * 100):F1}% difference between best and worst");
        }

        // Overall Summary by Archetype
        report.AppendLine($"\n{new string('=', 100)}");
        report.AppendLine("ARCHETYPE COMPARISON");
        report.AppendLine(new string('=', 100));

        var physicalClasses = results.Where(r => _statsService.IsPhysicalArchetype(r.CharacterClass)).ToList();
        var casterClasses = results.Where(r => !_statsService.IsPhysicalArchetype(r.CharacterClass)).ToList();

        report.AppendLine($"\nPhysical Classes Average: {physicalClasses.Average(r => r.EstimatedAttacksToKill):F1} attacks to kill");
        report.AppendLine($"Caster Classes Average: {casterClasses.Average(r => r.EstimatedAttacksToKill):F1} attacks to kill");
        report.AppendLine($"\nTop 5 Fastest Classes Overall:");

        var top5 = results
            .GroupBy(r => r.CharacterClass)
            .Select(g => new
            {
                Class = g.Key,
                AvgAttacksToKill = g.Average(r => r.EstimatedAttacksToKill)
            })
            .OrderBy(x => x.AvgAttacksToKill)
            .Take(5)
            .ToList();

        for (int i = 0; i < top5.Count; i++)
        {
            report.AppendLine($"{i + 1}. {top5[i].Class} - {top5[i].AvgAttacksToKill:F1} avg attacks to kill");
        }

        report.AppendLine($"\n{new string('=', 100)}\n");

        // Output to test console
        TestContext.WriteLine(report.ToString());
    }

    private class SimulationResult
    {
        public CharacterClass CharacterClass { get; init; }
        public string CharacterName { get; init; } = string.Empty;
        public string DummyName { get; init; } = string.Empty;
        public int DummyDefense { get; init; }
        public int AttackPower { get; init; }
        public int TotalDamageDealt { get; init; }
        public int AttacksExecuted { get; init; }
        public double AverageDamagePerHit { get; init; }
        public int EstimatedAttacksToKill { get; init; }
        public double EstimatedTimeToKill { get; init; }
        public bool DummyKilled { get; init; }
        public List<int> AttackLog { get; init; } = new();
    }
}
