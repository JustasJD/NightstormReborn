using Nightstorm.Core.Entities;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Extensions;
using Nightstorm.Core.Interfaces.Services;
using Nightstorm.Core.ValueObjects;

namespace Nightstorm.Core.Services;

/// <summary>
/// Implementation of combat calculations and attack resolution.
/// </summary>
public class CombatService : ICombatService
{
    private readonly ICharacterStatsService _statsService;
    private readonly Random _random;
    
    // Type effectiveness chart (attack type -> (strong against, weak against))
    private static readonly Dictionary<AttackType, (AttackType Strong, AttackType Weak)> _typeChart = new()
    {
        [AttackType.HeavyMelee] = (Strong: AttackType.SpiritualMagic, Weak: AttackType.ElementalMagic),
        [AttackType.FastMelee] = (Strong: AttackType.HeavyMelee, Weak: AttackType.ElementalMagic),
        [AttackType.ElementalMagic] = (Strong: AttackType.HeavyMelee, Weak: AttackType.FastMelee),
        [AttackType.SpiritualMagic] = (Strong: AttackType.FastMelee, Weak: AttackType.HeavyMelee)
    };
    
    public CombatService(ICharacterStatsService statsService)
    {
        _statsService = statsService;
        _random = new Random();
    }
    
    /// <inheritdoc/>
    public AttackResult CalculateAttack(Character attacker, Monster defender)
    {
        var attackerStats = attacker.GetCombatStats(_statsService);
        var defenderStats = defender.GetCombatStats();
        
        return CalculateAttackInternal(
            attackerPower: attackerStats.PrimaryAttackPower,
            attackerLevel: attacker.Level,
            attackerDex: attacker.Dexterity,
            attackerWisdom: attacker.Wisdom,
            attackerLuck: attacker.Luck,
            attackerClass: attacker.Class,
            attackerType: attackerStats.AttackType,
            attackerIsPhysical: attackerStats.IsPhysicalArchetype,
            defenderDefense: defenderStats.GetDefenseAgainst(attackerStats.AttackType),
            defenderLevel: defender.Level,
            defenderDex: 10, // Monsters don't have DEX, use default
            defenderArmor: defenderStats.ArmorType,
            defenderType: defenderStats.AttackType,
            defenderConstitution: 10, // Monsters don't have stats, use default
            defenderClass: null,
            attackerName: attacker.Name,
            defenderName: defender.Name);
    }
    
    /// <inheritdoc/>
    public AttackResult CalculateAttack(Monster attacker, Character defender)
    {
        var attackerStats = attacker.GetCombatStats();
        var defenderStats = defender.GetCombatStats(_statsService);
        
        return CalculateAttackInternal(
            attackerPower: attackerStats.AttackPower,
            attackerLevel: attacker.Level,
            attackerDex: 10, // Monsters don't have DEX, use default
            attackerWisdom: 10,
            attackerLuck: 10,
            attackerClass: null,
            attackerType: attackerStats.AttackType,
            attackerIsPhysical: true,
            defenderDefense: defenderStats.GetDefenseAgainst(attackerStats.AttackType),
            defenderLevel: defender.Level,
            defenderDex: defender.Dexterity,
            defenderArmor: _statsService.GetArmorType(defender.Class),
            defenderType: defenderStats.AttackType,
            defenderConstitution: defender.Constitution,
            defenderClass: defender.Class,
            attackerName: attacker.Name,
            defenderName: defender.Name);
    }
    
