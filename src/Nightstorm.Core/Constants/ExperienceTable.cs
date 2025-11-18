namespace Nightstorm.Core.Constants;

/// <summary>
/// Defines the experience progression table for character leveling.
/// </summary>
public static class ExperienceTable
{
    /// <summary>
    /// Maximum level a character can achieve.
    /// </summary>
    public const int MaxLevel = 99;

    /// <summary>
    /// Starting level for new characters.
    /// </summary>
    public const int StartingLevel = 1;

    /// <summary>
    /// Array containing the total XP required to reach each level (0-indexed, so index 0 = level 1).
    /// Value represents the total XP at the END of that level.
    /// </summary>
    private static readonly long[] _totalXpAtLevel = new long[]
    {
        0L,         // Level 1
        83L,        // Level 2
        174L,       // Level 3
        276L,       // Level 4
        388L,       // Level 5
        512L,       // Level 6
        650L,       // Level 7
        801L,       // Level 8
        969L,       // Level 9
        1154L,      // Level 10
        1358L,      // Level 11
        1584L,      // Level 12
        1833L,      // Level 13
        2107L,      // Level 14
        2411L,      // Level 15
        2746L,      // Level 16
        3115L,      // Level 17
        3523L,      // Level 18
        3973L,      // Level 19
        4470L,      // Level 20
        5018L,      // Level 21
        5624L,      // Level 22
        6291L,      // Level 23
        7028L,      // Level 24
        7842L,      // Level 25
        // Difficulty Multiplier 1.05
        8767L,      // Level 26
        9781L,      // Level 27
        10895L,     // Level 28
        12122L,     // Level 29
        13476L,     // Level 30
        14972L,     // Level 31
        16625L,     // Level 32
        18452L,     // Level 33
        20473L,     // Level 34
        22709L,     // Level 35
        25183L,     // Level 36
        27917L,     // Level 37
        30938L,     // Level 38
        34277L,     // Level 39
        37969L,     // Level 40
        42049L,     // Level 41
        46554L,     // Level 42
        51529L,     // Level 43
        57015L,     // Level 44
        63059L,     // Level 45
        69715L,     // Level 46
        77039L,     // Level 47
        85089L,     // Level 48
        93927L,     // Level 49
        103623L,    // Level 50
        // Difficulty Multiplier 1.10
        114357L,    // Level 51
        126209L,    // Level 52
        139265L,    // Level 53
        153605L,    // Level 54
        169322L,    // Level 55
        186521L,    // Level 56
        205335L,    // Level 57
        225897L,    // Level 58
        248397L,    // Level 59
        273083L,    // Level 60
        300192L,    // Level 61
        330001L,    // Level 62
        362796L,    // Level 63
        398881L,    // Level 64
        438591L,    // Level 65
        482293L,    // Level 66
        530385L,    // Level 67
        583293L,    // Level 68
        641472L,    // Level 69
        705379L,    // Level 70
        775487L,    // Level 71
        852267L,    // Level 72
        936233L,    // Level 73
        1027931L,   // Level 74
        1128041L,   // Level 75
        // Difficulty Multiplier 1.15
        1237595L,   // Level 76
        1357804L,   // Level 77
        1489957L,   // Level 78
        1635532L,   // Level 79
        1796258L,   // Level 80
        1974138L,   // Level 81
        2171463L,   // Level 82
        2390741L,   // Level 83
        2634818L,   // Level 84
        2906854L,   // Level 85
        3209796L,   // Level 86
        3546477L,   // Level 87
        3919954L,   // Level 88
        4333744L,   // Level 89
        4791895L,   // Level 90
        5298995L,   // Level 91
        5859747L,   // Level 92
        6480186L,   // Level 93
        7167006L,   // Level 94
        7927337L,   // Level 95
        8768074L,   // Level 96
        9700775L,   // Level 97
        10737842L,  // Level 98
        11892108L   // Level 99
    };

    /// <summary>
    /// Gets the total XP required at the end of the specified level.
    /// </summary>
    /// <param name="level">The level (1-99).</param>
    /// <returns>Total XP at the end of that level.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when level is not between 1 and 99.</exception>
    public static long GetTotalXpForLevel(int level)
    {
        if (level < 1 || level > MaxLevel)
        {
            throw new ArgumentOutOfRangeException(nameof(level), $"Level must be between {StartingLevel} and {MaxLevel}.");
        }

        return _totalXpAtLevel[level - 1];
    }

