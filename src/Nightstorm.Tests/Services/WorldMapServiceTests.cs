using Nightstorm.Core.Services;
using Nightstorm.Core.Enums;

namespace Nightstorm.Tests.Services;

/// <summary>
/// Tests for world map navigation and zone management.
/// </summary>
[TestFixture]
public class WorldMapServiceTests
{
    private WorldMapService _mapService = null!;

    [SetUp]
    public void Setup()
    {
        _mapService = new WorldMapService();
    }

    [Test]
    public void WorldMap_HasCorrectTotalZones()
    {
        // Act
        var allZones = _mapService.GetAllZones();

        // Assert
        Assert.That(allZones.Count, Is.EqualTo(81), "Map should have exactly 81 zones (9x9)");
    }

    [Test]
    public void WorldMap_StartingZone_IsA1()
    {
        // Act
        var startingZone = _mapService.GetStartingZone();

        // Assert
        Assert.That(startingZone.ZoneId, Is.EqualTo("A1"));
        Assert.That(startingZone.Name, Is.EqualTo("Frostfall Citadel"));
        Assert.That(startingZone.DangerTier, Is.EqualTo(DangerTier.Civilized));
    }

    [Test]
    public void GetZone_WithValidId_ReturnsCorrectZone()
    {
        // Act
        var zone = _mapService.GetZone("E5");

        // Assert
        Assert.That(zone, Is.Not.Null);
        Assert.That(zone!.Name, Is.EqualTo("Scorpion's Gulch"));
        Assert.That(zone.Row, Is.EqualTo("E"));
        Assert.That(zone.Column, Is.EqualTo(5));
        Assert.That(zone.Biome, Is.EqualTo(BiomeType.BurningEquator));
    }

    [Test]
    public void GetZone_WithCoordinates_ReturnsCorrectZone()
    {
        // Act
        var zone = _mapService.GetZone("I", 9);

        // Assert
        Assert.That(zone, Is.Not.Null);
        Assert.That(zone!.ZoneId, Is.EqualTo("I9"));
        Assert.That(zone.Name, Is.EqualTo("World's End"));
    }

    [Test]
    public void GetZone_WithInvalidId_ReturnsNull()
    {
        // Act
        var zone = _mapService.GetZone("Z99");

        // Assert
        Assert.That(zone, Is.Null);
    }

    [Test]
    public void GetAdjacentZones_A1_HasSouthAndEastOnly()
    {
        // Act
        var adjacent = _mapService.GetAdjacentZones("A1");

        // Assert
        Assert.That(adjacent["North"], Is.Null, "A1 has no north neighbor");
        Assert.That(adjacent["West"], Is.Null, "A1 has no west neighbor");
        Assert.That(adjacent["South"]!.ZoneId, Is.EqualTo("B1"));
        Assert.That(adjacent["East"]!.ZoneId, Is.EqualTo("A2"));
    }

    [Test]
    public void GetAdjacentZones_E5_HasAllFourNeighbors()
    {
        // Act
        var adjacent = _mapService.GetAdjacentZones("E5");

        // Assert
        Assert.That(adjacent["North"]!.ZoneId, Is.EqualTo("D5"));
        Assert.That(adjacent["South"]!.ZoneId, Is.EqualTo("F5"));
        Assert.That(adjacent["East"]!.ZoneId, Is.EqualTo("E6"));
        Assert.That(adjacent["West"]!.ZoneId, Is.EqualTo("E4"));
    }

    [Test]
    public void GetAdjacentZones_I9_HasNorthAndWestOnly()
    {
        // Act
        var adjacent = _mapService.GetAdjacentZones("I9");

        // Assert
        Assert.That(adjacent["South"], Is.Null, "I9 has no south neighbor (map edge)");
        Assert.That(adjacent["East"], Is.Null, "I9 has no east neighbor (map edge)");
        Assert.That(adjacent["North"]!.ZoneId, Is.EqualTo("H9"));
        Assert.That(adjacent["West"]!.ZoneId, Is.EqualTo("I8"));
    }

    [Test]
    public void GetZones_FilterByDangerTier_Civilized()
    {
        // Act
        var civilizedZones = _mapService.GetZones(dangerTier: "Civilized");

        // Assert
        Assert.That(civilizedZones.Count, Is.EqualTo(27), "Should have 27 civilized zones (columns 1-3)");
        Assert.That(civilizedZones.All(z => z.DangerTier == DangerTier.Civilized), Is.True);
    }

    [Test]
    public void GetZones_FilterByBiome_NorthernFrost()
    {
        // Act
        var frostZones = _mapService.GetZones(biome: "NorthernFrost");

        // Assert
        Assert.That(frostZones.Count, Is.EqualTo(18), "Rows A and B have 9 zones each");
        Assert.That(frostZones.All(z => z.Biome == BiomeType.NorthernFrost), Is.True);
    }