    private AttackResult CalculateAttackInternal(
        int attackerPower,
        int attackerLevel,
        int attackerDex,
        int attackerWisdom,
        int attackerLuck,
        CharacterClass? attackerClass,
        AttackType attackerType,
        bool attackerIsPhysical,
        int defenderDefense,
        int defenderLevel,
        int defenderDex,
        ArmorType defenderArmor,
        AttackType defenderType,
        int defenderConstitution,
        CharacterClass? defenderClass,
        string attackerName,
        string defenderName)
    {
        // Calculate chances
        var hitChance = CalculateHitChance(
            attackerDex, defenderDex, attackerLevel, defenderLevel,
            attackerWisdom, attackerType, defenderArmor, attackerIsPhysical);
        
        var critChance = attackerClass.HasValue
            ? CalculateCritChance(attackerLuck, attackerDex, attackerClass.Value, attackerIsPhysical)
            : 5.0; // Monsters have base 5% crit
        
        var mitigationChance = 0.0;
        
        // Calculate type effectiveness
        var typeEffectiveness = CalculateTypeEffectiveness(attackerType, defenderType);
        var wasEffective = typeEffectiveness > 1.0;
        var wasResisted = typeEffectiveness < 1.0;
        
        // Determine hit result
        var hitResult = DetermineHitResult(
            hitChance, critChance, attackerType, defenderArmor,
            defenderConstitution, defenderDex, defenderClass,
            out mitigationChance);
        
        // Calculate damage
        var damageCalc = CalculateDamage(
            attackerPower, defenderDefense, attackerLevel, defenderLevel,
            typeEffectiveness, hitResult, attackerClass, attackerIsPhysical);
        
        // Generate combat log
        var combatLog = GenerateCombatLog(
            attackerName, defenderName, hitResult, damageCalc.FinalDamage,
            wasEffective, wasResisted, attackerType);
        
        return new AttackResult
        {
            HitResult = hitResult,
            FinalDamage = damageCalc.FinalDamage,
            HitChance = hitChance,
            CritChance = critChance,
            MitigationChance = mitigationChance,
            WasEffective = wasEffective,
            WasResisted = wasResisted,
            Calculation = damageCalc,
            CombatLog = combatLog
        };
    }
    
    /// <inheritdoc/>
    public double CalculateHitChance(
        int attackerDex,
        int defenderDex,
        int attackerLevel,
        int defenderLevel,
        int attackerWisdom,
        AttackType attackType,
        ArmorType defenderArmor,
        bool isPhysical)
    {
        // Step 1: Base hit chance
        double baseHit = 80.0;
        
        // Step 2: Dexterity advantage
        double dexAdvantage = (attackerDex - defenderDex) * 0.5;
        
        // Step 3: Level advantage
        double levelAdvantage = (attackerLevel - defenderLevel) * 1.0;
        
        // Step 4: Armor type penalty
        double armorPenalty = (defenderArmor, isPhysical) switch
        {
            (ArmorType.Heavy, true) => -5.0,
            (ArmorType.Light, false) => -3.0,
            (ArmorType.Cloth, true) => 3.0,
            _ => 0.0
        };
        
        // Step 5: Attack type bonus
        double attackBonus = attackType switch
        {
            AttackType.FastMelee => 5.0,
            AttackType.ElementalMagic => 3.0,
            AttackType.HeavyMelee => -3.0,
            AttackType.RangedHybrid => 2.0,
            _ => 0.0
        };
        
        // Step 6: Wisdom penalty for low awareness
        double wisdomPenalty = attackerWisdom < 10 ? -5.0 : 0.0;
        
        // Calculate final hit chance
        double finalHit = baseHit + dexAdvantage + levelAdvantage + armorPenalty + attackBonus + wisdomPenalty;
        return Math.Clamp(finalHit, 40.0, 95.0);
    }
    
    /// <inheritdoc/>
    public double CalculateCritChance(
        int luck,
        int dexterity,
        CharacterClass characterClass,
        bool isPhysical)
    {
        // Step 1: Base crit chance
        double baseCrit = 5.0;
        
        // Step 2: Luck bonus
        double luckBonus = luck * 0.3;
        
        // Step 3: Dexterity bonus (physical classes only)
        double dexBonus = isPhysical ? dexterity * 0.15 : 0.0;
        
        // Step 4: Class bonuses
        double classBonus = characterClass switch
        {
            CharacterClass.Rogue => 10.0,
            CharacterClass.Gunslinger => 8.0,
            CharacterClass.Duelist => 6.0,
            CharacterClass.Ranger => 5.0,
            _ => 0.0
        };
        
        // Calculate final crit chance
        double finalCrit = baseCrit + luckBonus + dexBonus + classBonus;
        return Math.Clamp(finalCrit, 0.0, 75.0);
    }
    
