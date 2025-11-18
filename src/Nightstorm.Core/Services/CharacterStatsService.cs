using Nightstorm.Core.Enums;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Core.Services;

/// <summary>
/// Implementation of character stats calculation service.
/// Handles all combat stat derivations and HP/MP calculations with three-tier armor system.
/// </summary>
public class CharacterStatsService : ICharacterStatsService
{
    private const int BaseHp = 100;
    private const int BaseMp = 100;
    private const int BaseDefenseValue = 10;
    
    // HP/MP Multipliers by Armor Type
    private const int HeavyArmorConstitutionMultiplier = 20;
    private const int LightArmorConstitutionMultiplier = 15;
    private const int ClothArmorConstitutionMultiplier = 10;
    
    private const int HeavyArmorSpiritMultiplier = 10;
    private const int LightArmorSpiritMultiplier = 15;
    private const int ClothArmorSpiritMultiplier = 20;

    // Defense Bonuses by Armor Type
    private const int HeavyArmorPhysicalBonus = 15;
    private const int HeavyArmorMagicalBonus = 5;
    
    private const int LightArmorHeavyMeleeBonus = 5;
    private const int LightArmorFastMeleeBonus = 10;
    private const int LightArmorMagicBonus = 10;
    private const int LightArmorSpiritualBonus = 5;
    
    private const int ClothArmorMagicalBonus = 15;

    /// <inheritdoc/>
    public int CalculateMeleeAttackPower(int strength, int dexterity)
    {
        return (strength * 4) + (dexterity * 2) + 10;
    }

    /// <inheritdoc/>
    public int CalculateRangedAttackPower(int dexterity, int strength)
    {
        return (dexterity * 4) + (strength * 2) + 10;
    }

    /// <inheritdoc/>
    public int CalculateMagicPower(CharacterClass characterClass, int intelligence, int wisdom, int spirit)
    {
        var primaryCasterStat = GetPrimaryCasterStat(characterClass, intelligence, wisdom);
        return (primaryCasterStat * 4) + (spirit * 2) + 10;
    }

    /// <inheritdoc/>
    public int CalculateMaxHealth(CharacterClass characterClass, int constitution, int strength, int dexterity)
    {
        var armorType = GetArmorType(characterClass);
        var constitutionMultiplier = armorType switch
        {
            ArmorType.Heavy => HeavyArmorConstitutionMultiplier,
            ArmorType.Light => LightArmorConstitutionMultiplier,
            ArmorType.Cloth => ClothArmorConstitutionMultiplier,
            _ => ClothArmorConstitutionMultiplier
        };
        
        return BaseHp + (constitution * constitutionMultiplier) + ((strength + dexterity) * 3);
    }

    /// <inheritdoc/>
    public int CalculateMaxMana(CharacterClass characterClass, int spirit, int intelligence, int wisdom)
    {
        var armorType = GetArmorType(characterClass);
        var spiritMultiplier = armorType switch
        {
            ArmorType.Heavy => HeavyArmorSpiritMultiplier,
            ArmorType.Light => LightArmorSpiritMultiplier,
            ArmorType.Cloth => ClothArmorSpiritMultiplier,
            _ => ClothArmorSpiritMultiplier
        };
        
        return BaseMp + (spirit * spiritMultiplier) + ((intelligence + wisdom) * 3);
    }

    /// <inheritdoc/>
    public int CalculateHeavyMeleeDefense(CharacterClass characterClass, int dexterity, int constitution)
    {
        var armorBonus = GetArmorType(characterClass) switch
        {
            ArmorType.Heavy => HeavyArmorPhysicalBonus,
            ArmorType.Light => LightArmorHeavyMeleeBonus,
            ArmorType.Cloth => 0,
            _ => 0
        };
        
        return BaseDefenseValue + (dexterity * 2) + constitution + armorBonus;
    }

    /// <inheritdoc/>
    public int CalculateFastMeleeDefense(CharacterClass characterClass, int constitution, int strength)
    {
        var armorBonus = GetArmorType(characterClass) switch
        {
            ArmorType.Heavy => HeavyArmorPhysicalBonus,
            ArmorType.Light => LightArmorFastMeleeBonus,
            ArmorType.Cloth => 0,
            _ => 0
        };
        
        return BaseDefenseValue + (constitution * 2) + strength + armorBonus;
    }

    /// <inheritdoc/>
    public int CalculateElementalMagicDefense(CharacterClass characterClass, int wisdom, int spirit)
    {
        var armorBonus = GetArmorType(characterClass) switch
        {
            ArmorType.Heavy => HeavyArmorMagicalBonus,
            ArmorType.Light => LightArmorMagicBonus,
            ArmorType.Cloth => ClothArmorMagicalBonus,
            _ => 0
        };
        
        return BaseDefenseValue + (wisdom * 2) + spirit + armorBonus;
    }

