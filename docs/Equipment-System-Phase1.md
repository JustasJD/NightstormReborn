# Equipment System - Phase 1: Equipment Slots ?

## ?? Step 1 Complete: Equipment Slot Enum

### ? What Was Created

#### 1. **EquipmentSlot Enum**
**File:** `src\Nightstorm.Core\Enums\EquipmentSlot.cs`

**13 Equipment Slots Defined:**

| Slot | Enum Value | Category | Description |
|------|------------|----------|-------------|
| **MainHand** | 0 | Weapon | One-handed or two-handed weapon |
| **OffHand** | 1 | Weapon | Shield or one-handed weapon (blocked by two-handed) |
| **Head** | 2 | Armor | Helmet, Hat, Crown |
| **Shoulders** | 3 | Armor | Pauldrons, Mantle |
| **Chest** | 4 | Armor | Primary armor piece |
| **Hands** | 5 | Armor | Gloves, Gauntlets |
| **Belt** | 6 | Armor | Belt, Sash |
| **Legs** | 7 | Armor | Pants, Greaves |
| **Feet** | 8 | Armor | Boots, Shoes |
| **Cloak** | 9 | Accessory | Cloak, Cape |
| **Amulet** | 10 | Accessory | Necklace, Pendant |
| **Ring1** | 11 | Accessory | Ring (left hand) |
| **Ring2** | 12 | Accessory | Ring (right hand) |

**Total: 13 slots** (No trinkets as requested)

---

#### 2. **EquipmentSlotExtensions**
**File:** `src\Nightstorm.Core\Enums\EquipmentSlotExtensions.cs`

**Helper Methods:**

```csharp
// Display & UI
slot.GetDisplayName()    // "Main Hand", "Ring 1", etc.
slot.GetIcon()           // ??, ???, ??, etc. (for Discord)

// Slot Classification
slot.IsWeaponSlot()      // MainHand or OffHand
slot.IsArmorSlot()       // Head, Shoulders, Chest, Hands, Belt, Legs, Feet
slot.IsAccessorySlot()   // Cloak, Amulet, Ring1, Ring2
slot.IsRingSlot()        // Ring1 or Ring2

// Slot Collections
EquipmentSlotExtensions.GetAllSlots()        // All 13 slots
EquipmentSlotExtensions.GetWeaponSlots()     // MainHand, OffHand
EquipmentSlotExtensions.GetArmorSlots()      // All 7 armor slots
EquipmentSlotExtensions.GetAccessorySlots()  // All 4 accessory slots
```

---

## ?? Equipment Slot Categories

### **Weapon Slots (2)**
```
MainHand ??  - Primary weapon slot
OffHand  ???  - Secondary weapon/shield (blocked by two-handed weapons)
```

**Rules:**
- If MainHand has **two-handed weapon** ? OffHand is **blocked**
- If MainHand has **one-handed weapon** ? OffHand can be **weapon or shield**

### **Armor Slots (7)**
```
Head      ?? - Helmet, Hat, Crown
Shoulders ??? - Pauldrons, Mantle
Chest     ??? - Primary armor piece (highest defense)
Hands     ?? - Gloves, Gauntlets
Belt      ?? - Belt, Sash
Legs      ?? - Pants, Greaves
Feet      ?? - Boots, Shoes
```

### **Accessory Slots (4)**
```
Cloak   ?? - Cloak, Cape (resistances)
Amulet  ?? - Necklace, Pendant (magical bonuses)
Ring1   ?? - Ring (left hand)
Ring2   ?? - Ring (right hand)
```

---

## ?? Usage Examples

### Example 1: Check Slot Type
```csharp
var slot = EquipmentSlot.MainHand;

if (slot.IsWeaponSlot())
{
    Console.WriteLine("This is a weapon slot!");
}

Console.WriteLine($"Slot: {slot.GetDisplayName()} {slot.GetIcon()}");
// Output: "Slot: Main Hand ??"
```

### Example 2: Loop Through All Slots
```csharp
foreach (var slot in EquipmentSlotExtensions.GetAllSlots())
{
    Console.WriteLine($"{slot.GetIcon()} {slot.GetDisplayName()}");
}

// Output:
// ?? Main Hand
// ??? Off Hand
// ?? Head
// ...
```

### Example 3: Discord Bot Display
```csharp
var equipmentDisplay = new EmbedBuilder()
    .WithTitle("?? Equipment")
    .WithDescription("Your currently equipped items:");

foreach (var slot in EquipmentSlotExtensions.GetAllSlots())
{
    var item = character.GetEquippedItem(slot); // Future method
    var itemName = item?.Name ?? "Empty";
    
    embedBuilder.AddField(
        $"{slot.GetIcon()} {slot.GetDisplayName()}", 
        itemName, 
        inline: true
    );
}
```

