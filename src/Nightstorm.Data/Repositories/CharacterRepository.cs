using Microsoft.EntityFrameworkCore;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Interfaces.Repositories;
using Nightstorm.Data.Contexts;

namespace Nightstorm.Data.Repositories;

/// <summary>
/// Repository implementation for Character entities.
/// </summary>
public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(RpgContext context) : base(context)
    {
    }

    public async Task<Character?> GetByDiscordUserIdAsync(ulong discordUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.DiscordUserId == discordUserId, cancellationToken);
    }

    public async Task<Character?> GetWithInventoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Inventory)
                .ThenInclude(ci => ci.Item)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Character?> GetWithQuestsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Quests)
                .ThenInclude(cq => cq.Quest)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Character?> GetWithGuildAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Guild)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Character>> GetByGuildIdAsync(Guid guildId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.GuildId == guildId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Character>> GetTopByLevelAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(c => c.Level)
            .ThenByDescending(c => c.Experience)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
