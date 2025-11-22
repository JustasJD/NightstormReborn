namespace Nightstorm.Core.Enums;

/// <summary>
/// Represents different zones/areas in the game world.
/// Each zone has a level range and specific monster spawn tables.
/// </summary>
public enum ZoneType
{
    /// <summary>
    /// Starting zone - Levels 1-10
    /// Cave Goblins, Kobolds, Slimes, Skeletons, Rats, Wolves
    /// </summary>
    WhisperingWoods = 1,
    
    /// <summary>
    /// Early zone - Levels 5-15
    /// Exploders, Sprites, Spiders, Eyes, Imps, Gnolls
    /// </summary>
    CrystalMeadows = 2,
    
    /// <summary>
    /// Mid-early zone - Levels 10-25
    /// Harpies, Cockatrices, Gelatinous Cubes, Mimics, Ghouls
    /// </summary>
    TwilightMarsh = 3,
    
    /// <summary>
    /// Mid zone - Levels 20-40
    /// Giants, Chimeras, Golems, Treants
    /// </summary>
    StormpeakMountains = 4,
    
    /// <summary>
    /// Mid-high zone - Levels 35-60
    /// Drow, Fire/Frost/Storm Giants, Demons, Medusas
    /// </summary>
    UnderdarkCaverns = 5,
    
    /// <summary>
    /// High zone - Levels 50-80
    /// Behemoths, Dragons, Purple Worms, Phoenixes
    /// </summary>
    DragonspireVolcano = 6,
    
    /// <summary>
    /// Very high zone - Levels 75-120
    /// Ancient Dragons, Tonberry Kings, Jumbo Cactuars
    /// </summary>
    AstralPlane = 7,
    
    /// <summary>
    /// Endgame zone - Levels 100-180
    /// Elder Wyrms, Solars, Primal Elementals
    /// </summary>
    VoidCitadel = 8,
    
    /// <summary>
    /// Late endgame zone - Levels 150-250
    /// Cosmic Horrors, Time Phantoms, Crystal Guardians
    /// </summary>
    EternalAbyss = 9,
    
    /// <summary>
    /// Ultimate zone - Levels 200-300
    /// Guardian Forces, Error Entities, Energy Beings, Gods
    /// </summary>
    PrimordialChaos = 10,
    
    /// <summary>
    /// Special boss encounter zone - All boss levels
    /// Boss-only encounters, instanced fights
    /// </summary>
    BossLair = 99,
    
    /// <summary>
    /// Special raid zone - Raid bosses only
    /// Raid-only encounters, requires groups
    /// </summary>
    RaidNexus = 100
}