**Would display:**
```
?? Equipment
Your currently equipped items:

?? Main Hand          ??? Off Hand           ?? Head
Steel Sword           Wooden Shield         Iron Helmet

??? Shoulders          ??? Chest              ?? Hands
Leather Pauldrons    Chainmail Armor       Empty

?? Belt               ?? Legs                ?? Feet
Sturdy Belt          Iron Greaves          Leather Boots

?? Cloak              ?? Amulet              ?? Ring 1
Empty                Magic Amulet           Gold Ring

?? Ring 2
Empty
```

---

## ?? Next Steps

Now that we have the equipment slots defined, we can proceed to:

### **Step 2: Item Types & Categories**
Create enums for:
- ItemType (Weapon, Armor, Accessory, Consumable, etc.)
- WeaponType (Sword, Axe, Staff, Bow, etc.)
- ArmorType (Plate, Chain, Leather, Cloth)
- ItemRarity (Common, Uncommon, Rare, Epic, Legendary)

### **Step 3: Update Item Entity**
Add equipment properties to the existing Item entity:
- EquipmentSlot (which slot it goes in)
- ItemType, WeaponType, ArmorType
- Stat bonuses (Strength, Dexterity, etc.)
- Requirements (Level, Class, Stats)
- IsTwoHanded flag for weapons

### **Step 4: Character Equipment Tracking**
Create a new entity to track equipped items:
- CharacterEquipment (Character ? EquipmentSlot ? Item)
- Methods: Equip(), Unequip(), GetEquippedItem()

### **Step 5: Stat Calculation**
Update stat calculation to include equipment bonuses:
- Base stats from character
- + Equipment bonuses
- = Total stats

### **Step 6: Equip/Unequip API**
Create API endpoints:
- POST /api/characters/me/equipment/{slot}/equip/{itemId}
- DELETE /api/characters/me/equipment/{slot}/unequip
- GET /api/characters/me/equipment (view all equipped items)

---

## ?? Equipment System Architecture

```
???????????????????????????????????????????????????
? EquipmentSlot Enum (13 slots)                   ?
? ? COMPLETED                                     ?
???????????????????????????????????????????????????
                    ?
???????????????????????????????????????????????????
? Item Entity (with equipment properties)         ?
? ? NEXT STEP                                     ?
? - EquipmentSlot                                  ?
? - ItemType, WeaponType, ArmorType                ?
? - Stat bonuses                                   ?
? - Requirements                                   ?
???????????????????????????????????????????????????
                    ?
???????????????????????????????????????????????????
? CharacterEquipment Entity                        ?
? ? PENDING                                       ?
? - Maps Character ? Slot ? Item                   ?
???????????????????????????????????????????????????
                    ?
???????????????????????????????????????????????????
? Equip/Unequip Logic & Validation                ?
? ? PENDING                                       ?
? - Can equip? (level, class, slot validation)    ?
? - Two-handed weapon handling                    ?
? - Stat recalculation                             ?
???????????????????????????????????????????????????
                    ?
???????????????????????????????????????????????????
? API Endpoints                                    ?
? ? PENDING                                       ?
? - Equip item                                     ?
? - Unequip item                                   ?
? - View equipment                                 ?
???????????????????????????????????????????????????
```

---

## ? Checklist

### Phase 1: Equipment Slots
- [x] Create EquipmentSlot enum (13 slots)
- [x] Create EquipmentSlotExtensions helper methods
- [x] Add display names and icons
- [x] Add slot classification methods
- [x] Build successful

### Phase 2: Item Types (Next)
- [ ] Create ItemType enum
- [ ] Create WeaponType enum
- [ ] Create ArmorType enum
- [ ] Create ItemRarity enum
- [ ] Update Item entity with equipment properties

### Phase 3: Character Equipment (Future)
- [ ] Create CharacterEquipment entity
- [ ] Add EF Core configuration
- [ ] Create database migration
- [ ] Add navigation properties to Character

### Phase 4: Equip Logic (Future)
- [ ] Create equipment service
- [ ] Implement equip validation
- [ ] Implement stat calculation with equipment
- [ ] Handle two-handed weapon logic

### Phase 5: API Endpoints (Future)
- [ ] Create EquipmentController
- [ ] Add equip/unequip endpoints
- [ ] Add equipment view endpoint
- [ ] Test with Discord bot

---

## ?? Summary

? **Equipment Slot Enum Created** - 13 slots defined
? **Helper Methods Added** - Easy slot classification and display
? **Icons Defined** - Ready for Discord bot UI
? **Build Successful** - No errors

**Ready for Step 2: Item Types & Categories!** ??

---

**What would you like to do next?**
- **A)** Continue with Item Types (WeaponType, ArmorType, ItemRarity)
- **B)** Update the Item entity with equipment properties
- **C)** Discuss stat bonuses and how they'll affect character stats
- **D)** Take a break and review what we've built

Let me know! ??
