# Equipment System - Phase 2: Item Types & Categories ?

## ?? Step 2 Complete: Item Type Enums

### ? What Was Created

#### 1. **WeaponType Enum** ??
**File:** `src\Nightstorm.Core\Enums\WeaponType.cs`

**13 Weapon Types Defined:**

| Weapon Type | Hands | Category | Damage Type | Icon |
|-------------|-------|----------|-------------|------|
| **Sword** | 1H | Melee | Slashing | ?? |
| **Greatsword** | 2H | Melee | Slashing | ??? |
| **Axe** | 1H | Melee | Slashing | ?? |
| **Greataxe** | 2H | Melee | Slashing | ?? |
| **Mace** | 1H | Melee | Crushing | ?? |
| **Warhammer** | 2H | Melee | Crushing | ?? |
| **Dagger** | 1H | Melee | Piercing | ??? |
| **Spear** | 1H | Melee | Piercing | ?? |
| **Staff** | 2H | Magic | Magical | ?? |
| **Wand** | 1H | Magic | Magical | ?? |
| **Bow** | 2H | Ranged | Ranged Physical | ?? |
| **Crossbow** | 2H | Ranged | Ranged Physical | ?? |
| **Shield** | 1H (Off-hand only) | Defensive | Defensive | ??? |

---

#### 2. **ArmorMaterial Enum** ???
**File:** `src\Nightstorm.Core\Enums\ArmorMaterial.cs`

**5 Armor Materials:**

| Material | Defense | Weight | Best For | Special |
|----------|---------|--------|----------|---------|
| **Cloth** | Lowest | Very Light | Mages, Casters | Enchantable |
| **Leather** | Light | Light | Rogues, Rangers | Mobility |
| **Chain** | Medium | Medium | Hybrids | Balanced |
| **Plate** | Highest | Heavy | Warriors, Paladins | High Defense |
| **Dragonscale** | Very High | Medium | High-level chars | Magic Resist |

---

#### 3. **WeaponTypeExtensions** ??
**File:** `src\Nightstorm.Core\Enums\WeaponTypeExtensions.cs`

**Helper Methods:**

```csharp
// Weapon Classification
weapon.IsTwoHanded()           // Greatsword, Greataxe, Warhammer, Staff, Bow, Crossbow
weapon.IsOneHanded()           // Sword, Axe, Mace, Dagger, Spear, Wand
weapon.IsMelee()               // All close-combat weapons
weapon.IsRanged()              // Bow, Crossbow
weapon.IsMagical()             // Staff, Wand

// Weapon Properties
weapon.GetDamageType()         // "Slashing", "Crushing", "Piercing", etc.
weapon.GetIcon()               // ??, ???, ??, ??, etc.
weapon.GetRequiredSlots()      // [MainHand] or [MainHand, OffHand]
weapon.GetDisplayName()        // "Greatsword", "Warhammer", etc.
```

---

#### 4. **ItemRarityExtensions** ?
**File:** `src\Nightstorm.Core\Enums\ItemRarityExtensions.cs`

**Helper Methods:**

```csharp
// Visual Display
rarity.GetColorHex()               // "#FF8000" for Legendary
rarity.GetDiscordColor()           // 0xFF8000 for Discord embeds
rarity.GetDisplayName()            // "? Legendary"

// Game Mechanics
rarity.GetStatMultiplier()         // 2.0m for Legendary (+100%)
rarity.GetVendorPriceMultiplier()  // 25.0m for Legendary
rarity.GetDropRatePercentage()     // 0.19% for Legendary
```

**Rarity Breakdown:**

| Rarity | Color | Drop Rate | Stat Bonus | Vendor Price | Icon |
|--------|-------|-----------|------------|--------------|------|
| **Common** | Gray | 65% | +0% | 1x | ? |
| **Uncommon** | Green | 25% | +15% | 2.5x | ?? |
| **Rare** | Blue | 8% | +35% | 5x | ?? |
| **Epic** | Purple | 1.8% | +65% | 10x | ?? |
| **Legendary** | Orange | 0.19% | +100% | 25x | ? |
| **Mythic** | Red/Gold | 0.01% | +150% | 100x | ? |

---

## ?? Existing Enums (Already in Place)

### **ItemType** (Already existed)
```csharp
- Weapon
- Armor
- Accessory
- Consumable
- Quest (QuestItem)
- Material
- Miscellaneous
```

### **ItemRarity** (Already existed)
```csharp
- Common
- Uncommon
- Rare
- Epic
- Legendary
- Mythic
```

### **ArmorType** (Different purpose - class defensive archetype)
```csharp
- Heavy (Warriors, Paladins)
- Light (Rogues, Rangers)
- Cloth (Mages, Healers)
```
**Note:** This is for **character class** armor proficiency, NOT equipment material!

---

## ?? Usage Examples

### Example 1: Weapon Classification

```csharp
var weaponType = WeaponType.Greatsword;

if (weaponType.IsTwoHanded())
{
    Console.WriteLine("This weapon requires both hands!");
    Console.WriteLine($"Blocks off-hand slot: {weaponType.GetRequiredSlots().Length > 1}");
}

Console.WriteLine($"Damage Type: {weaponType.GetDamageType()}");
// Output: "Damage Type: Slashing"

Console.WriteLine($"Icon: {weaponType.GetIcon()}");
// Output: "Icon: ???"
```

### Example 2: Item Rarity Display

