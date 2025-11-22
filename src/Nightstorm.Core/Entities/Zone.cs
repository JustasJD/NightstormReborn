using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a game zone/area where monsters spawn.
/// Part of a 9x9 world map (81 zones total).
/// </summary>
public class Zone : BaseEntity
{
    /// <summary>
    /// Gets or sets the zone identifier (e.g., "A1", "E5", "I9").
    /// </summary>
    public string ZoneId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the zone name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the row coordinate (A-I).
    /// </summary>
    public string Row { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the column coordinate (1-9).
    /// </summary>
    public int Column { get; set; }
    
    /// <summary>
    /// Gets or sets the biome type.
    /// </summary>
    public BiomeType Biome { get; set; }
    
    /// <summary>
    /// Gets or sets the zone type (Capital, Town, Dungeon, etc.).
    /// </summary>
    public MapZoneType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the danger tier (Civilized, Frontier, Ruined).
    /// </summary>
    public DangerTier DangerTier { get; set; }
    
    /// <summary>
    /// Gets or sets the zone description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the minimum recommended level for this zone.
    /// Calculated based on danger tier and position.
    /// </summary>
    public int MinLevel { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum recommended level for this zone.
    /// Calculated based on danger tier and position.
    /// </summary>
    public int MaxLevel { get; set; }
    
    /// <summary>
    /// Gets or sets whether this zone is currently accessible.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether this is a special zone (boss/raid area).
    /// </summary>
    public bool IsSpecialZone { get; set; }
    
    /// <summary>
    /// Gets or sets whether this zone allows PvP combat.
    /// </summary>
    public bool IsPvpEnabled { get; set; }
}
