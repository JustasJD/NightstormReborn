using Nightstorm.Core.Entities;

namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for managing world map navigation and zone information.
/// </summary>
public interface IWorldMapService
{
    /// <summary>
    /// Gets a zone by its zone ID (e.g., "A1", "E5").
    /// </summary>
    /// <param name="zoneId">The zone identifier.</param>
    /// <returns>The zone if found, null otherwise.</returns>
    Zone? GetZone(string zoneId);
    
    /// <summary>
    /// Gets a zone by row and column coordinates.
    /// </summary>
    /// <param name="row">The row (A-I).</param>
    /// <param name="column">The column (1-9).</param>
    /// <returns>The zone if found, null otherwise.</returns>
    Zone? GetZone(string row, int column);
    
    /// <summary>
    /// Gets all zones in the world map.
    /// </summary>
    /// <returns>Collection of all 81 zones.</returns>
    IReadOnlyList<Zone> GetAllZones();
    
    /// <summary>
    /// Gets zones filtered by criteria.
    /// </summary>
    /// <param name="biome">Optional biome filter.</param>
    /// <param name="dangerTier">Optional danger tier filter.</param>
    /// <param name="minLevel">Minimum level requirement.</param>
    /// <param name="maxLevel">Maximum level requirement.</param>
    /// <returns>Filtered collection of zones.</returns>
    IReadOnlyList<Zone> GetZones(
        string? biome = null,
        string? dangerTier = null,
        int? minLevel = null,
        int? maxLevel = null);
    
    /// <summary>
    /// Gets adjacent zones (north, south, east, west).
    /// </summary>
    /// <param name="zoneId">The current zone ID.</param>
    /// <returns>Dictionary of direction to zone (if exists).</returns>
    Dictionary<string, Zone?> GetAdjacentZones(string zoneId);
    
    /// <summary>
    /// Checks if travel between two zones is allowed.
    /// </summary>
    /// <param name="from ZoneId">The starting zone ID.</param>
    /// <param name="toZoneId">The destination zone ID.</param>
    /// <param name="characterLevel">The character's level.</param>
    /// <returns>True if travel is allowed, false otherwise.</returns>
    bool CanTravel(string fromZoneId, string toZoneId, int characterLevel);
    
    /// <summary>
    /// Gets the starting zone for new characters.
    /// </summary>
    /// <returns>The starting zone (A1 - Frostfall Citadel).</returns>
    Zone GetStartingZone();
    
    /// <summary>
    /// Gets all capital/major cities.
    /// </summary>
    /// <returns>Collection of capital zones.</returns>
    IReadOnlyList<Zone> GetCapitalZones();
}
