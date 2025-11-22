using Nightstorm.Core.Enums;

namespace Nightstorm.API.DTOs.Characters;

/// <summary>
/// Request DTO for creating a new character.
/// UserId is automatically extracted from JWT token.
/// </summary>
public record CreateCharacterRequest
{
    /// <summary>
    /// Character name (2-80 characters).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Character class.
    /// </summary>
    public required CharacterClass Class { get; init; }
}

/// <summary>
/// Request DTO for updating a character.
/// </summary>
public record UpdateCharacterRequest
{
    /// <summary>
    /// Optional name change (2-80 characters).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Optional guild ID change.
    /// </summary>
    public Guid? GuildId { get; init; }
}

/// <summary>
/// Response DTO for character basic information.
/// Lightweight for list operations.
/// </summary>
public record CharacterResponse
{
    public required Guid Id { get; init; }
    public required ulong DiscordUserId { get; init; }
    public required string Name { get; init; }
    public required CharacterClass Class { get; init; }
    public required int Level { get; init; }
    public required long Experience { get; init; }
    public required int CurrentHealth { get; init; }
    public required int MaxHealth { get; init; }
    public required int CurrentMana { get; init; }
    public required int MaxMana { get; init; }
    public required long Gold { get; init; }
    public Guid? GuildId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Response DTO for character with full stats.
/// </summary>
public record CharacterDetailResponse : CharacterResponse
{
    public required int Strength { get; init; }
    public required int Dexterity { get; init; }
    public required int Constitution { get; init; }
    public required int Intelligence { get; init; }
    public required int Wisdom { get; init; }
    public required int Spirit { get; init; }
    public required int Luck { get; init; }
}

/// <summary>
/// Paginated response wrapper.
/// </summary>
/// <typeparam name="T">Type of items in the page.</typeparam>
public record PagedResponse<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
    public required bool HasPreviousPage { get; init; }
    public required bool HasNextPage { get; init; }
}
