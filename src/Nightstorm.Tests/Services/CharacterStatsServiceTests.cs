using FluentAssertions;
using Nightstorm.Core.Enums;
using Nightstorm.Core.Services;

namespace Nightstorm.Tests.Services;

/// <summary>
/// Unit tests for CharacterStatsService - Attack Power calculations.
/// Tests cover MAP (Melee Attack Power), RAP (Ranged Attack Power), and Magic Power calculations.
/// </summary>
[TestFixture]
public class CharacterStatsServiceTests
{
    private CharacterStatsService _sut = null!;

    [SetUp]
    public void Setup()
    {
        _sut = new CharacterStatsService();
    }

    #region Melee Attack Power Tests

    [Test]
    public void CalculateMeleeAttackPower_WithBalancedStats_ReturnsCorrectValue()
    {
        // Arrange
        var strength = 10;
        var dexterity = 10;
        var expected = (10 * 4) + (10 * 2) + 10; // 40 + 20 + 10 = 70

        // Act
        var result = _sut.CalculateMeleeAttackPower(strength, dexterity);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void CalculateMeleeAttackPower_WithZeroStats_ReturnsBaseValue()
    {
        // Arrange
        var strength = 0;
        var dexterity = 0;

        // Act
        var result = _sut.CalculateMeleeAttackPower(strength, dexterity);

        // Assert - Should only return the base value of 10
        result.Should().Be(10);
    }

    [TestCase(14, 10, 86)]
    [TestCase(14, 12, 90)]
    [TestCase(14, 14, 94)]
    [TestCase(8, 16, 74)]   // (8*4) + (16*2) + 10 = 32 + 32 + 10 = 74
    [TestCase(12, 16, 90)]  // (12*4) + (16*2) + 10 = 48 + 32 + 10 = 90
    public void CalculateMeleeAttackPower_WithVariousStats_ReturnsExpectedValues(
        int strength, int dexterity, int expected)
    {
        // Act
        var result = _sut.CalculateMeleeAttackPower(strength, dexterity);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Ranged Attack Power Tests

    [Test]
    public void CalculateRangedAttackPower_WithBalancedStats_ReturnsCorrectValue()
    {
        // Arrange
        var dexterity = 10;
        var strength = 10;
        var expected = (10 * 4) + (10 * 2) + 10; // 40 + 20 + 10 = 70

        // Act
        var result = _sut.CalculateRangedAttackPower(dexterity, strength);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void CalculateRangedAttackPower_WithZeroStats_ReturnsBaseValue()
    {
        // Arrange
        var dexterity = 0;
        var strength = 0;

        // Act
        var result = _sut.CalculateRangedAttackPower(dexterity, strength);

        // Assert - Should only return the base value of 10
        result.Should().Be(10);
    }

    [TestCase(16, 10, 94)]
    [TestCase(16, 12, 98)]
    [TestCase(16, 8, 90)]
    [TestCase(14, 8, 82)]
    [TestCase(12, 12, 82)]
    public void CalculateRangedAttackPower_WithVariousStats_ReturnsExpectedValues(
        int dexterity, int strength, int expected)
    {
        // Act
        var result = _sut.CalculateRangedAttackPower(dexterity, strength);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Magic Power Tests - Wisdom-based Classes

    [Test]
    public void CalculateMagicPower_ForPaladin_UsesWisdomAsPrimaryStat()
    {
        // Arrange - Paladin: WIS 12, SPR 12
        var characterClass = CharacterClass.Paladin;
        var intelligence = 8;
        var wisdom = 12;
        var spirit = 12;
        var expected = (12 * 4) + (12 * 2) + 10; // 48 + 24 + 10 = 82

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(82);
    }

    [Test]
    public void CalculateMagicPower_ForWarden_UsesWisdomAsPrimaryStat()
    {
        // Arrange - Warden: WIS 10, SPR 12
        var characterClass = CharacterClass.Warden;
        var intelligence = 8;
        var wisdom = 10;
        var spirit = 12;
        var expected = (10 * 4) + (12 * 2) + 10; // 40 + 24 + 10 = 74

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(74);
    }

    [Test]
    public void CalculateMagicPower_ForCleric_UsesWisdomAsPrimaryStat()
    {
        // Arrange - Cleric: WIS 16, SPR 14
        var characterClass = CharacterClass.Cleric;
        var intelligence = 8;
        var wisdom = 16;
        var spirit = 14;
        var expected = (16 * 4) + (14 * 2) + 10; // 64 + 28 + 10 = 102

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(102);
    }

    [Test]
    public void CalculateMagicPower_ForDruid_UsesWisdomAsPrimaryStat()
    {
        // Arrange - Druid: WIS 16, SPR 12
        var characterClass = CharacterClass.Druid;
        var intelligence = 10;
        var wisdom = 16;
        var spirit = 12;
        var expected = (16 * 4) + (12 * 2) + 10; // 64 + 24 + 10 = 98

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(98);
    }

    [Test]
    public void CalculateMagicPower_ForRanger_UsesWisdomAsPrimaryStat()
    {
        // Arrange - Ranger: WIS 12, SPR 10
        var characterClass = CharacterClass.Ranger;
        var intelligence = 8;
        var wisdom = 12;
        var spirit = 10;
        var expected = (12 * 4) + (10 * 2) + 10; // 48 + 20 + 10 = 78

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(78);
    }

    #endregion

    #region Magic Power Tests - Intelligence-based Classes

    [Test]
    public void CalculateMagicPower_ForWizard_UsesIntelligenceAsPrimaryStat()
    {
        // Arrange - Wizard: INT 18, SPR 14
        var characterClass = CharacterClass.Wizard;
        var intelligence = 18;
        var wisdom = 8;
        var spirit = 14;
        var expected = (18 * 4) + (14 * 2) + 10; // 72 + 28 + 10 = 110

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(110);
    }

    [Test]
    public void CalculateMagicPower_ForSorcerer_UsesIntelligenceAsPrimaryStat()
    {
        // Arrange - Sorcerer: INT 16, SPR 14
        var characterClass = CharacterClass.Sorcerer;
        var intelligence = 16;
        var wisdom = 8;
        var spirit = 14;
        var expected = (16 * 4) + (14 * 2) + 10; // 64 + 28 + 10 = 102

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(102);
    }

    [Test]
    public void CalculateMagicPower_ForNecromancer_UsesIntelligenceAsPrimaryStat()
    {
        // Arrange - Necromancer: INT 16, SPR 12
        var characterClass = CharacterClass.Necromancer;
        var intelligence = 16;
        var wisdom = 10;
        var spirit = 12;
        var expected = (16 * 4) + (12 * 2) + 10; // 64 + 24 + 10 = 98

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(98);
    }

    [Test]
    public void CalculateMagicPower_ForDarkKnight_UsesIntelligenceAsPrimaryStat()
    {
        // Arrange - DarkKnight: INT 8, SPR 12
        var characterClass = CharacterClass.DarkKnight;
        var intelligence = 8;
        var wisdom = 8;
        var spirit = 12;
        var expected = (8 * 4) + (12 * 2) + 10; // 32 + 24 + 10 = 66

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(66);
    }

    [Test]
    public void CalculateMagicPower_ForBard_UsesIntelligenceAsPrimaryStat()
    {
        // Arrange - Bard: INT 12, SPR 16
        var characterClass = CharacterClass.Bard;
        var intelligence = 12;
        var wisdom = 8;
        var spirit = 16;
        var expected = (12 * 4) + (16 * 2) + 10; // 48 + 32 + 10 = 90

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(90);
    }

    [Test]
    public void CalculateMagicPower_ForAlchemist_UsesIntelligenceAsPrimaryStat()
    {
        // Arrange - Alchemist: INT 16, SPR 10
        var characterClass = CharacterClass.Alchemist;
        var intelligence = 16;
        var wisdom = 8;
        var spirit = 10;
        var expected = (16 * 4) + (10 * 2) + 10; // 64 + 20 + 10 = 94

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert
        result.Should().Be(94);
    }

    #endregion

    #region Magic Power Tests - Edge Cases

    [Test]
    public void CalculateMagicPower_WithZeroStats_ReturnsBaseValue()
    {
        // Arrange
        var characterClass = CharacterClass.Wizard;
        var intelligence = 0;
        var wisdom = 0;
        var spirit = 0;

        // Act
        var result = _sut.CalculateMagicPower(characterClass, intelligence, wisdom, spirit);

        // Assert - Should only return the base value of 10
        result.Should().Be(10);
    }

    #endregion
}
