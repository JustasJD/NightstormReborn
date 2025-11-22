using Nightstorm.Core.Entities;

namespace Nightstorm.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Character entities.
/// Optimized for high concurrency scenarios (100s of calls per second).
/// </summary>
public interface ICharacterRepository : IRepository<Character>
{
    /// <summary>
    /// Gets a character by Discord user ID.
    /// Uses AsNoTracking for read-only performance.
    /// </summary>
    /// <param name="discordUserId">The Discord user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character, or null if not found.</returns>
    Task<Character?> GetByDiscordUserIdAsync(ulong discordUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character with their inventory included.
    /// Single query optimization with Include.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character with inventory, or null if not found.</returns>
    Task<Character?> GetWithInventoryAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character with their quests included.
    /// Single query optimization with Include.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character with quests, or null if not found.</returns>
    Task<Character?> GetWithQuestsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character with their guild included.
    /// Single query optimization with Include.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character with guild, or null if not found.</returns>
    Task<Character?> GetWithGuildAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all characters in a guild with pagination.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of characters and total count.</returns>
    Task<(IEnumerable<Character> Characters, int TotalCount)> GetByGuildIdAsync(
        Guid guildId, 
        int pageNumber = 1, 
        int pageSize = 50, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top characters by level for leaderboard.
    /// Uses AsNoTracking for read-only performance.
    /// </summary>
    /// <param name="count">Number of characters to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of top characters.</returns>
    Task<IEnumerable<Character>> GetTopByLevelAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a character name is already taken.
    /// Ultra-fast existence check without loading entity.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <param name="excludeCharacterId">Character ID to exclude (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if name exists.</returns>
    Task<bool> IsNameTakenAsync(string name, Guid? excludeCharacterId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates character health/mana/experience without loading full entity.
    /// Optimized for frequent combat updates.
    /// </summary>
    /// <param name="characterId">The character ID.</param>
    /// <param name="healthChange">Health change (+ or -).</param>
    /// <param name="manaChange">Mana change (+ or -).</param>
    /// <param name="experienceGain">Experience gained.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> UpdateStatsAsync(
        Guid characterId, 
        int? healthChange = null, 
        int? manaChange = null, 
        long? experienceGain = null, 
        CancellationToken cancellationToken = default);
}
