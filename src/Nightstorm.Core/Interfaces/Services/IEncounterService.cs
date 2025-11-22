using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Represents a combat encounter with one or more monsters.
/// </summary>
public record Encounter
{
    /// <summary>
    /// Gets the unique identifier for this encounter.
    /// </summary>
    public Guid EncounterId { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// Gets the encounter name/description.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the zone where this encounter takes place.
    /// </summary>
    public required ZoneType Zone { get; init; }
    
    /// <summary>
    /// Gets the recommended level for this encounter.
    /// </summary>
    public required int RecommendedLevel { get; init; }
    
    /// <summary>
    /// Gets all monsters in this encounter.
    /// </summary>
    public required IReadOnlyList<Monster> Monsters { get; init; }
    
    /// <summary>
    /// Gets the boss monster if this is a boss encounter.
    /// </summary>
    public Monster? Boss { get; init; }
    
    /// <summary>
    /// Gets the raid boss if this is a raid encounter.
    /// </summary>
    public Monster? RaidBoss { get; init; }
    
    /// <summary>
    /// Gets the total difficulty rating of this encounter.
    /// </summary>
    public int DifficultyRating { get; init; }
    
    /// <summary>
    /// Gets the total experience reward for completing this encounter.
    /// </summary>
    public long TotalExperience { get; init; }
    
    /// <summary>
    /// Gets the total gold reward for completing this encounter.
    /// </summary>
    public int TotalGold { get; init; }
}

/// <summary>
/// Service for generating combat encounters.
/// </summary>
public interface IEncounterService
{
    /// <summary>
    /// Generates a random encounter appropriate for the character's level and zone.
    /// </summary>
    /// <param name="characterLevel">The character's level.</param>
    /// <param name="zone">The zone where the encounter takes place.</param>
    /// <param name="isBossEncounter">Whether to generate a boss encounter.</param>
    /// <param name="isRaidEncounter">Whether to generate a raid encounter.</param>
    /// <returns>A complete encounter with monsters.</returns>
    Encounter GenerateEncounter(
        int characterLevel,
        ZoneType zone,
        bool isBossEncounter = false,
        bool isRaidEncounter = false);
    
    /// <summary>
    /// Generates an encounter with specific monsters.
    /// </summary>
    /// <param name="templateIds">The template IDs to include in the encounter.</param>
    /// <param name="zone">The zone where the encounter takes place.</param>
    /// <returns>A complete encounter with the specified monsters.</returns>
    Encounter GenerateCustomEncounter(IEnumerable<string> templateIds, ZoneType zone);
    
    /// <summary>
    /// Generates a boss-only encounter.
    /// </summary>
    /// <param name="bossTemplateId">The boss template ID.</param>
    /// <param name="includeMinions">Whether to include minion monsters.</param>
    /// <returns>A boss encounter.</returns>
    Encounter GenerateBossEncounter(string bossTemplateId, bool includeMinions = false);
    
    /// <summary>
    /// Generates a raid-only encounter.
    /// </summary>
    /// <param name="raidBossTemplateId">The raid boss template ID.</param>
    /// <param name="includeMinions">Whether to include minion monsters.</param>
    /// <returns>A raid encounter.</returns>
    Encounter GenerateRaidEncounter(string raidBossTemplateId, bool includeMinions = false);
}
