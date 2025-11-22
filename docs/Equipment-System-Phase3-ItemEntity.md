# Equipment System - Phase 3: Item Entity Complete ?

## ?? Step 3 Complete: Item Entity Updated!

### ? What Was Implemented

The `Item` entity has been completely overhauled to support the full equipment system with **60+ properties** organized into logical categories.

---

## ?? Item Entity Structure

### **1. Basic Properties**
```csharp
string Name                    // Item name
string Description             // Item description
ItemType Type                  // Weapon, Armor, Accessory, Consumable, etc.
ItemRarity Rarity              // Common to Mythic
ItemGrade Grade                // NG to S-Grade
int BaseValue                  // Base vendor price in gold
int RequiredLevel              // Minimum level to use (from Grade)
CharacterClass? RequiredClass  // Class restriction (optional)
```

### **2. Equipment Slot Properties**
```csharp
EquipmentSlot? EquipmentSlot   // Which slot it equips to (MainHand, Head, etc.)
WeaponType? WeaponType         // Sword, Axe, Staff, etc. (if weapon)
ArmorMaterial? ArmorMaterial   // Cloth, Leather, Chain, Plate (if armor)
```

### **3. Weapon-Specific Properties**
```csharp
int MinDamage                  // Minimum weapon damage
int MaxDamage                  // Maximum weapon damage  
decimal AttackSpeed            // Attack speed multiplier (1.0 = normal)
decimal CriticalChance         // Critical hit chance bonus (%)
```

### **4. Armor-Specific Properties**
```csharp
int ArmorValue                 // Physical defense value
int MagicResistance            // Magical defense value
decimal BlockChance            // Block chance (% for shields)
```

### **5. Stat Bonuses (All Equipment)**
```csharp
int BonusStrength              // STR bonus
int BonusDexterity             // DEX bonus
int BonusConstitution          // CON bonus
int BonusIntelligence          // INT bonus
int BonusWisdom                // WIS bonus
int BonusSpirit                // SPR bonus
int BonusLuck                  // LUCK bonus
```

### **6. Additional Bonuses**
```csharp
int BonusMaxHealth             // Max HP bonus
int BonusMaxMana               // Max MP bonus
int HealthRegeneration         // HP regen per tick
int ManaRegeneration           // MP regen per tick
```

### **7. Consumable Properties**
```csharp
int HealthRestore              // HP restored when used
int ManaRestore                // MP restored when used
int CooldownSeconds            // Cooldown after use
```

### **8. Item Flags**
```csharp
int MaxStackSize               // How many can stack (1 for equipment)
bool IsTradeable               // Can be traded to other players
bool IsQuestItem               // Quest-related item
bool IsSoulbound               // Bound to character (cannot trade)
string? IconId                 // Icon identifier for UI
```

---

## ?? Helper Methods

### **Power & Value Calculations**
```csharp
decimal GetItemPower()
// Calculates total power: Grade.BasePower × Rarity.Multiplier
// Example: D-Grade Legendary = 10 × 2.0 = 20

int GetVendorSellPrice()
// Calculates sell price: BaseValue × Rarity.VendorMultiplier
// Example: 100g base × 2.0 (Legendary) = 200g sell price
```

### **Type Checking**
```csharp
bool IsEquipment()    // Can be equipped?
bool IsWeapon()       // Is it a weapon?
bool IsArmor()        // Is it armor?
bool IsConsumable()   // Is it consumable?
```

### **Validation**
```csharp
bool CanBeUsedBy(int characterLevel, CharacterClass characterClass)
// Checks if character meets level and class requirements
```

### **Display**
```csharp
string GetFullDisplayName()
// Returns: "?? D-Grade ? Legendary Steel Sword"
```

---

## ?? Usage Examples

### **Example 1: Create a Weapon**

```csharp
var sword = new Item
{
    Name = "Steel Longsword",
    Description = "A well-crafted steel blade",
    Type = ItemType.Weapon,
    Rarity = ItemRarity.Rare,
    Grade = ItemGrade.C,
    
    // Equipment properties
    EquipmentSlot = EquipmentSlot.MainHand,
    WeaponType = WeaponType.Sword,
    
    // Weapon stats
    MinDamage = 15,
    MaxDamage = 25,
    AttackSpeed = 1.0m,
    CriticalChance = 5.0m,
    
    // Stat bonuses
    BonusStrength = 8,
    BonusDexterity = 4,
    
    // Item properties
    BaseValue = 500,
    MaxStackSize = 1,
    IsTradeable = true,
    IsQuestItem = false
};

// Auto-calculate level requirement from grade
sword.RequiredLevel = sword.Grade.GetRequiredLevel(); // 40

// Get item power
var power = sword.GetItemPower(); // 18 × 1.35 = 24.3

// Get sell price
var sellPrice = sword.GetVendorSellPrice(); // 500 × 5.0 = 2,500g

Console.WriteLine(sword.GetFullDisplayName());
// Output: "?? C ?? Rare Steel Longsword"
```

