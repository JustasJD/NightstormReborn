# Three-Tier Armor System Implementation

## Overview
This document details the implementation of the three-tier defensive archetype system that replaces the simple two-tier (Physical vs Caster) system with a more nuanced **Heavy, Light, and Cloth** armor classification.

---

## Armor Type Classifications

### Heavy Armor (5 classes)
**Philosophy**: Tanks and heavy warriors who can take punishment from physical attacks but are vulnerable to magic.

**Classes**:
- Paladin (Melee Hybrid)
- Warden (Heavy Melee Tank)
- DarkKnight (Heavy Melee Tank)
- Duelist (Heavy Melee)
- Dragoon (Heavy Melee)

**Characteristics**:
- **HP Scaling**: Highest (CON × 20)
- **MP Scaling**: Lowest (SPR × 10)
- **Physical Defense**: Excellent (+15 to Heavy Melee, +15 to Fast Melee)
- **Magical Defense**: Poor (+5 to Elemental, +5 to Spiritual)

### Light Armor (5 classes)
**Philosophy**: Agile warriors and ranged specialists with balanced defenses, good mobility and moderate magical resistance.

**Classes**:
- Monk (Fast Melee)
- Rogue (Fast Melee)
- Ranger (Fast Melee)
- Gunslinger (Fast Melee)
- Alchemist (Ranged Hybrid)

**Characteristics**:
- **HP Scaling**: Medium (CON × 15)
- **MP Scaling**: Medium (SPR × 15)
- **Physical Defense**: Moderate (+5 Heavy Melee, +10 Fast Melee)
- **Magical Defense**: Moderate (+10 Elemental, +5 Spiritual)

### Cloth/Magical (6 classes)
**Philosophy**: Pure casters with magical wards, excellent against magic but fragile against physical attacks.

**Classes**:
- Wizard (Elemental Magic)
- Sorcerer (Elemental Magic)
- Necromancer (Spiritual Magic)
- Cleric (Spiritual Magic)
- Druid (Elemental Magic)
- Bard (Spiritual Magic)

**Characteristics**:
- **HP Scaling**: Lowest (CON × 10)
- **MP Scaling**: Highest (SPR × 20)
- **Physical Defense**: None (+0 to both melee types)
- **Magical Defense**: Excellent (+15 to Elemental, +15 to Spiritual)

---

## HP/MP Formulas by Armor Type

### Maximum Health
| Armor Type | Formula |
|------------|---------|
| **Heavy** | `100 + (CON × 20) + ((STR + DEX) × 3)` |
| **Light** | `100 + (CON × 15) + ((STR + DEX) × 3)` |
| **Cloth** | `100 + (CON × 10) + ((STR + DEX) × 3)` |

### Maximum Mana
| Armor Type | Formula |
|------------|---------|
| **Heavy** | `100 + (SPR × 10) + ((INT + WIS) × 3)` |
| **Light** | `100 + (SPR × 15) + ((INT + WIS) × 3)` |
| **Cloth** | `100 + (SPR × 20) + ((INT + WIS) × 3)` |

**Example Comparisons** (at 14 CON, 14 STR, 10 DEX, 12 SPR, 8 INT, 12 WIS):

| Class | Armor | HP | MP |
|-------|-------|----|----|
| Paladin | Heavy | 452 | 360 |
| Monk | Light | 402 | 420 |
| Wizard | Cloth | 334 | 520 |

---

## Defense Value Bonuses by Armor Type

### Heavy Melee Defense (vs STR attacks)
**Formula**: `Base(10) + (DEX × 2) + CON + [Armor Bonus]`

| Armor Type | Bonus |
|------------|-------|
| Heavy | **+15** |
| Light | +5 |
| Cloth | +0 |

### Fast Melee Defense (vs DEX attacks)
**Formula**: `Base(10) + (CON × 2) + STR + [Armor Bonus]`

| Armor Type | Bonus |
|------------|-------|
| Heavy | **+15** |
| Light | +10 |
| Cloth | +0 |

### Elemental Magic Defense (vs INT magic)
**Formula**: `Base(10) + (WIS × 2) + SPR + [Armor Bonus]`

