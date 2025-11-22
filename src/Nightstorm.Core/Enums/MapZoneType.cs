namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents the type/category of a map zone.
/// </summary>
public enum MapZoneType
{
    // Civilized Zones
    Capital = 1,
    City = 2,
    Town = 3,
    Village = 4,
    Settlement = 5,
    Fortress = 6,
    Hub = 7,
    Mine = 8,
    Industrial = 9,
    Camp = 10,
    Prison = 11,
    NavalBase = 12,
    Road = 13,
    
    // Wilderness Zones
    Wilderness = 20,
    Forest = 21,
    Plains = 22,
    Mountain = 23,
    Desert = 24,
    Swamp = 25,
    Lake = 26,
    Canyon = 27,
    Wasteland = 28,
    Crater = 29,
    
    // Dangerous Zones
    Dungeon = 40,
    Ruin = 41,
    BossArea = 42,
    Raid = 43,
    Anomaly = 44,
    Unknown = 45,
    Edge = 46
}
