# Quick Fix Action Plan - Code Quality Improvements

## Immediate Fixes (Can be done today)

### Fix #1: Add Missing Stat Bonuses to Item Entity ?? 5 minutes
**File**: `src\Nightstorm.Core\Entities\Item.cs`
**Lines to add after line 45** (after WisdomBonus):

```csharp
/// <summary>
/// Gets or sets the spirit bonus provided by this item.
/// </summary>
public int SpiritBonus { get; set; }

/// <summary>
/// Gets or sets the luck bonus provided by this item.
/// </summary>
public int LuckBonus { get; set; }
```

**Impact**: Allows items to buff all 7 character stats consistently.

---

### Fix #2: Remove Duplicate Extension Methods ?? 10 minutes
**File**: `src\Nightstorm.Core\Entities\Character.cs`

**Delete these methods** (lines ~140-195):
```csharp
public int GetMeleeAttackPower(ICharacterStatsService statsService) { }
public int GetRangedAttackPower(ICharacterStatsService statsService) { }
public int GetMagicPower(ICharacterStatsService statsService) { }
public int GetHeavyMeleeDefense(ICharacterStatsService statsService) { }
public int GetFastMeleeDefense(ICharacterStatsService statsService) { }
public int GetElementalMagicDefense(ICharacterStatsService statsService) { }
public int GetSpiritualMagicDefense(ICharacterStatsService statsService) { }
public AttackType GetAttackType(ICharacterStatsService statsService) { }
```

**Keep**: The same methods already exist in `CharacterExtensions.cs` and `CharacterCombatExtensions.cs`

**Impact**: Removes ~60 lines of duplicate code, follows DRY principle.

---

### Fix #3: Add Combat System to Monster Entity ?? 15 minutes
**File**: `src\Nightstorm.Core\Entities\Monster.cs`

**Replace** (lines 24-30):
```csharp
/// <summary>
/// Gets or sets the base attack damage.
/// </summary>
public int AttackDamage { get; set; }

/// <summary>
/// Gets or sets the defense rating.
/// </summary>
public int Defense { get; set; }
```

**With**:
```csharp
/// <summary>
/// Gets or sets the attack type used by this monster.
/// </summary>
public AttackType AttackType { get; set; }

/// <summary>
/// Gets or sets the attack power (replaces AttackDamage).
/// </summary>
public int AttackPower { get; set; }

/// <summary>
/// Gets or sets the armor type (determines defense bonuses).
/// </summary>
public ArmorType ArmorType { get; set; }

/// <summary>
/// Gets or sets the Heavy Melee defense value.
/// </summary>
public int HeavyMeleeDefense { get; set; }

/// <summary>
/// Gets or sets the Fast Melee defense value.
/// </summary>
public int FastMeleeDefense { get; set; }

/// <summary>
/// Gets or sets the Elemental Magic defense value.
/// </summary>
public int ElementalMagicDefense { get; set; }

/// <summary>
/// Gets or sets the Spiritual Magic defense value.
/// </summary>
public int SpiritualMagicDefense { get; set; }
```

**Impact**: Monsters can now participate in the four-quadrant combat system.

---

## Short-term Improvements (Next session)

### Improvement #1: Extract Value Objects from Character
**Create**: `src\Nightstorm.Core\ValueObjects\CharacterStats.cs`

```csharp
namespace Nightstorm.Core.ValueObjects;

/// <summary>
/// Value object representing character base statistics.
/// </summary>
public record CharacterStats
{
    public int Strength { get; init; }
    public int Dexterity { get; init; }
    public int Constitution { get; init; }
    public int Intelligence { get; init; }
    public int Wisdom { get; init; }
    public int Spirit { get; init; }
    public int Luck { get; init; }

    public CharacterStats(int str, int dex, int con, int @int, int wis, int spr, int luck)
    {
        Strength = str;
        Dexterity = dex;
        Constitution = con;
        Intelligence = @int;
        Wisdom = wis;
        Spirit = spr;
        Luck = luck;
    }
}
```

