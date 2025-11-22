using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Configuration;

/// <summary>
/// Static configuration for the 9x9 world map zones.
/// Contains all 81 zones with their properties and relationships.
/// </summary>
public static class WorldMapConfiguration
{
    /// <summary>
    /// World dimensions.
    /// </summary>
    public const int MapWidth = 9;
    public const int MapHeight = 9;
    public const int TotalZones = 81;
    
    /// <summary>
    /// Gets all 81 zones in the world map.
    /// </summary>
    public static IReadOnlyList<Zone> AllZones { get; } = BuildAllZones();
    
    private static List<Zone> BuildAllZones()
    {
        var zones = new List<Zone>(81);
        var zoneData = GetZoneRawData();
        
        foreach (var data in zoneData)
        {
            var biome = ParseBiome(data.biome);
            var type = ParseZoneType(data.type);
            var dangerTier = ParseDangerTier(data.danger_tier);
            
            zones.Add(new Zone
            {
                ZoneId = data.id,
                Name = data.name,
                Row = data.row,
                Column = data.col,
                Biome = biome,
                Type = type,
                DangerTier = dangerTier,
                Description = data.description,
                MinLevel = CalculateMinLevel(data.row, data.col, data.danger_tier),
                MaxLevel = CalculateMaxLevel(data.row, data.col, data.danger_tier),
                IsActive = true,
                IsSpecialZone = dangerTier == DangerTier.Ruined && (type == MapZoneType.Raid || type == MapZoneType.BossArea),
                IsPvpEnabled = dangerTier == DangerTier.Wilderness || dangerTier == DangerTier.Ruined
            });
        }
        
        return zones;
    }
    
    /// <summary>
    /// Calculates minimum level based on zone position and danger.
    /// </summary>
    private static int CalculateMinLevel(string row, int col, string dangerTier)
    {
        // Base level from latitude (row)
        int latitudeBase = row switch
        {
            "A" or "B" => 1,      // Northern Frost: 1-30
            "C" or "D" => 20,     // Iron Taiga: 20-60
            "E" => 40,            // Burning Equator: 40-80
            "F" or "G" => 60,     // Ashen Steppes: 60-100
            "H" or "I" => 80,     // Southern Freeze: 80-120
            _ => 1
        };
        
        // Add longitude progression (columns 1-9)
        int longitudeBonus = (col - 1) * 10;
        
        // Apply danger tier multiplier
        double dangerMultiplier = dangerTier switch
        {
            "Civilized" => 1.0,
            "Wilderness" => 1.3,
            "Ruined" => 1.6,
            _ => 1.0
        };
        
        return (int)Math.Max(1, (latitudeBase + longitudeBonus) * dangerMultiplier);
    }
    
    /// <summary>
    /// Calculates maximum level based on zone position and danger.
    /// </summary>
    private static int CalculateMaxLevel(string row, int col, string dangerTier)
    {
        int minLevel = CalculateMinLevel(row, col, dangerTier);
        
        // Max level is min + range based on danger tier
        int range = dangerTier switch
        {
            "Civilized" => 10,
            "Wilderness" => 20,
            "Ruined" => 50,  // Raid zones have wide level ranges
            _ => 10
        };
        
        return Math.Min(300, minLevel + range);
    }
    
