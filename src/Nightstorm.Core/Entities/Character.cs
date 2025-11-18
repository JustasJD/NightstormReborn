using Nightstorm.Core.Constants;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a player character in the game.
/// </summary>
public class Character : BaseEntity
{
    /// <summary>
    /// Gets or sets the Discord user ID associated with this character.
    /// </summary>
    public ulong DiscordUserId { get; set; }

    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character class.
    /// </summary>
    public CharacterClass Class { get; set; }

    /// <summary>
    /// Gets or sets the character level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the current experience points.
    /// </summary>
    public long Experience { get; set; }

    /// <summary>
    /// Gets or sets the maximum health points.
    /// </summary>
    public int MaxHealth { get; set; }

    /// <summary>
    /// Gets or sets the current health points.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// Gets or sets the maximum mana points.
    /// </summary>
    public int MaxMana { get; set; }

    /// <summary>
    /// Gets or sets the current mana points.
    /// </summary>
    public int CurrentMana { get; set; }

    // Primary Stats
    
    /// <summary>
    /// Gets or sets the Strength stat (Physical Might).
    /// </summary>
    public int Strength { get; set; }

    /// <summary>
    /// Gets or sets the Dexterity stat (Agility, Speed and Accuracy).
    /// </summary>
    public int Dexterity { get; set; }

    /// <summary>
    /// Gets or sets the Constitution stat (Physical Endurance & Health).
    /// </summary>
    public int Constitution { get; set; }

    /// <summary>
    /// Gets or sets the Intelligence stat (Knowledge & Arcane Power).
    /// </summary>
    public int Intelligence { get; set; }

    /// <summary>
    /// Gets or sets the Wisdom stat (Magical Endurance).
    /// </summary>
    public int Wisdom { get; set; }

    /// <summary>
    /// Gets or sets the Spirit stat (Mental Fortitude & Mana Amount).
    /// </summary>
    public int Spirit { get; set; }

    /// <summary>
    /// Gets or sets the Luck stat (Fortune & Drop Chance).
    /// </summary>
    public int Luck { get; set; }

    /// <summary>
    /// Gets or sets the amount of gold the character has.
    /// </summary>
    public long Gold { get; set; }

    /// <summary>
    /// Gets or sets the guild ID this character belongs to.
    /// </summary>
    public Guid? GuildId { get; set; }

    /// <summary>
    /// Navigation property to the guild.
    /// </summary>
    public Guild? Guild { get; set; }

    /// <summary>
    /// Navigation property to the character's inventory items.
    /// </summary>
    public ICollection<CharacterItem> Inventory { get; set; } = new List<CharacterItem>();

    /// <summary>
    /// Navigation property to the character's quests.
    /// </summary>
    public ICollection<CharacterQuest> Quests { get; set; } = new List<CharacterQuest>();

    public Character()
    {
        Level = CharacterConstants.DefaultLevel;
        Experience = CharacterConstants.DefaultExperience;
        Gold = CharacterConstants.DefaultGold;
    }

    /// <summary>
    /// Initializes character stats based on class using the stats service.
    /// </summary>
    /// <param name="statsService">The character stats calculation service.</param>
    public void InitializeStats(ICharacterStatsService statsService)
    {
        // Set base stats based on class
        SetBaseStatsForClass();
        
        // Calculate derived stats using the service
        MaxHealth = statsService.CalculateMaxHealth(Class, Constitution, Strength, Dexterity);
        MaxMana = statsService.CalculateMaxMana(Class, Spirit, Intelligence, Wisdom);
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
    }

    /// <summary>
    /// Recalculates all derived stats when base stats change (e.g., after leveling up or equipping items).
    /// </summary>
    /// <param name="statsService">The character stats calculation service.</param>
    public void RecalculateStats(ICharacterStatsService statsService)
    {
        var healthPercentage = MaxHealth > 0 ? (double)CurrentHealth / MaxHealth : 1.0;
        var manaPercentage = MaxMana > 0 ? (double)CurrentMana / MaxMana : 1.0;
        
        MaxHealth = statsService.CalculateMaxHealth(Class, Constitution, Strength, Dexterity);
        MaxMana = statsService.CalculateMaxMana(Class, Spirit, Intelligence, Wisdom);
        
        // Maintain health/mana percentages after recalculation
        CurrentHealth = Math.Min((int)(MaxHealth * healthPercentage), MaxHealth);
        CurrentMana = Math.Min((int)(MaxMana * manaPercentage), MaxMana);
    }

    private void SetBaseStatsForClass()
    {
        var stats = Configuration.CharacterClassConfiguration.BaseStats.TryGetValue(Class, out var baseStats)
            ? baseStats
            : Configuration.CharacterClassConfiguration.DefaultStats;

        Strength = stats.Strength;
        Dexterity = stats.Dexterity;
        Constitution = stats.Constitution;
        Intelligence = stats.Intelligence;
        Wisdom = stats.Wisdom;
        Spirit = stats.Spirit;
        Luck = stats.Luck;
    }
}
