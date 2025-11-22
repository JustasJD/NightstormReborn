using Nightstorm.Core.Enums;

namespace Nightstorm.Core.Configuration;

/// <summary>
/// Static configuration for all monster templates in the game.
/// Contains 145 monster definitions: 85 normal, 50 bosses, 10 raid bosses.
/// </summary>
public static class MonsterTemplateConfiguration
{
    /// <summary>
    /// Gets all monster templates in the game.
    /// </summary>
    public static IReadOnlyList<MonsterTemplate> AllTemplates { get; } = BuildAllTemplates();

    private static List<MonsterTemplate> BuildAllTemplates()
    {
        var templates = new List<MonsterTemplate>(145);
        
        // Add all monster categories
        templates.AddRange(GetNormalMonsters());
        templates.AddRange(GetBossMonsters());
        templates.AddRange(GetRaidBossMonsters());
        
        return templates;
    }

    #region Normal Monsters (85 templates)
    
    private static IEnumerable<MonsterTemplate> GetNormalMonsters()
    {
        return new[]
        {
            // Levels 1-10: Whispering Woods
            CreateNormal("cave-goblin", "Cave Goblin", MonsterType.Humanoid, 1, 
                ZoneType.WhisperingWoods, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("kobold-trapmaker", "Kobold Trapmaker", MonsterType.Humanoid, 1,
                ZoneType.WhisperingWoods, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("green-slime", "Green Slime", MonsterType.Ooze, 2,
                ZoneType.WhisperingWoods, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("blue-pudding", "Blue Pudding", MonsterType.Ooze, 2,
                ZoneType.WhisperingWoods, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateNormal("skeletal-footman", "Skeletal Footman", MonsterType.Undead, 3,
                ZoneType.WhisperingWoods, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("walking-fungus", "Walking Fungus", MonsterType.Plant, 3,
                ZoneType.WhisperingWoods, AttackType.SpiritualMagic, ArmorType.Light),
            CreateNormal("giant-rat", "Giant Rat", MonsterType.Beast, 4,
                ZoneType.WhisperingWoods, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("dire-wolf", "Dire Wolf", MonsterType.Beast, 4,
                ZoneType.WhisperingWoods, AttackType.FastMelee, ArmorType.Light),
            
            // Levels 5-15: Crystal Meadows
            CreateNormal("exploder-small", "Exploder (Small)", MonsterType.Elemental, 5,
                ZoneType.CrystalMeadows, AttackType.ElementalMagic, ArmorType.Light),
            CreateNormal("needle-sprite", "Needle Sprite", MonsterType.Plant, 5,
                ZoneType.CrystalMeadows, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("lantern-stalker-small", "Lantern Stalker (Small)", MonsterType.Aberration, 6,
                ZoneType.CrystalMeadows, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("fishman-scout", "Fish-Man Scout", MonsterType.Humanoid, 6,
                ZoneType.CrystalMeadows, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("giant-spider", "Giant Spider", MonsterType.Beast, 7,
                ZoneType.CrystalMeadows, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("winged-eye", "Winged Eye", MonsterType.Aberration, 7,
                ZoneType.CrystalMeadows, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateNormal("imp", "Imp", MonsterType.Fiend, 8,
                ZoneType.CrystalMeadows, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateNormal("worg", "Worg", MonsterType.Monstrosity, 9,
                ZoneType.CrystalMeadows, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("hyenaman", "Hyena-Man", MonsterType.Humanoid, 10,
                ZoneType.CrystalMeadows, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("hairy-bugbear", "Hairy Bug-Bear", MonsterType.Humanoid, 10,
                ZoneType.CrystalMeadows, AttackType.HeavyMelee, ArmorType.Heavy),
            
            // Levels 10-25: Twilight Marsh
            CreateNormal("harpy", "Harpy", MonsterType.Monstrosity, 11,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("stone-gaze-bird", "Stone Gaze Bird", MonsterType.Monstrosity, 12,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("stone-gaze-lizard", "Stone Gaze Lizard", MonsterType.Monstrosity, 12,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Heavy),
            CreateNormal("metal-eater", "Metal-Eater", MonsterType.Monstrosity, 13,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Heavy),
            CreateNormal("gelatinous-cube", "Gelatinous Cube", MonsterType.Ooze, 14,
                ZoneType.TwilightMarsh, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("mimic", "Mimic", MonsterType.Monstrosity, 15,
                ZoneType.TwilightMarsh, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("ghoul", "Ghoul", MonsterType.Undead, 15,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("phase-cat", "Phase Cat", MonsterType.Monstrosity, 16,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("owlbear", "Owl-Bear", MonsterType.Monstrosity, 17,
                ZoneType.TwilightMarsh, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("griffon", "Griffon", MonsterType.Monstrosity, 18,
                ZoneType.TwilightMarsh, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("minotaur", "Minotaur", MonsterType.Monstrosity, 19,
                ZoneType.TwilightMarsh, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("ogre", "Ogre", MonsterType.Giant, 20,
                ZoneType.TwilightMarsh, AttackType.HeavyMelee, ArmorType.Heavy),
            
            // Levels 20-40: Stormpeak Mountains
            CreateNormal("hill-giant", "Hill Giant", MonsterType.Giant, 22,
                ZoneType.StormpeakMountains, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("bad-breath-plant", "Bad Breath Plant", MonsterType.Plant, 23,
                ZoneType.StormpeakMountains, AttackType.SpiritualMagic, ArmorType.Light),
            CreateNormal("blaster-leopard", "Blaster Leopard", MonsterType.Monstrosity, 24,
                ZoneType.StormpeakMountains, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("chimera", "Chimera", MonsterType.Monstrosity, 25,
                ZoneType.StormpeakMountains, AttackType.ElementalMagic, ArmorType.Light),
            CreateNormal("iron-golem", "Iron Golem", MonsterType.Construct, 28,
                ZoneType.StormpeakMountains, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("treant", "Treant", MonsterType.Plant, 30,
                ZoneType.StormpeakMountains, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("brain-eater", "Brain Eater", MonsterType.Aberration, 32,
                ZoneType.StormpeakMountains, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("tunnel-hulk", "Tunnel Hulk", MonsterType.Monstrosity, 33,
                ZoneType.StormpeakMountains, AttackType.HeavyMelee, ArmorType.Heavy),
            
            // Levels 35-60: Underdark Caverns
            CreateNormal("drow-warrior", "Drow Warrior", MonsterType.Humanoid, 35,
                ZoneType.UnderdarkCaverns, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("fire-giant", "Fire Giant", MonsterType.Giant, 38,
                ZoneType.UnderdarkCaverns, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("frost-giant", "Frost Giant", MonsterType.Giant, 40,
                ZoneType.UnderdarkCaverns, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("storm-giant", "Storm Giant", MonsterType.Giant, 42,
                ZoneType.UnderdarkCaverns, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("oni", "Oni", MonsterType.Giant, 45,
                ZoneType.UnderdarkCaverns, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateNormal("tiger-demon", "Tiger Demon", MonsterType.Fiend, 48,
                ZoneType.UnderdarkCaverns, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("snake-haired-lady", "Snake-Haired Lady", MonsterType.Monstrosity, 50,
                ZoneType.UnderdarkCaverns, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("metal-bull", "Metal Bull", MonsterType.Monstrosity, 52,
                ZoneType.UnderdarkCaverns, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("death-gaze-cow", "Death Gaze Cow", MonsterType.Monstrosity, 55,
                ZoneType.UnderdarkCaverns, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("giant-turtle", "Giant Turtle", MonsterType.Beast, 56,
                ZoneType.UnderdarkCaverns, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("wraith", "Wraith", MonsterType.Undead, 58,
                ZoneType.UnderdarkCaverns, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("mummy-lord", "Mummy Lord", MonsterType.Undead, 59,
                ZoneType.UnderdarkCaverns, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("genie", "Genie (Fire/Wind)", MonsterType.Elemental, 60,
                ZoneType.UnderdarkCaverns, AttackType.ElementalMagic, ArmorType.Cloth),
            
            // Levels 50-80: Dragonspire Volcano
            CreateNormal("behemoth", "Behemoth", MonsterType.Monstrosity, 62,
                ZoneType.DragonspireVolcano, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("steel-titan", "Steel Titan", MonsterType.Construct, 65,
                ZoneType.DragonspireVolcano, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("adult-red-dragon", "Adult Red Dragon", MonsterType.Dragon, 68,
                ZoneType.DragonspireVolcano, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("adult-blue-dragon", "Adult Blue Dragon", MonsterType.Dragon, 70,
                ZoneType.DragonspireVolcano, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("purple-worm", "Purple Worm", MonsterType.Monstrosity, 72,
                ZoneType.DragonspireVolcano, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("abyssal-squid", "Abyssal Squid", MonsterType.Aberration, 75,
                ZoneType.DragonspireVolcano, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("primordial-fish", "Primordial Fish", MonsterType.Aberration, 78,
                ZoneType.DragonspireVolcano, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("sea-serpent", "Sea Serpent", MonsterType.Beast, 80,
                ZoneType.DragonspireVolcano, AttackType.FastMelee, ArmorType.Light),
            
            // Levels 75-120: Astral Plane
            CreateNormal("phoenix", "Phoenix", MonsterType.Celestial, 82,
                ZoneType.AstralPlane, AttackType.ElementalMagic, ArmorType.Light),
            CreateNormal("chaos-frog-red", "Chaos Frog (Red)", MonsterType.Aberration, 85,
                ZoneType.AstralPlane, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("astral-knight", "Astral Knight", MonsterType.Humanoid, 88,
                ZoneType.AstralPlane, AttackType.FastMelee, ArmorType.Heavy),
            CreateNormal("brain-hive-mind", "Brain Hive Mind", MonsterType.Aberration, 90,
                ZoneType.AstralPlane, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("balor-demon", "Balor Demon", MonsterType.Fiend, 92,
                ZoneType.AstralPlane, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateNormal("pit-fiend-devil", "Pit Fiend Devil", MonsterType.Fiend, 95,
                ZoneType.AstralPlane, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("six-armed-demon", "Six-Armed Demon", MonsterType.Fiend, 99,
                ZoneType.AstralPlane, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("star-spawn", "Star Spawn", MonsterType.Aberration, 100,
                ZoneType.AstralPlane, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("ancient-red-dragon", "Ancient Red Dragon", MonsterType.Dragon, 105,
                ZoneType.AstralPlane, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("undead-dragon", "Undead Dragon", MonsterType.Undead, 110,
                ZoneType.AstralPlane, AttackType.SpiritualMagic, ArmorType.Heavy),
            CreateNormal("giant-bad-breath", "Giant Bad Breath Plant", MonsterType.Plant, 115,
                ZoneType.AstralPlane, AttackType.SpiritualMagic, ArmorType.Light),
            CreateNormal("lantern-stalker-king", "Lantern Stalker King", MonsterType.Aberration, 120,
                ZoneType.AstralPlane, AttackType.FastMelee, ArmorType.Light),
            
            // Levels 100-180: Void Citadel
            CreateNormal("giant-needle-sprite", "Giant Needle Sprite", MonsterType.Plant, 125,
                ZoneType.VoidCitadel, AttackType.FastMelee, ArmorType.Light),
            CreateNormal("elixir-pot", "Elixir Pot", MonsterType.Construct, 130,
                ZoneType.VoidCitadel, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("samurai-lord", "Samurai Lord", MonsterType.Humanoid, 135,
                ZoneType.VoidCitadel, AttackType.FastMelee, ArmorType.Heavy),
            CreateNormal("magitek-armor", "Magitek Armor", MonsterType.Construct, 140,
                ZoneType.VoidCitadel, AttackType.RangedHybrid, ArmorType.Heavy),
            CreateNormal("elder-wyrm", "Elder Wyrm", MonsterType.Dragon, 150,
                ZoneType.VoidCitadel, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("solar-angel", "Solar Angel", MonsterType.Celestial, 160,
                ZoneType.VoidCitadel, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateNormal("astral-dreadnought", "Astral Dreadnought", MonsterType.Monstrosity, 170,
                ZoneType.VoidCitadel, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("death-walker", "Death Walker", MonsterType.Undead, 180,
                ZoneType.VoidCitadel, AttackType.SpiritualMagic, ArmorType.Cloth),
            
            // Levels 150-250: Eternal Abyss
            CreateNormal("force-dragon", "Force Dragon", MonsterType.Dragon, 190,
                ZoneType.EternalAbyss, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("diamond-construct", "Diamond Construct", MonsterType.Construct, 195,
                ZoneType.EternalAbyss, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("prismatic-dragon", "Prismatic Dragon", MonsterType.Dragon, 200,
                ZoneType.EternalAbyss, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateNormal("cosmic-horror", "Cosmic Horror", MonsterType.Aberration, 210,
                ZoneType.EternalAbyss, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("worm-that-walks", "Worm that Walks", MonsterType.Aberration, 220,
                ZoneType.EternalAbyss, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("time-phantom", "Time Phantom", MonsterType.Undead, 230,
                ZoneType.EternalAbyss, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("primal-fire-elemental", "Primal Fire Elemental", MonsterType.Elemental, 240,
                ZoneType.EternalAbyss, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateNormal("crystal-guardian", "Crystal Guardian", MonsterType.Construct, 250,
                ZoneType.EternalAbyss, AttackType.HeavyMelee, ArmorType.Heavy),
            
            // Levels 200-300: Primordial Chaos
            CreateNormal("undead-behemoth", "Undead Behemoth", MonsterType.Undead, 255,
                ZoneType.PrimordialChaos, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("guardian-force", "Guardian Force", MonsterType.Celestial, 260,
                ZoneType.PrimordialChaos, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateNormal("doom-train", "Doom Train", MonsterType.Undead, 270,
                ZoneType.PrimordialChaos, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("reapers-shadow", "The Reaper's Shadow", MonsterType.Undead, 280,
                ZoneType.PrimordialChaos, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("void-walker", "Void Walker", MonsterType.Aberration, 285,
                ZoneType.PrimordialChaos, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateNormal("error-entity", "Error Entity", MonsterType.Abstract, 290,
                ZoneType.PrimordialChaos, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateNormal("neutron-star-golem", "Neutron Star Golem", MonsterType.Construct, 295,
                ZoneType.PrimordialChaos, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateNormal("pure-energy-being", "Pure Energy Being", MonsterType.Elemental, 299,
                ZoneType.PrimordialChaos, AttackType.ElementalMagic, ArmorType.Cloth),
        };
    }
    
    #endregion

    #region Boss Monsters (50 templates)
    
    private static IEnumerable<MonsterTemplate> GetBossMonsters()
    {
        return new[]
        {
            // Early Bosses (5-40)
            CreateBoss("fallen-knight", "The Fallen Knight", MonsterType.Humanoid, 5,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("hobgoblin-warlord", "Hobgoblin Warlord", MonsterType.Humanoid, 8,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("king-explosions", "King of Explosions", MonsterType.Elemental, 12,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Light),
            CreateBoss("vampire-count", "The Vampire Count", MonsterType.Undead, 15,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("lecherous-octopus", "The Lecherous Octopus", MonsterType.Monstrosity, 18,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Light),
            CreateBoss("eye-tyrant-hive", "Eye Tyrant Hive Mother", MonsterType.Aberration, 20,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("earth-fiend-lich", "Earth Fiend Lich", MonsterType.Undead, 25,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("six-armed-snake", "Six-Armed Snake Demon", MonsterType.Fiend, 30,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Light),
            CreateBoss("abyssal-kraken", "Abyssal Kraken", MonsterType.Monstrosity, 35,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Heavy),
            CreateBoss("wind-fiend-avatar", "Wind Fiend Avatar", MonsterType.Fiend, 40,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            
            // Mid Bosses (45-100)
            CreateBoss("shadow-clad-mage", "The Shadow-Clad Mage", MonsterType.Humanoid, 45,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("crime-lord-eye", "The Crime-Lord Eye", MonsterType.Aberration, 50,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("dimension-samurai", "The Dimension-Hopping Samurai", MonsterType.Humanoid, 55,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Heavy),
            CreateBoss("two-headed-prince", "The Two-Headed Prince", MonsterType.Fiend, 60,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateBoss("mad-harlequin", "The Mad Harlequin", MonsterType.Humanoid, 65,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("octopus-windbeast", "The Octopus & The Wind Beast", MonsterType.Monstrosity, 70,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Light),
            CreateBoss("fallen-archangel", "The Fallen Archangel", MonsterType.Fiend, 75,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateBoss("elemental-archons", "The Elemental Archons", MonsterType.Elemental, 80,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("undying-maester", "The Undying Maester", MonsterType.Undead, 85,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("demilich-lord", "The Demi-Lich Lord", MonsterType.Undead, 90,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("void-tree", "The Void Tree", MonsterType.Plant, 95,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Heavy),
            CreateBoss("time-compressor", "The Time Compressor", MonsterType.Humanoid, 100,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            
            // High Bosses (105-200)
            CreateBoss("calamity-swordsman", "The Calamity Swordsman", MonsterType.Humanoid, 105,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Heavy),
            CreateBoss("prince-undeath", "Prince of Undeath", MonsterType.Fiend, 110,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Heavy),
            CreateBoss("sin-spawn-beast", "The Sin-Spawn Beast", MonsterType.Monstrosity, 115,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("spider-queen", "The Spider Queen", MonsterType.Fiend, 120,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Light),
            CreateBoss("trance-angel-death", "The Trance Angel of Death", MonsterType.Humanoid, 125,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("lord-nine", "The Lord of the Nine", MonsterType.Fiend, 130,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateBoss("worldeater-whale", "The World-Eater Whale", MonsterType.Monstrosity, 140,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("ruby-construct", "The Ruby Construct", MonsterType.Construct, 150,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("death-knight-rider", "The Death Knight Rider", MonsterType.Undead, 160,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("void-incarnate", "The Void Incarnate", MonsterType.Abstract, 170,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("onewinged-seraph", "The One-Winged Seraph", MonsterType.Celestial, 180,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateBoss("false-saint", "The False Saint", MonsterType.Undead, 190,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("lord-underworld", "Lord of the Underworld", MonsterType.Fiend, 200,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            
            // Epic Bosses (210-300)
            CreateBoss("chest-guardian-dragon", "The Chest-Guardian Dragon", MonsterType.Dragon, 210,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateBoss("titan-time", "The Titan of Time", MonsterType.Celestial, 220,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateBoss("virtue-sentinel", "The Virtue Sentinel", MonsterType.Construct, 230,
                ZoneType.BossLair, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateBoss("cloud-entropy", "The Cloud of Entropy", MonsterType.Elemental, 240,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("timeloop-demon", "The Time-Loop Demon", MonsterType.Fiend, 250,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("pure-hatred", "Pure Hatred Incarnate", MonsterType.Abstract, 255,
                ZoneType.BossLair, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateBoss("nullifier", "The Nullifier", MonsterType.Abstract, 260,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("goddess-magic", "The Goddess of Magic", MonsterType.Celestial, 270,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("light-bringer", "The Light Bringer", MonsterType.God, 280,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
            CreateBoss("moon-creator", "The Moon Creator", MonsterType.God, 285,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("corrupted-magic-mother", "Corrupted Magic Mother", MonsterType.God, 290,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("five-judges", "The Five Judges", MonsterType.Humanoid, 295,
                ZoneType.BossLair, AttackType.FastMelee, ArmorType.Heavy),
            CreateBoss("chaotic-sphere", "The Chaotic Sphere", MonsterType.Abstract, 298,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Cloth),
            CreateBoss("invincible-pot", "The Invincible Pot", MonsterType.Construct, 299,
                ZoneType.BossLair, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateBoss("harbinger-discord", "The Harbinger of Discord", MonsterType.God, 300,
                ZoneType.BossLair, AttackType.MeleeHybrid, ArmorType.Heavy),
        };
    }
    
    #endregion

    #region Raid Boss Monsters (10 templates)
    
    private static IEnumerable<MonsterTemplate> GetRaidBossMonsters()
    {
        return new[]
        {
            CreateRaidBoss("void-touched-titan", "The Void-Touched Titan", MonsterType.Monstrosity, 30,
                ZoneType.RaidNexus, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateRaidBoss("clockwork-fortress", "The Clockwork Fortress", MonsterType.Construct, 60,
                ZoneType.RaidNexus, AttackType.RangedHybrid, ArmorType.Heavy),
            CreateRaidBoss("fiveheaded-matriarch", "The Five-Headed Matriarch", MonsterType.Dragon, 90,
                ZoneType.RaidNexus, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateRaidBoss("jade-colossus", "The Jade Colossus", MonsterType.Construct, 120,
                ZoneType.RaidNexus, AttackType.HeavyMelee, ArmorType.Heavy),
            CreateRaidBoss("ascended-archlich", "The Ascended Arch-Lich", MonsterType.Undead, 150,
                ZoneType.RaidNexus, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateRaidBoss("cyclone-wyrm", "The Cyclone Wyrm", MonsterType.Dragon, 180,
                ZoneType.RaidNexus, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateRaidBoss("starorbit-dragon", "The Star-Orbit Dragon", MonsterType.Dragon, 210,
                ZoneType.RaidNexus, AttackType.ElementalMagic, ArmorType.Heavy),
            CreateRaidBoss("endtimes-machine", "The End-Times Machine", MonsterType.Construct, 240,
                ZoneType.RaidNexus, AttackType.RangedHybrid, ArmorType.Heavy),
            CreateRaidBoss("chained-oblivion", "The Chained Oblivion", MonsterType.Fiend, 270,
                ZoneType.RaidNexus, AttackType.SpiritualMagic, ArmorType.Cloth),
            CreateRaidBoss("absolution-arbiter", "The Absolution Arbiter", MonsterType.Celestial, 300,
                ZoneType.RaidNexus, AttackType.MeleeHybrid, ArmorType.Heavy),
        };
    }
    
    #endregion

    #region Helper Methods
    
    private static MonsterTemplate CreateNormal(
        string id, string name, MonsterType type, int level,
        ZoneType zone, AttackType attackType, ArmorType armorType)
    {
        return new MonsterTemplate
        {
            TemplateId = id,
            Name = name,
            Type = type,
            Difficulty = MonsterDifficulty.Normal,
            Level = level,
            ValidZones = new[] { zone },
            AttackType = attackType,
            ArmorType = armorType,
            HpMultiplier = 1.0,
            DamageMultiplier = 1.0,
            ExpMultiplier = 1.0,
            GoldMultiplier = 1.0
        };
    }
    
    private static MonsterTemplate CreateBoss(
        string id, string name, MonsterType type, int level,
        ZoneType zone, AttackType attackType, ArmorType armorType)
    {
        return new MonsterTemplate
        {
            TemplateId = id,
            Name = name,
            Type = type,
            Difficulty = MonsterDifficulty.Boss,
            Level = level,
            ValidZones = new[] { zone },
            AttackType = attackType,
            ArmorType = armorType,
            HpMultiplier = 3.0,
            DamageMultiplier = 1.5,
            ExpMultiplier = 5.0,
            GoldMultiplier = 3.0
        };
    }
    
    private static MonsterTemplate CreateRaidBoss(
        string id, string name, MonsterType type, int level,
        ZoneType zone, AttackType attackType, ArmorType armorType)
    {
        return new MonsterTemplate
        {
            TemplateId = id,
            Name = name,
            Type = type,
            Difficulty = MonsterDifficulty.RaidBoss,
            Level = level,
            ValidZones = new[] { zone },
            AttackType = attackType,
            ArmorType = armorType,
            HpMultiplier = 10.0,
            DamageMultiplier = 2.0,
            ExpMultiplier = 20.0,
            GoldMultiplier = 10.0
        };
    }
    
    #endregion
}