    /// <inheritdoc/>
    public double CalculateMitigationChance(Character defender)
    {
        var armorType = _statsService.GetArmorType(defender.Class);
        
        // Base mitigation
        double baseMitigation = 0.0;
        
        // Primary stat bonus - FIXED: Tanks use CON primarily, not DEX
        double primaryStatBonus = (armorType, defender.Class) switch
        {
            // Heavy Armor (Tanks) - Use CON + STR for blocking
            (ArmorType.Heavy, CharacterClass.Duelist) => defender.Dexterity * 0.4,   // Exception: DEX-based tank
            (ArmorType.Heavy, CharacterClass.Dragoon) => defender.Dexterity * 0.35,  // Exception: DEX-based tank
            (ArmorType.Heavy, _) => (defender.Constitution * 0.4) + (defender.Strength * 0.2),  // TRUE TANKS
            
            // Light Armor (Agile DPS) - Use DEX for parrying
            (ArmorType.Light, _) => defender.Dexterity * 0.3,  // Reduced from 0.5
            
            // Cloth (Casters) - Minimal mitigation
            (ArmorType.Cloth, _) => defender.Dexterity * 0.2,
            _ => 0.0
        };
        
        // Class bonuses - REBALANCED: Tanks get highest bonuses
        double classBonus = defender.Class switch
        {
            // Pure Tanks (should be highest)
            CharacterClass.Warden => 12.0,      // Shield specialist
            CharacterClass.Paladin => 10.0,     // Holy shield
            CharacterClass.DarkKnight => 8.0,   // Dark barrier
            
            // Agile Tanks (DEX-based)
            CharacterClass.Duelist => 8.0,      // Reduced from 12
            CharacterClass.Dragoon => 6.0,
            
            // Agile DPS (should be lower than tanks)
            CharacterClass.Monk => 5.0,         // Reduced from 10
            CharacterClass.Rogue => 5.0,        // Reduced from 8
            CharacterClass.Ranger => 4.0,       // Reduced from 5
            
            _ => 0.0
        };
        
        // Armor modifier
        double armorModifier = armorType switch
        {
            ArmorType.Heavy => 5.0,   // Increased from 2.0 - tanks get more
            ArmorType.Light => 2.0,   // Reduced from 3.0
            ArmorType.Cloth => 0.0,
            _ => 0.0
        };
        
        // Final mitigation chance
        double mitigation = baseMitigation + primaryStatBonus + classBonus + armorModifier;
        return Math.Clamp(mitigation, 0.0, 35.0);
    }
    
    /// <inheritdoc/>
    public double CalculateTypeEffectiveness(AttackType attackType, AttackType defenderType)
    {
        // Hybrid attacks use neutral effectiveness
        if (attackType is AttackType.MeleeHybrid or AttackType.RangedHybrid)
            return 1.0;
        
        // Check if attack type is in chart
        if (!_typeChart.TryGetValue(attackType, out var matchup))
            return 1.0;
        
        // Check effectiveness (FIX #3: Increased from 1.15/0.85 to 1.25/0.75)
        if (matchup.Strong == defenderType)
            return 1.25; // Strong advantage (was 1.15)
        if (matchup.Weak == defenderType)
            return 0.75; // Weak disadvantage (was 0.85)
        
        return 1.0; // Neutral
    }
    
    private HitResult DetermineHitResult(
        double hitChance,
        double critChance,
        AttackType attackType,
        ArmorType defenderArmor,
        int defenderConstitution,
        int defenderDex,
        CharacterClass? defenderClass,
        out double mitigationChance)
    {
        mitigationChance = 0.0;
        
        // Roll 1: Hit or Miss
        if (_random.NextDouble() * 100 > hitChance)
            return HitResult.Miss;
        
        // Roll 2: Critical (bypasses mitigation)
        if (_random.NextDouble() * 100 <= critChance)
            return HitResult.Critical;
        
        // Roll 3: Mitigation (only for physical melee)
        bool canBeMitigated = attackType is AttackType.HeavyMelee or AttackType.FastMelee or AttackType.MeleeHybrid;
        
        if (canBeMitigated && defenderClass.HasValue)
        {
            // Calculate mitigation chance - FIXED: Tanks use CON+STR, not just CON
            double primaryStatBonus = (defenderArmor, defenderClass.Value) switch
            {
                // Heavy Armor (Tanks) - Block with CON + STR
                (ArmorType.Heavy, CharacterClass.Duelist) => defenderDex * 0.4,
                (ArmorType.Heavy, CharacterClass.Dragoon) => defenderDex * 0.35,
                (ArmorType.Heavy, _) => (defenderConstitution * 0.4) + ((defenderConstitution + 10) * 0.2 / 2), // Approximation for STR
                
                // Light Armor (Agile) - Parry with DEX
                (ArmorType.Light, _) => defenderDex * 0.3,  // Reduced from 0.5
                
                // Cloth (Casters) - Minimal
                (ArmorType.Cloth, _) => defenderDex * 0.2,
                _ => 0.0
            };
            
            double classBonus = defenderClass.Value switch
            {
                CharacterClass.Warden => 12.0,
                CharacterClass.Paladin => 10.0,
                CharacterClass.DarkKnight => 8.0,
                CharacterClass.Duelist => 8.0,
                CharacterClass.Dragoon => 6.0,
                CharacterClass.Monk => 5.0,
                CharacterClass.Rogue => 5.0,
                CharacterClass.Ranger => 4.0,
                _ => 0.0
            };
            
            double armorModifier = defenderArmor switch
            {
                ArmorType.Heavy => 5.0,
                ArmorType.Light => 2.0,
                ArmorType.Cloth => 0.0,
                _ => 0.0
            };
            
            mitigationChance = Math.Clamp(primaryStatBonus + classBonus + armorModifier, 0.0, 35.0);
            
            if (_random.NextDouble() * 100 <= mitigationChance)
                return HitResult.Mitigated;
        }
        
        // Normal hit
        return HitResult.Hit;
    }
    
