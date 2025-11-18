# Character Stats Service

## Overview

The `ICharacterStatsService` provides centralized calculation logic for all character combat statistics and derived values. This follows the **Single Responsibility Principle** by separating calculation logic from the entity.

## Architecture

```
Character (Entity)          → Pure data/state
    ↓
ICharacterStatsService      → Business logic for calculations
    ↓
CharacterStatsService       → Implementation
    ↓
CharacterExtensions         → Convenience methods
```

## Usage

### 1. Register the Service (Dependency Injection)

In your `Program.cs` or startup configuration:

```csharp
// In Nightstorm.Core or Infrastructure layer
services.AddScoped<ICharacterStatsService, CharacterStatsService>();
```

### 2. Use the Service Directly

```csharp
public class CharacterService
{
    private readonly ICharacterStatsService _statsService;

    public CharacterService(ICharacterStatsService statsService)
    {
        _statsService = statsService;
    }

    public void DisplayCharacterStats(Character character)
    {
        var map = _statsService.CalculateMeleeAttackPower(
            character.Strength, 
            character.Dexterity
        );
        
        var rap = _statsService.CalculateRangedAttackPower(
            character.Dexterity, 
            character.Strength
        );
        
        var magicPower = _statsService.CalculateMagicPower(
            character.Class,
            character.Intelligence,
            character.Wisdom,
            character.Spirit
        );

        Console.WriteLine($"MAP: {map}, RAP: {rap}, Magic: {magicPower}");
    }
}
```

### 3. Use Extension Methods (Recommended)

```csharp
using Nightstorm.Core.Extensions;

public class CombatService
{
    private readonly ICharacterStatsService _statsService;

    public CombatService(ICharacterStatsService statsService)
    {
        _statsService = statsService;
    }

    public void ProcessAttack(Character attacker)
    {
        // Clean and readable syntax
        var meleeAttack = attacker.GetMeleeAttackPower(_statsService);
        var rangedAttack = attacker.GetRangedAttackPower(_statsService);
        var magicAttack = attacker.GetMagicPower(_statsService);

        // Recalculate after stat changes
        attacker.RecalculateMaxHealth(_statsService);
        attacker.RecalculateMaxMana(_statsService);
    }
}
```

### 4. When Stats Change (Equipment, Buffs, Leveling)

```csharp
public void EquipItem(Character character, Item item)
{
    // Apply stat bonuses
    character.Strength += item.StrengthBonus;
    character.Intelligence += item.IntelligenceBonus;

    // Recalculate derived stats
    character.RecalculateMaxHealth(_statsService);
    character.RecalculateMaxMana(_statsService);
    
    // Heal to new max if needed
    character.CurrentHealth = Math.Min(character.CurrentHealth, character.MaxHealth);
    character.CurrentMana = Math.Min(character.CurrentMana, character.MaxMana);
}
```

## Formulas

### Combat Stats
- **Melee Attack Power (MAP)**: `(STR × 4) + (DEX × 2) + 10`
- **Ranged Attack Power (RAP)**: `(DEX × 4) + (STR × 2) + 10`
- **Magic Power**: `(Primary_Caster_Stat × 4) + (SPR × 2) + 10`

### HP/MP Calculations

**Physical Archetype** (Paladin, Warden, DarkKnight, Duelist, Dragoon, Monk, Rogue, Ranger, Gunslinger):
- **HP**: `Base_HP(100) + (CON × 20) + ((STR + DEX) × 3)`
- **MP**: `Base_MP(100) + (SPR × 10) + ((INT + WIS) × 3)`

**Caster Archetype** (Wizard, Sorcerer, Necromancer, Cleric, Druid, Bard, Alchemist):
- **HP**: `Base_HP(100) + (CON × 10) + ((STR + DEX) × 3)`
- **MP**: `Base_MP(100) + (SPR × 20) + ((INT + WIS) × 3)`

### Primary Casting Stats

**Wisdom-based**: Paladin, Warden, Cleric, Druid, Ranger
**Intelligence-based**: DarkKnight, Wizard, Sorcerer, Necromancer, Bard, Alchemist

## Benefits

✅ **Single Responsibility**: Entity only manages state, service handles calculations  
✅ **Testability**: Easy to unit test calculations independently  
✅ **Maintainability**: Changes to formulas only require updating the service  
✅ **Reusability**: Service can be used across different layers (API, Bot, Game Logic)  
✅ **Flexibility**: Easy to add new calculation methods or modify existing ones  
✅ **Clean Architecture**: Follows dependency inversion principle  

## Testing Example

```csharp
[Fact]
public void CalculateMeleeAttackPower_WithStandardStats_ReturnsCorrectValue()
{
    // Arrange
    var service = new CharacterStatsService();
    var strength = 14;
    var dexterity = 10;
    var expected = (14 * 4) + (10 * 2) + 10; // = 86

    // Act
    var result = service.CalculateMeleeAttackPower(strength, dexterity);

    // Assert
    Assert.Equal(expected, result);
}
```