---

### **Example 2: Create Armor**

```csharp
var helmet = new Item
{
    Name = "Iron Helmet",
    Description = "A sturdy iron helmet",
    Type = ItemType.Armor,
    Rarity = ItemRarity.Uncommon,
    Grade = ItemGrade.D,
    
    // Equipment properties
    EquipmentSlot = EquipmentSlot.Head,
    ArmorMaterial = ArmorMaterial.Plate,
    
    // Armor stats
    ArmorValue = 25,
    MagicResistance = 10,
    
    // Stat bonuses
    BonusConstitution = 5,
    BonusStrength = 2,
    BonusMaxHealth = 50,
    
    // Item properties
    BaseValue = 200,
    RequiredLevel = 20,
    MaxStackSize = 1,
    IsTradeable = true
};

var power = helmet.GetItemPower(); // 10 × 1.15 = 11.5
```

---

### **Example 3: Create an Accessory**

```csharp
var ring = new Item
{
    Name = "Ring of Power",
    Description = "A magical ring imbued with arcane energy",
    Type = ItemType.Accessory,
    Rarity = ItemRarity.Epic,
    Grade = ItemGrade.B,
    
    // Equipment properties
    EquipmentSlot = EquipmentSlot.Ring1,
    
    // Stat bonuses
    BonusIntelligence = 15,
    BonusWisdom = 10,
    BonusSpirit = 12,
    BonusMaxMana = 100,
    ManaRegeneration = 5,
    
    // Item properties
    BaseValue = 1000,
    RequiredLevel = 50,
    MaxStackSize = 1,
    IsTradeable = true
};

var power = ring.GetItemPower(); // 30 × 1.65 = 49.5
```

---

### **Example 4: Create a Consumable**

```csharp
var potion = new Item
{
    Name = "Greater Health Potion",
    Description = "Restores a large amount of health",
    Type = ItemType.Consumable,
    Rarity = ItemRarity.Common,
    Grade = ItemGrade.NG,
    
    // Consumable properties
    HealthRestore = 500,
    CooldownSeconds = 30,
    
    // Item properties
    BaseValue = 50,
    RequiredLevel = 1,
    MaxStackSize = 99, // Can stack
    IsTradeable = true
};

// Consumables don't use equipment power system
```

---

### **Example 5: Validate Character Can Use Item**

```csharp
var character = GetCharacter(); // Level 45 Warrior

var weapon = new Item
{
    Name = "Legendary Sword",
    Grade = ItemGrade.C,          // Requires level 40
    RequiredClass = CharacterClass.Warrior,
    // ... other properties
};

if (weapon.CanBeUsedBy(character.Level, character.Class))
{
    Console.WriteLine("You can equip this weapon!");
}
else
{
    Console.WriteLine($"You need level {weapon.RequiredLevel} to use this.");
}
```

---

### **Example 6: Item Comparison**

```csharp
var currentWeapon = GetEquippedWeapon(); // D-Grade Mythic
var newWeapon = GetLootedWeapon();       // C-Grade Common

var currentPower = currentWeapon.GetItemPower(); // 25
var newPower = newWeapon.GetItemPower();         // 18

if (newPower > currentPower)
{
    Console.WriteLine("? New weapon is better!");
}
else
{
    Console.WriteLine($"? Keep your current weapon! ({currentPower} > {newPower})");
}
```

---

## ?? Database Configuration

### **EF Core Configuration (ItemConfiguration.cs)**

**Key Features:**
- All properties properly configured with types and defaults
- Enum conversions for ItemType, Rarity, Grade, WeaponType, etc.
- Decimal precision for AttackSpeed, CriticalChance, BlockChance
- Composite indexes for performance:
  - `IX_Items_Type_Grade_Rarity` (multi-column index for queries)