    [Test]
    public void CanTravel_AdjacentSameLevel_Allowed()
    {
        // Arrange - A1 to A2, both civilized
        int characterLevel = 5;

        // Act
        bool canTravel = _mapService.CanTravel("A1", "A2", characterLevel);

        // Assert
        Assert.That(canTravel, Is.True, "Should allow travel between adjacent safe zones");
    }

    [Test]
    public void CanTravel_TooLowLevel_Denied()
    {
        // Arrange - A1 (level 1-11) to A7 (Ruined, high level)
        int characterLevel = 5;

        // Act
        bool canTravel = _mapService.CanTravel("A1", "A7", characterLevel);

        // Assert
        Assert.That(canTravel, Is.False, "Should deny travel - zones not adjacent");
    }

    [Test]
    public void CanTravel_NonAdjacentZones_Denied()
    {
        // Arrange - A1 to E5 (not adjacent)
        int characterLevel = 100;

        // Act
        bool canTravel = _mapService.CanTravel("A1", "E5", characterLevel);

        // Assert
        Assert.That(canTravel, Is.False, "Should deny travel to non-adjacent zones");
    }

    [Test]
    public void GetCapitalZones_ReturnsCorrectCount()
    {
        // Act
        var capitals = _mapService.GetCapitalZones();

        // Assert
        Assert.That(capitals.Count, Is.GreaterThanOrEqualTo(2), "Should have at least 2 capitals");
        Assert.That(capitals.Any(z => z.ZoneId == "A1"), Is.True, "A1 should be a capital");
        Assert.That(capitals.Any(z => z.ZoneId == "E1"), Is.True, "E1 should be a capital");
    }

    [Test]
    public void ZoneLevels_IncreaseWithLatitude()
    {
        // Act
        var a1 = _mapService.GetZone("A1")!;  // Northern Frost
        var e1 = _mapService.GetZone("E1")!;  // Burning Equator
        var i1 = _mapService.GetZone("I1")!;  // Southern Freeze

        // Assert
        Assert.That(e1.MinLevel, Is.GreaterThan(a1.MinLevel), "Equator should be higher level than north");
        Assert.That(i1.MinLevel, Is.GreaterThan(e1.MinLevel), "Southern should be higher level than equator");
    }

    [Test]
    public void ZoneLevels_IncreaseWithLongitude()
    {
        // Act
        var e1 = _mapService.GetZone("E1")!;  // Column 1 (Civilized)
        var e5 = _mapService.GetZone("E5")!;  // Column 5 (Wilderness)
        var e9 = _mapService.GetZone("E9")!;  // Column 9 (Ruined)

        // Assert
        Assert.That(e5.MinLevel, Is.GreaterThan(e1.MinLevel), "Wilderness should be higher level than civilized");
        Assert.That(e9.MinLevel, Is.GreaterThan(e5.MinLevel), "Ruined should be higher level than wilderness");
    }

    [Test]
    public void SpecialZones_AreMarkedCorrectly()
    {
        // Act
        var raidZone = _mapService.GetZone("E9")!;  // The Nightstorm Eye (Raid)
        var bossZone = _mapService.GetZone("B9")!;  // Yeti's Dominion (Boss Area)

        // Assert
        Assert.That(raidZone.IsSpecialZone, Is.True, "E9 should be marked as special (Raid)");
        Assert.That(bossZone.IsSpecialZone, Is.True, "B9 should be marked as special (Boss Area)");
    }

    [Test]
    public void PvpZones_AreSetCorrectly()
    {
        // Act
        var civilizedZone = _mapService.GetZone("A1")!;
        var wildernessZone = _mapService.GetZone("A4")!;
        var ruinedZone = _mapService.GetZone("A7")!;

        // Assert
        Assert.That(civilizedZone.IsPvpEnabled, Is.False, "Civilized zones should not have PvP");
        Assert.That(wildernessZone.IsPvpEnabled, Is.True, "Wilderness zones should have PvP");
        Assert.That(ruinedZone.IsPvpEnabled, Is.True, "Ruined zones should have PvP");
    }
    
    [Test]
    public void ZoneEnums_AreSetCorrectly()
    {
        // Act
        var a1 = _mapService.GetZone("A1")!;
        var e5 = _mapService.GetZone("E5")!;
        var e9 = _mapService.GetZone("E9")!;

        // Assert - Biomes
        Assert.That(a1.Biome, Is.EqualTo(BiomeType.NorthernFrost));
        Assert.That(e5.Biome, Is.EqualTo(BiomeType.BurningEquator));
        
        // Assert - Types
        Assert.That(a1.Type, Is.EqualTo(MapZoneType.Capital));
        Assert.That(e5.Type, Is.EqualTo(MapZoneType.Canyon));
        Assert.That(e9.Type, Is.EqualTo(MapZoneType.Raid));
        
        // Assert - Danger Tiers
        Assert.That(a1.DangerTier, Is.EqualTo(DangerTier.Civilized));
        Assert.That(e5.DangerTier, Is.EqualTo(DangerTier.Wilderness));
        Assert.That(e9.DangerTier, Is.EqualTo(DangerTier.Ruined));
    }
}