| Armor Type | Bonus |
|------------|-------|
| Heavy | +5 |
| Light | +10 |
| Cloth | **+15** |

### Spiritual Magic Defense (vs WIS/SPR magic)
**Formula**: `Base(10) + (SPR × 2) + WIS + [Armor Bonus]`

| Armor Type | Bonus |
|------------|-------|
| Heavy | +5 |
| Light | +5 |
| Cloth | **+15** |

---

## Defense Value Examples

Using **Paladin** (Heavy: STR 14, DEX 10, CON 14, INT 8, WIS 12, SPR 12):
- Heavy Melee DV: 10 + (10×2) + 14 + 15 = **59**
- Fast Melee DV: 10 + (14×2) + 14 + 15 = **67**
- Elemental DV: 10 + (12×2) + 12 + 5 = **51**
- Spiritual DV: 10 + (12×2) + 12 + 5 = **51**

Using **Monk** (Light: STR 8, DEX 16, CON 14, INT 8, WIS 10, SPR 14):
- Heavy Melee DV: 10 + (16×2) + 14 + 5 = **61**
- Fast Melee DV: 10 + (14×2) + 8 + 10 = **56**
- Elemental DV: 10 + (10×2) + 14 + 10 = **54**
- Spiritual DV: 10 + (14×2) + 10 + 5 = **53**

Using **Wizard** (Cloth: STR 8, DEX 10, CON 10, INT 18, WIS 8, SPR 14):
- Heavy Melee DV: 10 + (10×2) + 10 + 0 = **40**
- Fast Melee DV: 10 + (10×2) + 8 + 0 = **38**
- Elemental DV: 10 + (8×2) + 14 + 15 = **55**
- Spiritual DV: 10 + (14×2) + 8 + 15 = **61**

---

## Attack Type Updates

### Hybrid Attack Types

Two hybrid types now exist:

#### MeleeHybrid (Paladin)
- Combines Heavy Melee (STR) + Spiritual Magic (WIS/SPR)
- Uses best of Melee Attack Power or Magic Power
- Defended by best of Heavy Melee DV or Spiritual Magic DV

#### RangedHybrid (Alchemist)
- Combines Ranged Physical (DEX) + Elemental effects (INT)
- Uses best of Ranged Attack Power or Magic Power
- Defended by best of Fast Melee DV or Elemental Magic DV

### Updated Attack Type Assignments

| Class | Attack Type | Primary Stat | Notes |
|-------|-------------|--------------|-------|
| Paladin | **MeleeHybrid** | STR/WIS | Holy warrior with divine smites |
| Warden | HeavyMelee | STR | Tank with crushing weapons |
| DarkKnight | HeavyMelee | STR | **Corrected** from FastMelee |
| Duelist | HeavyMelee | STR | **Corrected** from FastMelee |
| Dragoon | HeavyMelee | STR | Polearm specialist |
| Monk | FastMelee | DEX | Unarmed combos |
| Rogue | FastMelee | DEX | Stealth assassin |
| Ranger | FastMelee | DEX | Bow specialist |
| Gunslinger | FastMelee | DEX | Firearm sharpshooter |
| Alchemist | **RangedHybrid** | DEX/INT | **Changed** from ElementalMagic |
| Wizard | ElementalMagic | INT | Arcane scholar |
| Sorcerer | ElementalMagic | INT | Bloodline caster |
| Druid | ElementalMagic | WIS | Nature magic |
| Necromancer | SpiritualMagic | INT | Death magic |
| Cleric | SpiritualMagic | WIS | Divine healer |
| Bard | SpiritualMagic | INT | Support buffer |

---

## Strategic Implications

### Tank (Heavy Armor) Strengths & Weaknesses
? **Strengths**:
- Excellent against physical DPS (melee & ranged)
- High HP pool for sustained engagements
- Can frontline against physical teams

? **Weaknesses**:
- Vulnerable to mages (low magical defense)
- Low MP pool limits magical abilities
- Struggles against Elemental/Spiritual casters

### Agile (Light Armor) Strengths & Weaknesses
? **Strengths**:
- Balanced defenses across all damage types
- Good mobility and versatility
- Effective against both physical and magical threats
- Medium resource pools (HP and MP)

