namespace Nightstorm.Core.Enums;

/// <summary>
/// Extension methods for WeaponType enum.
/// </summary>
public static class WeaponTypeExtensions
{
    /// <summary>
    /// Checks if the weapon is two-handed (requires both MainHand and OffHand slots).
    /// </summary>
    public static bool IsTwoHanded(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Greatsword or WeaponType.Greataxe
            or WeaponType.Warhammer or WeaponType.Spear or WeaponType.Polearm
            or WeaponType.Staff or WeaponType.Bow or WeaponType.Crossbow
            or WeaponType.Rifle or WeaponType.FlaskThrower
            or WeaponType.Lute or WeaponType.Harp or WeaponType.Guitar;
    }

    /// <summary>
    /// Checks if the weapon is one-handed (allows off-hand usage).
    /// </summary>
    public static bool IsOneHanded(this WeaponType weaponType)
    {
        return !IsTwoHanded(weaponType) && !IsOffHandOnly(weaponType);
    }

    /// <summary>
    /// Checks if the item can only be equipped in off-hand slot.
    /// </summary>
    public static bool IsOffHandOnly(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Shield or WeaponType.Orb 
            or WeaponType.Tome or WeaponType.Talisman;
    }

    /// <summary>
    /// Checks if the weapon is melee (close-range combat).
    /// </summary>
    public static bool IsMelee(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Sword or WeaponType.Greatsword
            or WeaponType.Axe or WeaponType.Greataxe
            or WeaponType.Mace or WeaponType.Warhammer
            or WeaponType.Dagger or WeaponType.Knuckles
            or WeaponType.Spear or WeaponType.Polearm;
    }

    /// <summary>
    /// Checks if the weapon is ranged physical (bow, crossbow, firearms).
    /// </summary>
    public static bool IsRanged(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Bow or WeaponType.Crossbow
            or WeaponType.Pistol or WeaponType.Rifle
            or WeaponType.FlaskThrower;
    }

    /// <summary>
    /// Checks if the weapon is a firearm (gunslinger weapons).
    /// </summary>
    public static bool IsFirearm(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Pistol or WeaponType.Rifle;
    }

    /// <summary>
    /// Checks if the weapon is magical (spell casting).
    /// </summary>
    public static bool IsMagical(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Staff or WeaponType.Wand
            or WeaponType.Orb or WeaponType.Tome or WeaponType.Talisman or WeaponType.Symbol;
    }

    /// <summary>
    /// Checks if the weapon is a musical instrument (bard weapons).
    /// </summary>
    public static bool IsMusicalInstrument(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Lute or WeaponType.Harp or WeaponType.Flute or WeaponType.Guitar;
    }

    /// <summary>
    /// Checks if the weapon is a polearm (dragoon/warden).
    /// </summary>
    public static bool IsPolearm(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Spear or WeaponType.Polearm;
    }

    /// <summary>
    /// Checks if the weapon is monk-specific (unarmed combat).
    /// </summary>
    public static bool IsMonkWeapon(this WeaponType weaponType)
    {
        return weaponType == WeaponType.Knuckles;
    }

    /// <summary>
    /// Gets the primary damage type for the weapon.
    /// </summary>
    public static string GetDamageType(this WeaponType weaponType)
    {
        return weaponType switch
        {
            // Slashing
            WeaponType.Sword or WeaponType.Greatsword or WeaponType.Axe or WeaponType.Greataxe
                => "Slashing",
            
            // Crushing
            WeaponType.Mace or WeaponType.Warhammer or WeaponType.Knuckles
                => "Crushing",
            
            // Piercing
            WeaponType.Dagger or WeaponType.Spear
                => "Piercing",
            
            // Slashing/Piercing (Polearms)
            WeaponType.Polearm
                => "Slashing/Piercing",
            
            // Magical
            WeaponType.Staff or WeaponType.Wand or WeaponType.Orb or WeaponType.Tome or WeaponType.Talisman or WeaponType.Symbol
                => "Magical",
            
            // Musical/Support
            WeaponType.Lute or WeaponType.Harp or WeaponType.Flute or WeaponType.Guitar
                => "Musical/Magical",
            
            // Ranged Physical
            WeaponType.Bow or WeaponType.Crossbow
                => "Ranged Physical",
            
            // Firearms
            WeaponType.Pistol or WeaponType.Rifle
                => "Firearm",
            
            // Explosives
            WeaponType.FlaskThrower
                => "Explosive",
            
            // Defensive
            WeaponType.Shield
                => "Defensive",
            
            _ => "Physical"
        };
    }