    /// <summary>
    /// Raw zone data from the world JSON.
    /// </summary>
    private static (string id, string row, int col, string name, string biome, string type, string danger_tier, string description)[] GetZoneRawData()
    {
        return new[]
        {
            // Row A - Northern Frost
            ("A1", "A", 1, "Frostfall Citadel", "Northern Frost", "Capital", "Civilized", "Seat of Northern Power"),
            ("A2", "A", 2, "Winter's Harbor", "Northern Frost", "Town", "Civilized", "Trade Port"),
            ("A3", "A", 3, "Pine-Needle Town", "Northern Frost", "Settlement", "Civilized", "Logging"),
            ("A4", "A", 4, "The White Road", "Northern Frost", "Wilderness", "Wilderness", "Snowy Pass"),
            ("A5", "A", 5, "Mammoth Valley", "Northern Frost", "Wilderness", "Wilderness", "Beast Territory"),
            ("A6", "A", 6, "Shiver-Peak", "Northern Frost", "Mountain", "Wilderness", "Wyvern Roost"),
            ("A7", "A", 7, "Ruins of Borealis", "Northern Frost", "Ruin", "Ruined", "Frozen City"),
            ("A8", "A", 8, "The Lich's Spire", "Northern Frost", "Dungeon", "Ruined", "Undead Zone"),
            ("A9", "A", 9, "The Silent Glaciers", "Northern Frost", "Unknown", "Ruined", "Unmapped"),
            
            // Row B - Northern Frost
            ("B1", "B", 1, "Iron-Gate Keep", "Northern Frost", "Fortress", "Civilized", "Military Fort"),
            ("B2", "B", 2, "Snow-Melt Farms", "Northern Frost", "Settlement", "Civilized", "Hardy Crops"),
            ("B3", "B", 3, "Trapper's Ridge", "Northern Frost", "Settlement", "Civilized", "Fur Trade"),
            ("B4", "B", 4, "Wolf-Run Forest", "Northern Frost", "Forest", "Wilderness", "Dense Woods"),
            ("B5", "B", 5, "The Frozen Lake", "Northern Frost", "Lake", "Wilderness", "Ice Fishing"),
            ("B6", "B", 6, "Storm-Caller's Crag", "Northern Frost", "Mountain", "Wilderness", "Elemental Zone"),
            ("B7", "B", 7, "The Buried Library", "Northern Frost", "Dungeon", "Ruined", "Ancient Knowledge"),
            ("B8", "B", 8, "Cryomancer's Folly", "Northern Frost", "Ruin", "Ruined", "Magical Ice"),
            ("B9", "B", 9, "Yeti's Dominion", "Northern Frost", "Boss Area", "Ruined", "Boss Zone"),
            
            // Row C - Iron Taiga
            ("C1", "C", 1, "High-King's Road", "Iron Taiga", "Road", "Civilized", "Patrolled Route"),
            ("C2", "C", 2, "Grey-Stone Quarry", "Iron Taiga", "Mine", "Civilized", "Mining Hub"),
            ("C3", "C", 3, "The Watchtowers", "Iron Taiga", "Fortress", "Civilized", "Defense Line"),
            ("C4", "C", 4, "The Misty Taiga", "Iron Taiga", "Forest", "Wilderness", "Foggy Woods"),
            ("C5", "C", 5, "Elk-Stalker Plains", "Iron Taiga", "Plains", "Wilderness", "Hunting Grounds"),
            ("C6", "C", 6, "The Whispering Bog", "Iron Taiga", "Swamp", "Wilderness", "Haunted"),
            ("C7", "C", 7, "The Broken Wall", "Iron Taiga", "Ruin", "Ruined", "Old War Site"),
            ("C8", "C", 8, "Shadow-Moss Woods", "Iron Taiga", "Forest", "Ruined", "Poisonous"),
            ("C9", "C", 9, "The Void-Scar", "Iron Taiga", "Anomaly", "Ruined", "Magical Anomaly"),
            
            // Row D - Iron Taiga
            ("D1", "D", 1, "Merchant's Guild", "Iron Taiga", "Hub", "Civilized", "Trade Hub"),
            ("D2", "D", 2, "River Cross Town", "Iron Taiga", "Town", "Civilized", "Ferry Crossing"),
            ("D3", "D", 3, "The Mud-Flats", "Iron Taiga", "Village", "Civilized", "Poor Village"),
            ("D4", "D", 4, "Savanna Edge", "Iron Taiga", "Plains", "Wilderness", "Transition Zone"),
            ("D5", "D", 5, "The Dry Woods", "Iron Taiga", "Forest", "Wilderness", "Fire Risk"),
            ("D6", "D", 6, "Goblin Scrapyard", "Iron Taiga", "Camp", "Wilderness", "Raiders"),
            ("D7", "D", 7, "Temple of Dust", "Iron Taiga", "Dungeon", "Ruined", "Cultists"),
            ("D8", "D", 8, "The Petroglyphs", "Iron Taiga", "Ruin", "Ruined", "Eldritch Lore"),
            ("D9", "D", 9, "Canyon of Screams", "Iron Taiga", "Canyon", "Ruined", "Sonic Wind"),
            
            // Row E - Burning Equator
            ("E1", "E", 1, "The Gilded Oasis", "Burning Equator", "Capital", "Civilized", "Rich Capital"),
            ("E2", "E", 2, "Sun-Dial City", "Burning Equator", "City", "Civilized", "Astronomy"),
            ("E3", "E", 3, "Sand-Sailor Port", "Burning Equator", "Town", "Civilized", "Dune Ships"),
            ("E4", "E", 4, "The Red Dunes", "Burning Equator", "Desert", "Wilderness", "Hot Zone"),
            ("E5", "E", 5, "Scorpion's Gulch", "Burning Equator", "Canyon", "Wilderness", "Monsters"),
            ("E6", "E", 6, "The Mirage Fields", "Burning Equator", "Desert", "Wilderness", "Illusions"),
            ("E7", "E", 7, "The Glass Crater", "Burning Equator", "Ruin", "Ruined", "Nuclear Site"),
            ("E8", "E", 8, "Tomb of Kings", "Burning Equator", "Dungeon", "Ruined", "Mummies"),
            ("E9", "E", 9, "The Nightstorm Eye", "Burning Equator", "Raid", "Ruined", "Raid Zone"),
            
            // Row F - Ashen Steppes
            ("F1", "F", 1, "Steel-Foundry", "Ashen Steppes", "Industrial", "Civilized", "Industry"),
            ("F2", "F", 2, "Dust-Bowl Farms", "Ashen Steppes", "Settlement", "Civilized", "Agriculture"),
            ("F3", "F", 3, "Mercenary Camp", "Ashen Steppes", "Camp", "Civilized", "Lawless Safezone"),
            ("F4", "F", 4, "The Cracked Earth", "Ashen Steppes", "Wasteland", "Wilderness", "Earthquakes"),
            ("F5", "F", 5, "Vulture's Perch", "Ashen Steppes", "Mountain", "Wilderness", "Scavengers"),
            ("F6", "F", 6, "Basilisk Nests", "Ashen Steppes", "Wasteland", "Wilderness", "Petrification"),
            ("F7", "F", 7, "The Rusted Colossus", "Ashen Steppes", "Ruin", "Ruined", "Giant Mech"),
            ("F8", "F", 8, "Plague-Wind Gap", "Ashen Steppes", "Canyon", "Ruined", "Disease"),
            ("F9", "F", 9, "The Ashen Waste", "Ashen Steppes", "Wasteland", "Ruined", "Volcanic Ash"),
            
            // Row G - Ashen Steppes
            ("G1", "G", 1, "Storm-Break Wall", "Ashen Steppes", "Fortress", "Civilized", "Shield Tech"),
            ("G2", "G", 2, "The Grey Abbey", "Ashen Steppes", "Settlement", "Civilized", "Monks"),
            ("G3", "G", 3, "South-Road Inn", "Ashen Steppes", "Settlement", "Civilized", "Safe House"),
            ("G4", "G", 4, "The Cold Steppe", "Ashen Steppes", "Plains", "Wilderness", "Open Plains"),
            ("G5", "G", 5, "Thunder-Hoof Plains", "Ashen Steppes", "Plains", "Wilderness", "Bison Herds"),
            ("G6", "G", 6, "The Howling Caves", "Ashen Steppes", "Dungeon", "Wilderness", "Trolls"),
            ("G7", "G", 7, "Gargoyle's Keep", "Ashen Steppes", "Ruin", "Ruined", "Abandoned"),
            ("G8", "G", 8, "The Soul Well", "Ashen Steppes", "Dungeon", "Ruined", "Necromancy"),
            ("G9", "G", 9, "Entropy's Edge", "Ashen Steppes", "Anomaly", "Ruined", "Reality Break"),
            
            // Row H - Southern Freeze
            ("H1", "H", 1, "Ice-Breaker Bay", "Southern Freeze", "Naval Base", "Civilized", "Naval Base"),
            ("H2", "H", 2, "Penguin Coast", "Southern Freeze", "Settlement", "Civilized", "Fishery"),
            ("H3", "H", 3, "The Long Dark", "Southern Freeze", "Wilderness", "Civilized", "No Sun"),
            ("H4", "H", 4, "Floe-Walker Path", "Southern Freeze", "Wilderness", "Wilderness", "Moving Ice"),
            ("H5", "H", 5, "Aurora Ridge", "Southern Freeze", "Mountain", "Wilderness", "Magic Sky"),
            ("H6", "H", 6, "The Razor Ice", "Southern Freeze", "Wasteland", "Wilderness", "Sharp Terrain"),
            ("H7", "H", 7, "The Sunken City", "Southern Freeze", "Ruin", "Ruined", "Underwater"),
            ("H8", "H", 8, "Vampire's Fjord", "Southern Freeze", "Dungeon", "Ruined", "Blood Snow"),
            ("H9", "H", 9, "The Absolute Zero", "Southern Freeze", "Unknown", "Ruined", "Cold Damage"),
            
            // Row I - Southern Freeze
            ("I1", "I", 1, "The Last Bastion", "Southern Freeze", "Fortress", "Civilized", "End of Civilization"),
            ("I2", "I", 2, "Deep-Core Mine", "Southern Freeze", "Mine", "Civilized", "Rare Ore"),
            ("I3", "I", 3, "Exile's Rock", "Southern Freeze", "Prison", "Civilized", "Prison"),
            ("I4", "I", 4, "The White Void", "Southern Freeze", "Wilderness", "Wilderness", "Blizzard"),
            ("I5", "I", 5, "Leviathan's Rest", "Southern Freeze", "Ruin", "Wilderness", "Giant Skeleton"),
            ("I6", "I", 6, "Titan's Footprint", "Southern Freeze", "Crater", "Wilderness", "Crater"),
            ("I7", "I", 7, "The Forgotten Lab", "Southern Freeze", "Dungeon", "Ruined", "Sci-Fi Ruins"),
            ("I8", "I", 8, "Gateway to Hell", "Southern Freeze", "Anomaly", "Ruined", "Demon Gate"),
            ("I9", "I", 9, "World's End", "Southern Freeze", "Edge", "Ruined", "Map Edge")
        };
    }
    
