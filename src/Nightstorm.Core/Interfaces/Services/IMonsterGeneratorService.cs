using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for generating monsters from templates with level-scaled stats.
/// </summary>
public interface IMonsterGeneratorService
{
    /// <summary>
    /// Generates a monster from a template ID.
    /// </summary>
    /// <param name="templateId">The template identifier.</param>
    /// <param name="level">Optional level override (uses template level if not specified).</param>
    /// <returns>A fully-stat generated Monster instance.</returns>
    Monster GenerateMonster(string templateId, int? level = null);
    
    /// <summary>
    /// Generates a random monster appropriate for the specified level.
    /// </summary>
    /// <param name="targetLevel">The target level range.</param>
    /// <param name="difficulty">Optional difficulty filter.</param>
    /// <param name="zone">Optional zone restriction.</param>
    /// <returns>A randomly selected and generated monster.</returns>
    Monster GenerateRandomMonster(int targetLevel, MonsterDifficulty? difficulty = null, ZoneType? zone = null);
    
    /// <summary>
    /// Gets all available monster templates.
    /// </summary>
    /// <returns>Collection of all monster templates.</returns>
    IReadOnlyList<Configuration.MonsterTemplate> GetAllTemplates();
    
    /// <summary>
    /// Gets templates filtered by criteria.
    /// </summary>
    /// <param name="minLevel">Minimum level.</param>
    /// <param name="maxLevel">Maximum level.</param>
    /// <param name="difficulty">Optional difficulty filter.</param>
    /// <param name="zone">Optional zone filter.</param>
    /// <returns>Filtered collection of templates.</returns>
    IReadOnlyList<Configuration.MonsterTemplate> GetTemplates(
        int? minLevel = null,
        int? maxLevel = null,
        MonsterDifficulty? difficulty = null,
        ZoneType? zone = null);
}