    /// <inheritdoc/>
    public int CalculateSpiritualMagicDefense(CharacterClass characterClass, int spirit, int wisdom)
    {
        var armorBonus = GetArmorType(characterClass) switch
        {
            ArmorType.Heavy => HeavyArmorMagicalBonus,
            ArmorType.Light => LightArmorSpiritualBonus,
            ArmorType.Cloth => ClothArmorMagicalBonus,
            _ => 0
        };
        
        return BaseDefenseValue + (spirit * 2) + wisdom + armorBonus;
    }

    /// <inheritdoc/>
    public ArmorType GetArmorType(CharacterClass characterClass)
    {
        return characterClass switch
        {
            // Heavy Armor - Tanks and heavy melee warriors
            CharacterClass.Paladin => ArmorType.Heavy,
            CharacterClass.Warden => ArmorType.Heavy,
            CharacterClass.DarkKnight => ArmorType.Heavy,
            CharacterClass.Duelist => ArmorType.Heavy,
            CharacterClass.Dragoon => ArmorType.Heavy,
            
            // Light Armor - Agile melee and ranged physical
            CharacterClass.Monk => ArmorType.Light,
            CharacterClass.Rogue => ArmorType.Light,
            CharacterClass.Ranger => ArmorType.Light,
            CharacterClass.Gunslinger => ArmorType.Light,
            CharacterClass.Alchemist => ArmorType.Light,
            
            // Cloth - Pure casters
            CharacterClass.Wizard => ArmorType.Cloth,
            CharacterClass.Sorcerer => ArmorType.Cloth,
            CharacterClass.Necromancer => ArmorType.Cloth,
            CharacterClass.Cleric => ArmorType.Cloth,
            CharacterClass.Druid => ArmorType.Cloth,
            CharacterClass.Bard => ArmorType.Cloth,
            
            _ => ArmorType.Cloth // Default
        };
    }

    /// <inheritdoc/>
    public bool IsPhysicalArchetype(CharacterClass characterClass)
    {
        return GetArmorType(characterClass) != ArmorType.Cloth;
    }

    /// <inheritdoc/>
    public bool IsTankClass(CharacterClass characterClass)
    {
        return GetArmorType(characterClass) == ArmorType.Heavy;
    }

    /// <inheritdoc/>
    public AttackType GetAttackType(CharacterClass characterClass)
    {
        return characterClass switch
        {
            // Melee Hybrid (Heavy Melee + Spiritual Magic)
            CharacterClass.Paladin => AttackType.MeleeHybrid,
            
            // Heavy Melee
            CharacterClass.Warden => AttackType.HeavyMelee,
            CharacterClass.DarkKnight => AttackType.HeavyMelee,
            CharacterClass.Duelist => AttackType.HeavyMelee,
            CharacterClass.Dragoon => AttackType.HeavyMelee,
            
            // Fast Melee
            CharacterClass.Monk => AttackType.FastMelee,
            CharacterClass.Rogue => AttackType.FastMelee,
            CharacterClass.Ranger => AttackType.FastMelee,
            CharacterClass.Gunslinger => AttackType.FastMelee,
            
            // Ranged Hybrid (Ranged Physical + Elemental)
            CharacterClass.Alchemist => AttackType.RangedHybrid,
            
            // Elemental Magic
            CharacterClass.Wizard => AttackType.ElementalMagic,
            CharacterClass.Sorcerer => AttackType.ElementalMagic,
            CharacterClass.Druid => AttackType.ElementalMagic,
            
            // Spiritual Magic
            CharacterClass.Necromancer => AttackType.SpiritualMagic,
            CharacterClass.Cleric => AttackType.SpiritualMagic,
            CharacterClass.Bard => AttackType.SpiritualMagic,
            
            _ => AttackType.HeavyMelee // Default
        };
    }

    /// <summary>
    /// Gets the primary casting stat for a character class.
    /// </summary>
    private int GetPrimaryCasterStat(CharacterClass characterClass, int intelligence, int wisdom)
    {
        return characterClass switch
        {
            // Wisdom-based casters
            CharacterClass.Paladin => wisdom,
            CharacterClass.Warden => wisdom,
            CharacterClass.Cleric => wisdom,
            CharacterClass.Druid => wisdom,
            CharacterClass.Ranger => wisdom,
            
            // Intelligence-based casters
            CharacterClass.DarkKnight => intelligence,
            CharacterClass.Wizard => intelligence,
            CharacterClass.Sorcerer => intelligence,
            CharacterClass.Necromancer => intelligence,
            CharacterClass.Bard => intelligence,
            CharacterClass.Alchemist => intelligence,
            
            // Default to Intelligence
            _ => intelligence
        };
    }
}