**Create**: `src\Nightstorm.Core\ValueObjects\CharacterResources.cs`

```csharp
namespace Nightstorm.Core.ValueObjects;

/// <summary>
/// Value object representing character resources (health, mana, etc).
/// </summary>
public record CharacterResources
{
    public int CurrentHealth { get; init; }
    public int MaxHealth { get; init; }
    public int CurrentMana { get; init; }
    public int MaxMana { get; init; }
    public long Gold { get; init; }
    public long Experience { get; init; }

    public CharacterResources WithHealth(int current, int max) 
        => this with { CurrentHealth = current, MaxHealth = max };
    
    public CharacterResources WithMana(int current, int max) 
        => this with { CurrentMana = current, MaxMana = max };
}
```

---

### Improvement #2: Configuration-Based Class Stats
**Create**: `src\Nightstorm.Core\Configuration\CharacterClassConfiguration.cs`

```csharp
namespace Nightstorm.Core.Configuration;

public static class CharacterClassConfiguration
{
    public static readonly IReadOnlyDictionary<CharacterClass, CharacterStats> BaseStats = 
        new Dictionary<CharacterClass, CharacterStats>
        {
            [CharacterClass.Paladin] = new(str: 14, dex: 10, con: 14, @int: 8, wis: 12, spr: 12, luck: 10),
            [CharacterClass.Warden] = new(str: 14, dex: 12, con: 14, @int: 8, wis: 10, spr: 12, luck: 10),
            [CharacterClass.DarkKnight] = new(str: 14, dex: 10, con: 16, @int: 8, wis: 8, spr: 12, luck: 12),
            [CharacterClass.Duelist] = new(str: 10, dex: 16, con: 10, @int: 8, wis: 8, spr: 10, luck: 18),
            [CharacterClass.Dragoon] = new(str: 14, dex: 14, con: 12, @int: 8, wis: 8, spr: 12, luck: 12),
            [CharacterClass.Monk] = new(str: 8, dex: 16, con: 14, @int: 8, wis: 10, spr: 14, luck: 10),
            [CharacterClass.Rogue] = new(str: 8, dex: 16, con: 10, @int: 10, wis: 8, spr: 12, luck: 16),
            [CharacterClass.Wizard] = new(str: 8, dex: 10, con: 10, @int: 18, wis: 8, spr: 14, luck: 12),
            [CharacterClass.Sorcerer] = new(str: 8, dex: 10, con: 12, @int: 16, wis: 8, spr: 14, luck: 12),
            [CharacterClass.Necromancer] = new(str: 8, dex: 10, con: 12, @int: 16, wis: 10, spr: 12, luck: 12),
            [CharacterClass.Cleric] = new(str: 10, dex: 10, con: 12, @int: 8, wis: 16, spr: 14, luck: 10),
            [CharacterClass.Druid] = new(str: 8, dex: 12, con: 12, @int: 10, wis: 16, spr: 12, luck: 10),
            [CharacterClass.Ranger] = new(str: 12, dex: 16, con: 12, @int: 8, wis: 12, spr: 10, luck: 10),
            [CharacterClass.Bard] = new(str: 8, dex: 12, con: 10, @int: 12, wis: 8, spr: 16, luck: 14),
            [CharacterClass.Gunslinger] = new(str: 10, dex: 16, con: 10, @int: 8, wis: 8, spr: 10, luck: 18),
            [CharacterClass.Alchemist] = new(str: 8, dex: 14, con: 10, @int: 16, wis: 8, spr: 10, luck: 14),
        };
}
```

**Then replace** the massive switch statement in `Character.SetBaseStatsForClass()`:

```csharp
private void SetBaseStatsForClass()
{
    var stats = CharacterClassConfiguration.BaseStats[Class];
    Strength = stats.Strength;
    Dexterity = stats.Dexterity;
    Constitution = stats.Constitution;
    Intelligence = stats.Intelligence;
    Wisdom = stats.Wisdom;
    Spirit = stats.Spirit;
    Luck = stats.Luck;
}
```

**Impact**: Reduces method from 300+ lines to 8 lines, follows Open/Closed Principle.

---

## Testing Plan

### After Immediate Fixes
Run these tests to verify nothing broke:

```bash
# Run all unit tests
dotnet test

# Specifically check character stats tests
dotnet test --filter "FullyQualifiedName~CharacterStatsServiceTests"

# Run balance simulation
dotnet test --filter "FullyQualifiedName~CharacterBalanceSimulationTests"
```

### New Tests Needed
After adding combat system to Monster:

```csharp
[Test]
public void Monster_WithHeavyArmor_HasHighPhysicalDefense()
{
    var monster = new Monster 
    { 
        Name = "Iron Golem",
        ArmorType = ArmorType.Heavy,
        HeavyMeleeDefense = 50,
        FastMeleeDefense = 50,
        ElementalMagicDefense = 20,
        SpiritualMagicDefense = 20
    };
    
    monster.HeavyMeleeDefense.Should().BeGreaterThan(monster.ElementalMagicDefense);
}
```

---

## Migration Considerations

### Database Migrations Required

After Fix #1 (Item changes):
```bash
dotnet ef migrations add AddSpiritAndLuckBonusesToItem --project src/Nightstorm.Data
```

After Fix #3 (Monster changes):
```bash
dotnet ef migrations add UpdateMonsterCombatSystem --project src/Nightstorm.Data
```

### Breaking Changes
?? **Fix #3** (Monster) is a breaking change:
- Old: `monster.AttackDamage`, `monster.Defense`
- New: `monster.AttackPower`, `monster.ArmorType`, 4 defense values

**Migration Strategy**:
```csharp
// In migration Up() method
migrationBuilder.Sql(@"
    UPDATE Monsters 
    SET AttackPower = AttackDamage,
        ArmorType = 1, -- Default to Heavy
        HeavyMeleeDefense = Defense,
        FastMeleeDefense = Defense,
        ElementalMagicDefense = Defense / 2,
        SpiritualMagicDefense = Defense / 2
");
```

---

## Checklist

### Before Starting
- [ ] Create feature branch: `git checkout -b refactor/code-quality-improvements`
- [ ] Backup database (if testing locally)
- [ ] Run all tests to establish baseline

### After Each Fix
- [ ] Run unit tests
- [ ] Check for compilation errors
- [ ] Update related documentation
- [ ] Commit with descriptive message

### Before Merging
- [ ] All tests passing ?
- [ ] Documentation updated ?
- [ ] Code reviewed ?
- [ ] No merge conflicts ?

---

## Estimated Time

| Task | Time | Difficulty |
|------|------|-----------|
| Fix #1 (Item bonuses) | 5 min | Easy |
| Fix #2 (Remove duplicates) | 10 min | Easy |
| Fix #3 (Monster combat) | 30 min | Medium |
| **Total Immediate** | **45 min** | |
| Value Objects | 30 min | Medium |
| Configuration class | 20 min | Easy |
| **Total Short-term** | **50 min** | |
| **GRAND TOTAL** | **~2 hours** | |

---

## Questions to Resolve

1. **Monster Combat**: Should monsters have primary stats (STR, DEX, etc.) like characters, or just final combat values?
   - **Recommendation**: Keep it simple with final values for now

2. **Item Stat Bonuses**: Should items be able to give negative bonuses (curses)?
   - **Recommendation**: Yes, add cursed items later as a feature

3. **Character Entity**: Should we move all business logic to services, or keep some in entity?
   - **Recommendation**: Gradual migration to services, start with combat calculations

---

**Ready to implement?** Start with the Immediate Fixes! ??