    /// <summary>
    /// Parses string biome to enum.
    /// </summary>
    private static BiomeType ParseBiome(string biome)
    {
        return biome switch
        {
            "Northern Frost" => BiomeType.NorthernFrost,
            "Iron Taiga" => BiomeType.IronTaiga,
            "Burning Equator" => BiomeType.BurningEquator,
            "Ashen Steppes" => BiomeType.AshenSteppes,
            "Southern Freeze" => BiomeType.SouthernFreeze,
            _ => BiomeType.NorthernFrost
        };
    }
    
    /// <summary>
    /// Parses string zone type to enum.
    /// </summary>
    private static MapZoneType ParseZoneType(string type)
    {
        return type switch
        {
            "Capital" => MapZoneType.Capital,
            "City" => MapZoneType.City,
            "Town" => MapZoneType.Town,
            "Village" => MapZoneType.Village,
            "Settlement" => MapZoneType.Settlement,
            "Fortress" => MapZoneType.Fortress,
            "Hub" => MapZoneType.Hub,
            "Mine" => MapZoneType.Mine,
            "Industrial" => MapZoneType.Industrial,
            "Camp" => MapZoneType.Camp,
            "Prison" => MapZoneType.Prison,
            "Naval Base" => MapZoneType.NavalBase,
            "Road" => MapZoneType.Road,
            "Wilderness" => MapZoneType.Wilderness,
            "Forest" => MapZoneType.Forest,
            "Plains" => MapZoneType.Plains,
            "Mountain" => MapZoneType.Mountain,
            "Desert" => MapZoneType.Desert,
            "Swamp" => MapZoneType.Swamp,
            "Lake" => MapZoneType.Lake,
            "Canyon" => MapZoneType.Canyon,
            "Wasteland" => MapZoneType.Wasteland,
            "Crater" => MapZoneType.Crater,
            "Dungeon" => MapZoneType.Dungeon,
            "Ruin" => MapZoneType.Ruin,
            "Boss Area" => MapZoneType.BossArea,
            "Raid" => MapZoneType.Raid,
            "Anomaly" => MapZoneType.Anomaly,
            "Unknown" => MapZoneType.Unknown,
            "Edge" => MapZoneType.Edge,
            _ => MapZoneType.Unknown
        };
    }
    
    /// <summary>
    /// Parses string danger tier to enum.
    /// </summary>
    private static DangerTier ParseDangerTier(string dangerTier)
    {
        return dangerTier switch
        {
            "Civilized" => DangerTier.Civilized,
            "Wilderness" => DangerTier.Wilderness,
            "Ruined" => DangerTier.Ruined,
            _ => DangerTier.Civilized
        };
    }
}
