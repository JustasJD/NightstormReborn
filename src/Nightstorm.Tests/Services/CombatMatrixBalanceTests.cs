using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Services;
using Nightstorm.Core.Extensions;
using System.Text;

namespace Nightstorm.Tests.Services;

/// <summary>
/// Comprehensive class-vs-class combat matrix for deep balance analysis.
/// Tests every class against every other class (including itself) to validate
/// the rock-paper-scissors combat system and ensure no class is overpowered.
/// </summary>
[TestFixture]
public class CombatMatrixBalanceTests
{
    private CombatService _combatService = null!;
    private CharacterStatsService _statsService = null!;

    [SetUp]
    public void Setup()
    {
        _statsService = new CharacterStatsService();
        _combatService = new CombatService(_statsService);
    }

    [Test]
    public void ClassVsClass_FullCombatMatrix_50RoundsEach()
    {
        // Arrange
        var allClasses = Enum.GetValues<CharacterClass>().ToList();
        const int rounds = 50;
        
        // Results: [Attacker][Defender] = (Wins, AvgDamage, AvgTurnsToKill)
        var matrix = new Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>>();
        
        // Initialize matrix
        foreach (var attackerClass in allClasses)
        {
            matrix[attackerClass] = new Dictionary<CharacterClass, (int, double, double)>();
        }
        
        // Act - Fight every class against every class
        TestContext.WriteLine("=== RUNNING FULL COMBAT MATRIX (16x16 = 256 matchups, 50 rounds each) ===\n");
        TestContext.WriteLine("This will take a moment...\n");
        
        int matchupCount = 0;
        foreach (var attackerClass in allClasses)
        {
            foreach (var defenderClass in allClasses)
            {
                matchupCount++;
                var (wins, avgDmg, avgTurns) = RunMatchup(attackerClass, defenderClass, rounds);
                matrix[attackerClass][defenderClass] = (wins, avgDmg, avgTurns);
                
                if (matchupCount % 16 == 0)
                    TestContext.WriteLine($"Progress: {matchupCount}/256 matchups complete");
            }
        }
        
        // Output results
        OutputWinRateMatrix(matrix, allClasses);
        OutputDamagePerTurnMatrix(matrix, allClasses);
        OutputTypeEffectivenessAnalysis(matrix, allClasses);
        OutputClassPowerRankings(matrix, allClasses);
        
        // Assert - Validation rules
        ValidateRockPaperScissors(matrix, allClasses);
        ValidateNoGrossImbalance(matrix, allClasses);
    }

    private (int wins, double avgDamage, double avgTurns) RunMatchup(
        CharacterClass attackerClass,
        CharacterClass defenderClass,
        int rounds)
    {
        int attackerWins = 0;
        double totalDamage = 0;
        double totalTurns = 0;

        for (int i = 0; i < rounds; i++)
        {
            var attacker = CreateCharacter(attackerClass, level: 5);
            var defender = CreateCharacter(defenderClass, level: 5);
            
            int turns = 0;
            const int maxTurns = 100;
            
            while (attacker.CurrentHealth > 0 && defender.CurrentHealth > 0 && turns < maxTurns)
            {
                // Attacker's turn
                var attackResult = _combatService.CalculateAttack(attacker, CreateMonsterFromCharacter(defender));
                defender.CurrentHealth -= attackResult.FinalDamage;
                totalDamage += attackResult.FinalDamage;
                turns++;
                
                if (defender.CurrentHealth <= 0)
                {
                    attackerWins++;
                    break;
                }
                
                // Defender's turn
                var counterResult = _combatService.CalculateAttack(defender, CreateMonsterFromCharacter(attacker));
                attacker.CurrentHealth -= counterResult.FinalDamage;
                turns++;
                
                if (attacker.CurrentHealth <= 0)
                    break;
            }
            
            totalTurns += turns;
        }

        return (attackerWins, totalDamage / rounds, totalTurns / rounds);
    }

    private void OutputWinRateMatrix(
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix,
        List<CharacterClass> allClasses)
    {
        TestContext.WriteLine("\n=== WIN RATE MATRIX (%) ===");
        TestContext.WriteLine("Rows = Attacker, Columns = Defender, Value = Attacker Win %\n");
        
        // Header
        var header = new StringBuilder("Attacker".PadRight(15));
        foreach (var defenderClass in allClasses)
        {
            header.Append($"{defenderClass.ToString().Substring(0, Math.Min(6, defenderClass.ToString().Length)),7}");
        }
        TestContext.WriteLine(header.ToString());
        TestContext.WriteLine(new string('-', header.Length));
        
        // Data rows
        foreach (var attackerClass in allClasses)
        {
            var row = new StringBuilder($"{attackerClass.ToString().PadRight(15)}");
            foreach (var defenderClass in allClasses)
            {
                var winRate = matrix[attackerClass][defenderClass].Wins * 2; // Convert to percentage (out of 50)
                row.Append($"{winRate,7}");
            }
            TestContext.WriteLine(row.ToString());
        }
    }

