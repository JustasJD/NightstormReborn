using Nightstorm.Core.Configuration;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Services;

/// <summary>
/// Implementation of world map navigation and zone management.
/// </summary>
public class WorldMapService : IWorldMapService
{
    private readonly Dictionary<string, Zone> _zoneCache;
    
    public WorldMapService()
    {
        // Build zone cache for fast lookups
        _zoneCache = WorldMapConfiguration.AllZones.ToDictionary(z => z.ZoneId, z => z);
    }
    
    /// <inheritdoc/>
    public Zone? GetZone(string zoneId)
    {
        return _zoneCache.GetValueOrDefault(zoneId);
    }
    
    /// <inheritdoc/>
    public Zone? GetZone(string row, int column)
    {
        string zoneId = $"{row}{column}";
        return GetZone(zoneId);
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<Zone> GetAllZones()
    {
        return WorldMapConfiguration.AllZones;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<Zone> GetZones(
        string? biome = null,
        string? dangerTier = null,
        int? minLevel = null,
        int? maxLevel = null)
    {
        var query = WorldMapConfiguration.AllZones.AsEnumerable();
        
        // Parse and filter by biome
        if (!string.IsNullOrEmpty(biome) && Enum.TryParse<BiomeType>(biome, true, out var biomeEnum))
        {
            query = query.Where(z => z.Biome == biomeEnum);
        }
        
        // Parse and filter by danger tier
        if (!string.IsNullOrEmpty(dangerTier) && Enum.TryParse<DangerTier>(dangerTier, true, out var dangerEnum))
        {
            query = query.Where(z => z.DangerTier == dangerEnum);
        }
        
        if (minLevel.HasValue)
            query = query.Where(z => z.MaxLevel >= minLevel.Value);
        
        if (maxLevel.HasValue)
            query = query.Where(z => z.MinLevel <= maxLevel.Value);
        
        return query.ToList();
    }
    
    /// <inheritdoc/>
    public Dictionary<string, Zone?> GetAdjacentZones(string zoneId)
    {
        var zone = GetZone(zoneId);
        if (zone == null)
            return new Dictionary<string, Zone?>();
        
        var adjacent = new Dictionary<string, Zone?>
        {
            ["North"] = GetAdjacentZone(zone.Row, zone.Column, -1, 0),
            ["South"] = GetAdjacentZone(zone.Row, zone.Column, 1, 0),
            ["East"] = GetAdjacentZone(zone.Row, zone.Column, 0, 1),
            ["West"] = GetAdjacentZone(zone.Row, zone.Column, 0, -1)
        };
        
        return adjacent;
    }
    
    /// <inheritdoc/>
    public bool CanTravel(string fromZoneId, string toZoneId, int characterLevel)
    {
        var fromZone = GetZone(fromZoneId);
        var toZone = GetZone(toZoneId);
        
        if (fromZone == null || toZone == null)
            return false;
        
        // Check if zones are adjacent
        var adjacent = GetAdjacentZones(fromZoneId);
        if (!adjacent.Values.Any(z => z?.ZoneId == toZoneId))
            return false;  // Not adjacent
        
        // Always allow travel to safer zones (lower danger tier number means safer)
        if ((int)toZone.DangerTier <= (int)fromZone.DangerTier)
            return true;
        
        // Allow travel to more dangerous zones if level is sufficient
        return characterLevel >= toZone.MinLevel;
    }
    
    /// <inheritdoc/>
    public Zone GetStartingZone()
    {
        return GetZone("A1") ?? throw new InvalidOperationException("Starting zone A1 not found");
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<Zone> GetCapitalZones()
    {
        return WorldMapConfiguration.AllZones
            .Where(z => z.Type == MapZoneType.Capital || z.Type == MapZoneType.City)
            .ToList();
    }
    
    #region Private Methods
    
    private Zone? GetAdjacentZone(string currentRow, int currentCol, int rowOffset, int colOffset)
    {
        // Calculate new row
        char rowChar = currentRow[0];
        char newRowChar = (char)(rowChar + rowOffset);
        
        if (newRowChar < 'A' || newRowChar > 'I')
            return null;  // Out of bounds
        
        // Calculate new column
        int newCol = currentCol + colOffset;
        if (newCol < 1 || newCol > 9)
            return null;  // Out of bounds
        
        return GetZone(newRowChar.ToString(), newCol);
    }
    
    #endregion
}