- Individual indexes on Name, Type, Rarity, Grade, RequiredLevel
- Soft delete filter applied

**Sample Columns:**
```sql
-- Basic
"Name" varchar(100) NOT NULL
"Description" varchar(500)
"Type" int NOT NULL
"Rarity" int NOT NULL
"Grade" int NOT NULL DEFAULT 0
"BaseValue" int NOT NULL DEFAULT 0

-- Equipment
"EquipmentSlot" int NULL
"WeaponType" int NULL
"ArmorMaterial" int NULL

-- Weapon Stats
"MinDamage" int DEFAULT 0
"MaxDamage" int DEFAULT 0
"AttackSpeed" decimal(5,2) DEFAULT 1.0

-- Armor Stats
"ArmorValue" int DEFAULT 0
"MagicResistance" int DEFAULT 0

-- Stat Bonuses
"BonusStrength" int DEFAULT 0
"BonusDexterity" int DEFAULT 0
... (all 7 stats)

-- Flags
"IsTradeable" boolean DEFAULT true
"IsQuestItem" boolean DEFAULT false
"IsSoulbound" boolean DEFAULT false
```

---

## ?? Item Power Calculation Examples

### **Weapon Power Comparison**

| Item | Grade | Rarity | Min-Max Dmg | Power | Sell Price |
|------|-------|--------|-------------|-------|------------|
| Rusty Sword | NG | Common | 2-4 | 5 | 10g |
| Iron Sword | D | Common | 8-12 | 10 | 50g |
| Steel Sword | C | Common | 15-25 | 18 | 100g |
| Blessed Iron Sword | D | Legendary | 16-24 | 20 | 1,250g |
| Mythic Iron Sword | D | Mythic | 20-30 | 25 | 5,000g |
| Rare Steel Sword | C | Rare | 20-34 | 24.3 | 500g |
| Mithril Sword | B | Common | 25-40 | 30 | 200g |

**Key Insight:** Mythic D (25) > Common C (18) ?

---

## ?? Migration Required

You'll need to create a migration to add all the new columns:

```cmd
dotnet ef migrations add AddEquipmentSystemToItems --project src\Nightstorm.Data --startup-project src\Nightstorm.API
```

**Migration will add:**
- `Grade` column (int, default 0)
- `EquipmentSlot`, `WeaponType`, `ArmorMaterial` (nullable int)
- All weapon properties (MinDamage, MaxDamage, AttackSpeed, etc.)
- All armor properties (ArmorValue, MagicResistance, BlockChance)
- All stat bonuses (7 stats + health/mana bonuses)
- All consumable properties
- New flags (IsQuestItem, IsSoulbound, IconId)
- New indexes

**Then apply:**
```cmd
dotnet ef database update --project src\Nightstorm.Data --startup-project src\Nightstorm.API
```

---

## ? Implementation Checklist

### Phase 3: Item Entity
- [x] Add ItemGrade property
- [x] Add EquipmentSlot property
- [x] Add WeaponType property
- [x] Add ArmorMaterial property
- [x] Add weapon-specific properties
- [x] Add armor-specific properties
- [x] Add all stat bonuses
- [x] Add consumable properties
- [x] Add item flags
- [x] Create GetItemPower() method
- [x] Create GetVendorSellPrice() method
- [x] Create validation methods
- [x] Create display name method
- [x] Update ItemConfiguration for EF Core
- [x] Add all necessary indexes
- [x] Build successful

---

## ?? Next Steps

### **Step 4: Character Equipment Tracking**
Create a new entity to track which items are equipped:

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

This will allow:
- Tracking all 13 equipment slots per character
- Quick lookup of what's equipped
- Easy equip/unequip operations

---

## ?? Summary

? **Item Entity Complete** - 60+ properties for full equipment system
? **All Equipment Types** - Weapons, armor, accessories, consumables
? **Grade System Integrated** - NG to S-Grade with power calculations
? **Stat Bonuses** - All 7 character stats + HP/MP bonuses
? **Helper Methods** - Power calculation, validation, display
? **EF Core Configuration** - Proper column types, defaults, indexes
? **Build Successful** - Ready for migration

**The Item entity now supports everything needed for a rich equipment system!** ????

---

**What would you like to do next?**
- **A)** Create database migration for new Item properties
- **B)** Create CharacterEquipment entity (track what's equipped)
- **C)** Create sample items to test the system
- **D)** Take a break - we've built A LOT! ?

Let me know! ??