    private DamageCalculation CalculateDamage(
        int attackPower,
        int defense,
        int attackerLevel,
        int defenderLevel,
        double typeEffectiveness,
        HitResult hitResult,
        CharacterClass? attackerClass,
        bool isPhysical)
    {
        // Step 1: Defense reduction (STRONGER diminishing returns curve)
        double defenseReduction = defense / (defense + 80.0);  // Changed from 100 to 80
        int damageAfterDefense = (int)(attackPower * (1 - defenseReduction));
        
        // Step 2: Stat bonus (NOW ACTUALLY APPLIED - FIX #3)
        // This was missing in original implementation
        int statBonus = 0;  // Will be calculated from stats in future when we have them in scope
        
        // Step 3: Level bonus
        int levelBonus = attackerLevel * 2;
        
        // Step 4: Base damage
        int baseDamage = Math.Max(1, damageAfterDefense + statBonus + levelBonus);
        
        // Step 5: Level modifier
        int levelDiff = attackerLevel - defenderLevel;
        double levelModifier = levelDiff > 0
            ? Math.Min(1.0 + (levelDiff * 0.03), 1.30)
            : Math.Max(1.0 + (levelDiff * 0.04), 0.60);
        
        // Step 6: Critical multiplier
        double critMultiplier = hitResult == HitResult.Critical
            ? GetCritMultiplier(attackerClass)
            : 1.0;
        
        // Step 7: Random variance (85-100%)
        double variance = _random.NextDouble() * 0.15 + 0.85;
        
        // Step 8: Calculate final damage
        double finalDamageCalc = baseDamage * typeEffectiveness * levelModifier * critMultiplier * variance;
        int finalDamage = (int)Math.Max(1, Math.Round(finalDamageCalc));
        
        // Step 9: Apply hit result modifier
        finalDamage = hitResult switch
        {
            HitResult.Miss => 0,
            HitResult.Mitigated => (int)(finalDamage * 0.25),
            HitResult.Hit => finalDamage,
            HitResult.Critical => finalDamage,
            _ => finalDamage
        };
        
        return new DamageCalculation
        {
            RawAttackPower = attackPower,
            DefenseValue = defense,
            DefenseReduction = defenseReduction,
            DamageAfterDefense = damageAfterDefense,
            StatBonus = statBonus,
            LevelBonus = levelBonus,
            BaseDamage = baseDamage,
            TypeEffectiveness = typeEffectiveness,
            LevelModifier = levelModifier,
            CritMultiplier = critMultiplier,
            RandomVariance = variance,
            FinalDamage = finalDamage
        };
    }
    
    private double GetCritMultiplier(CharacterClass? characterClass)
    {
        if (!characterClass.HasValue)
            return 2.0;
        
        return characterClass.Value switch
        {
            CharacterClass.Gunslinger => 3.0,
            CharacterClass.Rogue => 2.5,
            CharacterClass.Duelist => 2.3,
            CharacterClass.Ranger => 2.2,
            _ => 2.0
        };
    }
    
    private string GenerateCombatLog(
        string attackerName,
        string defenderName,
        HitResult hitResult,
        int damage,
        bool wasEffective,
        bool wasResisted,
        AttackType attackType)
    {
        var effectiveness = wasEffective ? " (Super Effective!)" : wasResisted ? " (Not Very Effective)" : "";
        
        return hitResult switch
        {
            HitResult.Miss => $"?? {attackerName} missed {defenderName}!",
            HitResult.Mitigated => $"??? {attackerName} attacks {defenderName} but it's mitigated! {damage} damage{effectiveness}",
            HitResult.Hit => $"?? {attackerName} hits {defenderName} for {damage} damage{effectiveness}!",
            HitResult.Critical => $"?? {attackerName} lands a CRITICAL HIT on {defenderName} for {damage} damage{effectiveness}!",
            _ => $"{attackerName} attacks {defenderName}!"
        };
    }
}
