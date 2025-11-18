# Nightstorm.Tests

Unit tests for the NightstormReborn game using **NUnit** and **FluentAssertions**.

## Test Coverage

### CharacterStatsService Tests

Comprehensive test suite for `CharacterStatsService` covering all attack power calculations:

#### Melee Attack Power (MAP) Tests
- ? Balanced stats calculation
- ? Class-specific stat calculations (Paladin, Duelist)
- ? Strength vs Dexterity weighting verification
- ? Zero stats edge case
- ? Parametrized tests for various stat combinations

#### Ranged Attack Power (RAP) Tests
- ? Balanced stats calculation
- ? Class-specific stat calculations (Duelist, Gunslinger)
- ? Dexterity vs Strength weighting verification
- ? Zero stats edge case
- ? Parametrized tests for various stat combinations

#### Magic Power Tests
**Wisdom-based Classes:**
- ? Paladin (WIS-based)
- ? Warden (WIS-based)
- ? Cleric (WIS-based)
- ? Druid (WIS-based)
- ? Ranger (WIS-based)

**Intelligence-based Classes:**
- ? Wizard (INT-based)
- ? Sorcerer (INT-based)
- ? Necromancer (INT-based)
- ? DarkKnight (INT-based)
- ? Bard (INT-based)
- ? Alchemist (INT-based)

**Edge Cases:**
- ? Zero stats handling
- ? Wisdom classes ignore Intelligence
- ? Intelligence classes ignore Wisdom

#### Comparative Tests
- ? Duelist ranged vs melee comparison
- ? Paladin melee vs ranged comparison

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests with Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test
```bash
dotnet test --filter "FullyQualifiedName~CharacterStatsServiceTests"
```

### Run Tests for Specific Method
```bash
dotnet test --filter "FullyQualifiedName~CalculateMeleeAttackPower"
```

## Test Statistics

- **Total Tests**: 36
- **Test Categories**: 
  - Melee Attack Power: 7 tests
  - Ranged Attack Power: 7 tests
  - Magic Power (Wisdom-based): 5 tests
  - Magic Power (Intelligence-based): 6 tests
  - Magic Power (Edge Cases): 3 tests
  - Attack Power Comparisons: 2 tests
- **All Tests Passing**: ?

## Technologies Used

- **NUnit 4.2.2** - Testing framework
- **FluentAssertions 8.8.0** - Readable assertions
- **Microsoft.NET.Test.Sdk 17.12.0** - Test SDK
- **NUnit3TestAdapter 4.6.0** - Test adapter
- **coverlet.collector 6.0.2** - Code coverage

## Test Structure

Tests follow the **AAA pattern** (Arrange-Act-Assert):

```csharp
[Test]
public void CalculateMeleeAttackPower_WithPaladinStats_ReturnsCorrectValue()
{
    // Arrange - Set up test data
    var strength = 14;
    var dexterity = 10;
    var expected = 86;

    // Act - Execute the method
    var result = _sut.CalculateMeleeAttackPower(strength, dexterity);

    // Assert - Verify the result
    result.Should().Be(expected);
}
```

## Best Practices

1. **Descriptive Test Names**: Test names clearly describe what they test
2. **One Assert Per Test**: Each test verifies a single behavior (with exceptions for related assertions)
3. **Test Isolation**: Each test is independent and can run in any order
4. **FluentAssertions**: Uses readable, natural language assertions
5. **Parametrized Tests**: Uses `[TestCase]` for testing multiple scenarios
6. **Edge Case Coverage**: Tests zero values, boundary conditions, and class-specific behavior

## Adding New Tests

When adding new functionality to `CharacterStatsService`:

1. Create a new test method with descriptive name
2. Follow AAA pattern
3. Use FluentAssertions for assertions
4. Add test to appropriate region (#region)
5. Run tests to verify they pass

Example:
```csharp
[Test]
public void NewMethod_WithSpecificCondition_ReturnsExpectedBehavior()
{
    // Arrange
    var input = /* test data */;

    // Act
    var result = _sut.NewMethod(input);

    // Assert
    result.Should().Be(expectedValue);
}
```

## Continuous Integration

These tests should be run:
- On every commit (pre-commit hook)
- On every pull request
- Before deployment
- As part of CI/CD pipeline

## Test Coverage Goals

- **Line Coverage**: Aim for 90%+ on service layer
- **Branch Coverage**: All conditional logic paths tested
- **Edge Cases**: All boundary conditions covered

## Future Test Plans

- [ ] Add tests for `CalculateMaxHealth()`
- [ ] Add tests for `CalculateMaxMana()`
- [ ] Add tests for `IsPhysicalArchetype()`
- [ ] Add integration tests with actual Character entities
- [ ] Add performance/benchmark tests for calculation methods
- [ ] Add tests for CharacterExtensions methods