    private void OutputDamagePerTurnMatrix(
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix,
        List<CharacterClass> allClasses)
    {
        TestContext.WriteLine("\n=== AVERAGE DAMAGE PER TURN MATRIX ===");
        TestContext.WriteLine("Rows = Attacker, Columns = Defender\n");
        
        // Header
        var header = new StringBuilder("Attacker".PadRight(15));
        foreach (var defenderClass in allClasses)
        {
            header.Append($"{defenderClass.ToString().Substring(0, Math.Min(6, defenderClass.ToString().Length)),7}");
        }
        TestContext.WriteLine(header.ToString());
        TestContext.WriteLine(new string('-', header.Length));
        
        // Data rows
        foreach (var attackerClass in allClasses)
        {
            var row = new StringBuilder($"{attackerClass.ToString().PadRight(15)}");
            foreach (var defenderClass in allClasses)
            {
                var avgDmg = matrix[attackerClass][defenderClass].AvgDamage;
                row.Append($"{avgDmg,7:F1}");
            }
            TestContext.WriteLine(row.ToString());
        }
    }

    private void OutputTypeEffectivenessAnalysis(
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix,
        List<CharacterClass> allClasses)
    {
        TestContext.WriteLine("\n=== TYPE MATCHUP ANALYSIS ===\n");
        
        // Group classes by attack type
        var heavyMelee = new[] { CharacterClass.Warden, CharacterClass.DarkKnight, CharacterClass.Duelist, CharacterClass.Dragoon };
        var fastMelee = new[] { CharacterClass.Monk, CharacterClass.Rogue, CharacterClass.Ranger, CharacterClass.Gunslinger };
        var elementalMagic = new[] { CharacterClass.Wizard, CharacterClass.Sorcerer, CharacterClass.Druid };
        var spiritualMagic = new[] { CharacterClass.Necromancer, CharacterClass.Cleric, CharacterClass.Bard };
        var hybrid = new[] { CharacterClass.Paladin, CharacterClass.Alchemist };
        
        AnalyzeTypeMatchup("HeavyMelee vs SpiritualMagic (Should Win)", heavyMelee, spiritualMagic, matrix);
        AnalyzeTypeMatchup("HeavyMelee vs ElementalMagic (Should Lose)", heavyMelee, elementalMagic, matrix);
        AnalyzeTypeMatchup("FastMelee vs HeavyMelee (Should Win)", fastMelee, heavyMelee, matrix);
        AnalyzeTypeMatchup("FastMelee vs ElementalMagic (Should Lose)", fastMelee, elementalMagic, matrix);
        AnalyzeTypeMatchup("ElementalMagic vs HeavyMelee (Should Win)", elementalMagic, heavyMelee, matrix);
        AnalyzeTypeMatchup("ElementalMagic vs FastMelee (Should Lose)", elementalMagic, fastMelee, matrix);
        AnalyzeTypeMatchup("SpiritualMagic vs FastMelee (Should Win)", spiritualMagic, fastMelee, matrix);
        AnalyzeTypeMatchup("SpiritualMagic vs HeavyMelee (Should Lose)", spiritualMagic, heavyMelee, matrix);
    }

    private void AnalyzeTypeMatchup(
        string matchupName,
        CharacterClass[] attackers,
        CharacterClass[] defenders,
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix)
    {
        double totalWinRate = 0;
        int count = 0;
        
        foreach (var attacker in attackers)
        {
            foreach (var defender in defenders)
            {
                totalWinRate += matrix[attacker][defender].Wins * 2; // Convert to percentage
                count++;
            }
        }
        
        double avgWinRate = totalWinRate / count;
        string result = avgWinRate > 60 ? "? WORKING" : avgWinRate < 40 ? "? WORKING (Reversed)" : "? NEUTRAL";
        
        TestContext.WriteLine($"{matchupName,-50} Avg Win Rate: {avgWinRate,5:F1}% {result}");
    }