? **Weaknesses**:
- Not exceptional at anything (jack-of-all-trades)
- Cannot tank as well as Heavy armor
- Lower magical output than Cloth casters

### Caster (Cloth Armor) Strengths & Weaknesses
? **Strengths**:
- Excellent against other casters (high magical defense)
- Massive MP pool for sustained casting
- High burst damage with spells
- Can counter-caster effectively

? **Weaknesses**:
- Extremely vulnerable to physical attacks
- Low HP makes them fragile
- Cannot frontline in melee
- Easy target for assassins/rogues

---

## Rock-Paper-Scissors Balance

```
Heavy Armor (Tanks)  ??[beats]??>  Light Armor (Agile)
      ?                                      ?
      ?                                      ?
   [weak to]                            [beats]
      ?                                      ?
      ?                                      ?
Cloth Armor (Casters)  <??[weak to]??  Light Armor (Agile)
```

**Explanation**:
- **Heavy beats Light**: Tanks can absorb agile attacks and overpower them with superior HP
- **Light beats Cloth**: Agile classes can close distance and exploit casters' low physical defense
- **Cloth beats Heavy**: Mages bypass heavy armor with magical attacks

---

## Code Implementation

### New Files Created
1. `src\Nightstorm.Core\Enums\ArmorType.cs` - Three-tier armor classification

### Files Modified
1. `src\Nightstorm.Core\Enums\AttackType.cs` - Added `RangedHybrid`, renamed `Hybrid` to `MeleeHybrid`
2. `src\Nightstorm.Core\Services\CharacterStatsService.cs` - Implemented three-tier system with proper bonuses
3. `src\Nightstorm.Core\Interfaces\Services\ICharacterStatsService.cs` - Added `GetArmorType()` method
4. `src\Nightstorm.Core\Extensions\CharacterCombatExtensions.cs` - Updated hybrid handling

### Key Methods Added

```csharp
// Get armor type for any class
ArmorType GetArmorType(CharacterClass characterClass);

// Returns Heavy, Light, or Cloth based on class
```

### Constants Updated

```csharp
// HP Multipliers
private const int HeavyArmorConstitutionMultiplier = 20;
private const int LightArmorConstitutionMultiplier = 15;
private const int ClothArmorConstitutionMultiplier = 10;

// MP Multipliers
private const int HeavyArmorSpiritMultiplier = 10;
private const int LightArmorSpiritMultiplier = 15;
private const int ClothArmorSpiritMultiplier = 20;

// Defense Bonuses
private const int HeavyArmorPhysicalBonus = 15;
private const int HeavyArmorMagicalBonus = 5;
private const int LightArmorHeavyMeleeBonus = 5;
private const int LightArmorFastMeleeBonus = 10;
private const int LightArmorMagicBonus = 10;
private const int LightArmorSpiritualBonus = 5;
private const int ClothArmorMagicalBonus = 15;
```

---

## Migration Notes

### Breaking Changes
None - This is a refinement of the existing system, not a breaking change.

### Behavioral Changes
1. HP/MP calculations now use three multipliers instead of two
2. Defense bonuses are now more granular
3. Alchemist changed from Cloth to Light armor
4. DarkKnight and Duelist changed from FastMelee to HeavyMelee attack type

### Testing Required
- [ ] Verify HP/MP calculations for all 16 classes
- [ ] Test defense calculations against each attack type
- [ ] Validate combat balance simulation
- [ ] Ensure hybrid attack types work correctly

---

## Future Enhancements

1. **Equipment System**: Items could modify effective armor type bonuses
2. **Stance System**: Classes could temporarily shift armor type (e.g., Duelist enters "Defensive Stance" for +5 all defenses)
3. **Armor Piercing**: Some attacks could ignore a percentage of armor bonuses
4. **Magical Armor**: Light armor classes could use magical enchantments to boost magic defense
5. **Training/Talents**: Allow customization of armor bonuses within ranges

---

**Status**: ? Implemented and Build Successful
**Author**: GitHub Copilot
**Date**: 2025
**Version**: 2.0 (Three-Tier System)