    /// <summary>
    /// Gets the XP required to gain the next level from the current level.
    /// </summary>
    /// <param name="currentLevel">The current level (1-98).</param>
    /// <returns>XP needed to reach the next level.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when already at max level or invalid level.</exception>
    public static long GetXpToNextLevel(int currentLevel)
    {
        if (currentLevel < 1 || currentLevel >= MaxLevel)
        {
            throw new ArgumentOutOfRangeException(nameof(currentLevel), $"Current level must be between {StartingLevel} and {MaxLevel - 1}.");
        }

        return _totalXpAtLevel[currentLevel] - _totalXpAtLevel[currentLevel - 1];
    }

    /// <summary>
    /// Calculates the current level based on total experience points.
    /// </summary>
    /// <param name="totalXp">Total experience points.</param>
    /// <returns>The current level (1-99).</returns>
    public static int GetLevelFromExperience(long totalXp)
    {
        if (totalXp < 0)
        {
            return StartingLevel;
        }

        // Find the highest level where totalXp >= required XP
        for (int i = _totalXpAtLevel.Length - 1; i >= 0; i--)
        {
            if (totalXp >= _totalXpAtLevel[i])
            {
                return i + 1;
            }
        }

        return StartingLevel;
    }

    /// <summary>
    /// Gets the experience remaining until the next level.
    /// </summary>
    /// <param name="currentXp">Current experience points.</param>
    /// <param name="currentLevel">Current level.</param>
    /// <returns>XP remaining to reach next level, or 0 if at max level.</returns>
    public static long GetExperienceToNextLevel(long currentXp, int currentLevel)
    {
        if (currentLevel >= MaxLevel)
        {
            return 0;
        }

        long xpForNextLevel = GetTotalXpForLevel(currentLevel + 1);
        long remaining = xpForNextLevel - currentXp;

        return remaining > 0 ? remaining : 0;
    }

    /// <summary>
    /// Gets the experience progress percentage towards the next level.
    /// </summary>
    /// <param name="currentXp">Current experience points.</param>
    /// <param name="currentLevel">Current level.</param>
    /// <returns>Percentage (0-100) of progress to next level.</returns>
    public static double GetProgressPercentage(long currentXp, int currentLevel)
    {
        if (currentLevel >= MaxLevel)
        {
            return 100.0;
        }

        long currentLevelXp = currentLevel == 1 ? 0 : GetTotalXpForLevel(currentLevel);
        long nextLevelXp = GetTotalXpForLevel(currentLevel + 1);
        long levelRange = nextLevelXp - currentLevelXp;

        if (levelRange == 0)
        {
            return 100.0;
        }

        long progressInLevel = currentXp - currentLevelXp;
        double percentage = (double)progressInLevel / levelRange * 100.0;

        return Math.Clamp(percentage, 0.0, 100.0);
    }

    /// <summary>
    /// Checks if the character should level up based on their current XP.
    /// </summary>
    /// <param name="currentXp">Current experience points.</param>
    /// <param name="currentLevel">Current level.</param>
    /// <returns>True if the character has enough XP to level up.</returns>
    public static bool ShouldLevelUp(long currentXp, int currentLevel)
    {
        if (currentLevel >= MaxLevel)
        {
            return false;
        }

        return currentXp >= GetTotalXpForLevel(currentLevel + 1);
    }

    /// <summary>
    /// Calculates how many levels can be gained from the current XP.
    /// </summary>
    /// <param name="currentXp">Current experience points.</param>
    /// <param name="currentLevel">Current level.</param>
    /// <returns>Number of levels that can be gained (could be 0 or multiple).</returns>
    public static int CalculateLevelGains(long currentXp, int currentLevel)
    {
        int levelsGained = 0;
        int tempLevel = currentLevel;

        while (tempLevel < MaxLevel && currentXp >= GetTotalXpForLevel(tempLevel + 1))
        {
            tempLevel++;
            levelsGained++;
        }

        return levelsGained;
    }
}