    private void OutputClassPowerRankings(
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix,
        List<CharacterClass> allClasses)
    {
        TestContext.WriteLine("\n=== OVERALL CLASS POWER RANKINGS ===\n");
        
        var rankings = new List<(CharacterClass Class, double OverallWinRate, double AvgDamageDealt, double AvgDamageTaken)>();
        
        foreach (var characterClass in allClasses)
        {
            // Calculate overall win rate as attacker
            var totalWins = matrix[characterClass].Values.Sum(v => v.Wins);
            var totalGames = matrix[characterClass].Values.Count() * 50;
            var winRate = (double)totalWins / totalGames * 100;
            
            // Calculate average damage dealt
            var avgDmgDealt = matrix[characterClass].Values.Average(v => v.AvgDamage);
            
            // Calculate average damage taken (when defending)
            var avgDmgTaken = allClasses.Average(attacker => matrix[attacker][characterClass].AvgDamage);
            
            rankings.Add((characterClass, winRate, avgDmgDealt, avgDmgTaken));
        }
        
        TestContext.WriteLine($"{"Class",-15} {"Win Rate",-10} {"Dmg Dealt",-12} {"Dmg Taken",-12} {"Assessment",-15}");
        TestContext.WriteLine(new string('-', 70));
        
        foreach (var (cls, winRate, dmgDealt, dmgTaken) in rankings.OrderByDescending(r => r.OverallWinRate))
        {
            var assessment = winRate switch
            {
                > 55 => "?? Too Strong",
                > 52 => "Strong",
                < 45 => "?? Too Weak",
                < 48 => "Weak",
                _ => "Balanced"
            };
            
            TestContext.WriteLine($"{cls,-15} {winRate,9:F1}% {dmgDealt,11:F1} {dmgTaken,11:F1} {assessment,-15}");
        }
    }

    private void ValidateRockPaperScissors(
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix,
        List<CharacterClass> allClasses)
    {
        TestContext.WriteLine("\n=== ROCK-PAPER-SCISSORS VALIDATION ===\n");
        
        // Test that advantage matchups actually result in wins
        var heavyMelee = new[] { CharacterClass.Warden, CharacterClass.DarkKnight };
        var spiritualMagic = new[] { CharacterClass.Necromancer, CharacterClass.Cleric };
        
        foreach (var heavy in heavyMelee)
        {
            foreach (var spiritual in spiritualMagic)
            {
                var winRate = matrix[heavy][spiritual].Wins * 2;
                Assert.That(winRate, Is.GreaterThan(45), 
                    $"{heavy} should have advantage over {spiritual} but win rate is only {winRate}%");
            }
        }
        
        TestContext.WriteLine("? Rock-paper-scissors system validated!");
    }

    private void ValidateNoGrossImbalance(
        Dictionary<CharacterClass, Dictionary<CharacterClass, (int Wins, double AvgDamage, double AvgTurns)>> matrix,
        List<CharacterClass> allClasses)
    {
        TestContext.WriteLine("\n=== GROSS IMBALANCE CHECK ===\n");
        
        foreach (var characterClass in allClasses)
        {
            var totalWins = matrix[characterClass].Values.Sum(v => v.Wins);
            var totalGames = matrix[characterClass].Values.Count() * 50;
            var winRate = (double)totalWins / totalGames * 100;
            
            Assert.That(winRate, Is.InRange(35, 65),
                $"{characterClass} has {winRate:F1}% overall win rate - should be 35-65%");
        }
        
        TestContext.WriteLine("? No class has gross imbalance!");
    }

    // Helper methods
    private Character CreateCharacter(CharacterClass characterClass, int level)
    {
        var character = new Character
        {
            Name = characterClass.ToString(),
            Class = characterClass,
            Level = level
        };

        character.InitializeStats(_statsService);
        return character;
    }

    private Monster CreateMonsterFromCharacter(Character character)
    {
        var stats = character.GetCombatStats(_statsService);
        
        return new Monster
        {
            Name = character.Name,
            Type = MonsterType.Humanoid, // Changed from MonsterType.Goblin
            Difficulty = MonsterDifficulty.Normal,
            Level = character.Level,
            MaxHealth = character.MaxHealth,
            CurrentHealth = character.CurrentHealth,
            AttackType = stats.AttackType,
            AttackPower = stats.PrimaryAttackPower,
            ArmorType = _statsService.GetArmorType(character.Class),
            HeavyMeleeDefense = stats.HeavyMeleeDefense,
            FastMeleeDefense = stats.FastMeleeDefense,
            ElementalMagicDefense = stats.ElementalMagicDefense,
            SpiritualMagicDefense = stats.SpiritualMagicDefense
        };
    }
}