    /// <summary>
    /// Gets the weapon icon for Discord display.
    /// </summary>
    public static string GetIcon(this WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Sword => "??",
            WeaponType.Greatsword => "???",
            WeaponType.Axe => "??",
            WeaponType.Greataxe => "??",
            WeaponType.Mace => "??",
            WeaponType.Warhammer => "??",
            WeaponType.Dagger => "???",
            WeaponType.Knuckles => "??",
            WeaponType.Spear => "??",
            WeaponType.Polearm => "??",
            WeaponType.Staff => "??",
            WeaponType.Wand => "??",
            WeaponType.Bow => "??",
            WeaponType.Crossbow => "??",
            WeaponType.Pistol => "??",
            WeaponType.Rifle => "??",
            WeaponType.FlaskThrower => "??",
            WeaponType.Lute => "??",
            WeaponType.Harp => "??",
            WeaponType.Flute => "??",
            WeaponType.Guitar => "??",
            WeaponType.Shield => "???",
            WeaponType.Orb => "??",
            WeaponType.Tome => "??",
            WeaponType.Talisman => "??",
            WeaponType.Symbol => "??",
            _ => "??"
        };
    }

    /// <summary>
    /// Gets the equipment slot(s) required for this weapon.
    /// </summary>
    public static EquipmentSlot[] GetRequiredSlots(this WeaponType weaponType)
    {
        // Off-hand only items
        if (IsOffHandOnly(weaponType))
        {
            return [EquipmentSlot.OffHand];
        }

        // Two-handed items
        if (weaponType.IsTwoHanded())
        {
            return [EquipmentSlot.MainHand, EquipmentSlot.OffHand];
        }

        // One-handed items (can use off-hand for dual-wield or shield)
        return [EquipmentSlot.MainHand];
    }

    /// <summary>
    /// Gets display name for weapon type.
    /// </summary>
    public static string GetDisplayName(this WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Greatsword => "Greatsword",
            WeaponType.Greataxe => "Greataxe",
            WeaponType.Warhammer => "Warhammer",
            WeaponType.Polearm => "Polearm",
            WeaponType.FlaskThrower => "Flask Thrower",
            _ => weaponType.ToString()
        };
    }

    /// <summary>
    /// Gets the recommended character classes for this weapon type.
    /// </summary>
    public static CharacterClass[] GetRecommendedClasses(this WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Sword => [CharacterClass.Paladin, CharacterClass.DarkKnight, CharacterClass.Duelist],
            WeaponType.Greatsword => [CharacterClass.Paladin, CharacterClass.DarkKnight],
            WeaponType.Axe or WeaponType.Greataxe => [CharacterClass.Warden],
            WeaponType.Mace or WeaponType.Warhammer => [CharacterClass.Paladin, CharacterClass.Cleric],
            WeaponType.Dagger => [CharacterClass.Rogue],
            WeaponType.Knuckles => [CharacterClass.Monk],
            WeaponType.Spear or WeaponType.Polearm => [CharacterClass.Dragoon, CharacterClass.Warden],
            WeaponType.Staff => [CharacterClass.Wizard, CharacterClass.Sorcerer, CharacterClass.Necromancer, CharacterClass.Druid],
            WeaponType.Wand => [CharacterClass.Wizard, CharacterClass.Sorcerer, CharacterClass.Necromancer],
            WeaponType.Bow or WeaponType.Crossbow => [CharacterClass.Ranger],
            WeaponType.Pistol or WeaponType.Rifle => [CharacterClass.Gunslinger],
            WeaponType.FlaskThrower => [CharacterClass.Alchemist],
            WeaponType.Lute or WeaponType.Harp or WeaponType.Flute or WeaponType.Guitar=> [CharacterClass.Bard],
            WeaponType.Shield => [CharacterClass.Paladin, CharacterClass.Warden, CharacterClass.Cleric],
            WeaponType.Orb or WeaponType.Tome => [CharacterClass.Wizard, CharacterClass.Sorcerer, CharacterClass.Necromancer],
            WeaponType.Talisman => [CharacterClass.Druid, CharacterClass.Cleric],
            WeaponType.Symbol => [CharacterClass.Wizard, CharacterClass.Cleric],
            _ => []
        };
    }

    /// <summary>
    /// Checks if the weapon requires ammunition.
    /// </summary>
    public static bool RequiresAmmunition(this WeaponType weaponType)
    {
        return weaponType is WeaponType.Bow or WeaponType.Crossbow
            or WeaponType.Pistol or WeaponType.Rifle
            or WeaponType.FlaskThrower;
    }

    /// <summary>
    /// Gets the ammunition type name for weapons that require it.
    /// </summary>
    public static string? GetAmmunitionType(this WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Bow or WeaponType.Crossbow => "Arrows/Bolts",
            WeaponType.Pistol or WeaponType.Rifle => "Bullets",
            WeaponType.FlaskThrower => "Grenades/Flasks",
            _ => null
        };
    }
}