```csharp
var item = GetSwordFromLoot();
var rarity = ItemRarity.Legendary;

Console.WriteLine($"{rarity.GetDisplayName()} Sword");
// Output: "? Legendary Sword"

var stats = baseStats * rarity.GetStatMultiplier();
// Legendary = baseStats * 2.0 (double stats!)

var vendorPrice = basePrice * rarity.GetVendorPriceMultiplier();
// Legendary = basePrice * 25 (25x sell value)
```

### Example 3: Discord Bot Item Display

```csharp
var embed = new EmbedBuilder()
    .WithTitle($"{rarity.GetIcon()} {itemName}")
    .WithColor(rarity.GetDiscordColor())
    .WithDescription($"**Rarity:** {rarity.GetDisplayName()}\n" +
                     $"**Type:** {weaponType.GetIcon()} {weaponType.GetDisplayName()}\n" +
                     $"**Damage:** {weaponType.GetDamageType()}\n" +
                     $"**Hands:** {(weaponType.IsTwoHanded() ? "Two-Handed" : "One-Handed")}")
    .AddField("Stats", 
        $"+{damageBonus} Attack\n" +
        $"+{strengthBonus} Strength\n" +
        $"+{dexterityBonus} Dexterity");
```

**Output:**
```
? Sword of Legends
???????????????????
Rarity: ? Legendary
Type: ?? Sword
Damage: Slashing
Hands: One-Handed

Stats:
+150 Attack
+25 Strength
+15 Dexterity

Drop Rate: 0.19%
Vendor Price: 5,000g
```

---

## ?? Weapon Type Rules

### Two-Handed Weapons
```csharp
// These weapons block the off-hand slot:
- Greatsword  ???
- Greataxe    ??
- Warhammer   ??
- Staff       ??
- Bow         ??
- Crossbow    ??
```

**When equipped:**
- MainHand slot = Weapon
- OffHand slot = **BLOCKED** (cannot equip shield or second weapon)

### One-Handed Weapons
```csharp
// These allow off-hand usage:
- Sword    ??
- Axe      ??
- Mace     ??
- Dagger   ???
- Spear    ??
- Wand     ??
```

**When equipped:**
- MainHand slot = Weapon
- OffHand slot = **AVAILABLE** (can equip shield or second weapon)

### Dual-Wielding Rules
```csharp
// Valid combinations:
? Sword + Sword     (dual wield)
? Sword + Dagger    (dual wield)
? Sword + Shield    (sword and board)
? Wand + Shield     (caster with defense)
? Dagger + Dagger   (rogue style)

// Invalid combinations:
? Greatsword + Shield    (two-handed blocks off-hand)
? Bow + Sword            (bow requires both hands)
? Staff + Wand           (staff blocks off-hand)
```

---

## ?? Next Steps

Now that we have all item types and categories defined, we can proceed to:

### **Step 3: Update Item Entity** (NEXT)
Add equipment properties to the Item entity:
```csharp
public class Item : BaseEntity
{
    // Existing properties...
    
    // NEW Equipment Properties:
    public ItemType ItemType { get; set; }
    public EquipmentSlot? EquipmentSlot { get; set; }  // Which slot it goes in
    public WeaponType? WeaponType { get; set; }        // If weapon
    public ArmorMaterial? ArmorMaterial { get; set; }  // If armor
    public ItemRarity Rarity { get; set; }
    
    // Stat Bonuses
    public int BonusStrength { get; set; }
    public int BonusDexterity { get; set; }
    public int BonusConstitution { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusWisdom { get; set; }
    public int BonusSpirit { get; set; }
    public int BonusLuck { get; set; }
    
    // Weapon-specific
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public decimal AttackSpeed { get; set; }
    
    // Armor-specific
    public int ArmorValue { get; set; }
    
    // Requirements
    public int RequiredLevel { get; set; }
    public CharacterClass? RequiredClass { get; set; }
}
```

### **Step 4: Character Equipment Tracking**
Create CharacterEquipment entity to track what's equipped:
```csharp
public class CharacterEquipment : BaseEntity
{
    public Guid CharacterId { get; set; }
    public Character Character { get; set; }
    
    public EquipmentSlot Slot { get; set; }
    
    public Guid? ItemId { get; set; }
    public Item? Item { get; set; }
}
```

### **Step 5: Equip/Unequip Logic**
- Validation (can character use this item?)
- Two-handed weapon handling
- Stat recalculation
- API endpoints

---

## ? Phase 2 Checklist

- [x] Create WeaponType enum (13 types)
- [x] Create ArmorMaterial enum (5 materials)
- [x] Create WeaponTypeExtensions (classification & helpers)
- [x] Create ItemRarityExtensions (colors, multipliers)
- [x] Verify ItemType enum exists
- [x] Verify ItemRarity enum exists
- [x] Build successful

---

## ?? Summary

? **WeaponType Enum** - 13 weapon types with detailed classification
? **ArmorMaterial Enum** - 5 armor materials for equipment pieces
? **WeaponTypeExtensions** - Helper methods for weapon logic
? **ItemRarityExtensions** - Color coding, stat multipliers, drop rates
? **Build Successful** - All enums compile without errors

**Ready for Step 3: Update Item Entity with Equipment Properties!** ??

---

**What would you like to do next?**
- **A)** Update Item entity with equipment properties
- **B)** Create CharacterEquipment tracking entity
- **C)** Discuss how stat bonuses will work
- **D)** Take a break and review

Let me know! ??
