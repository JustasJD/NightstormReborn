using Nightstorm.Core.Entities;

namespace Nightstorm.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Character entities.
/// </summary>
public interface ICharacterRepository : IRepository<Character>
{
    /// <summary>
    /// Gets a character by Discord user ID.
    /// </summary>
    /// <param name="discordUserId">The Discord user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character, or null if not found.</returns>
    Task<Character?> GetByDiscordUserIdAsync(ulong discordUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character with their inventory included.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character with inventory, or null if not found.</returns>
    Task<Character?> GetWithInventoryAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character with their quests included.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character with quests, or null if not found.</returns>
    Task<Character?> GetWithQuestsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character with their guild included.
    /// </summary>
    /// <param name="id">The character ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The character with guild, or null if not found.</returns>
    Task<Character?> GetWithGuildAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all characters in a guild.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of characters in the guild.</returns>
    Task<IEnumerable<Character>> GetByGuildIdAsync(Guid guildId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top characters by level for leaderboard.
    /// </summary>
    /// <param name="count">Number of characters to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of top characters.</returns>
    Task<IEnumerable<Character>> GetTopByLevelAsync(int count, CancellationToken cancellationToken = default);
}
