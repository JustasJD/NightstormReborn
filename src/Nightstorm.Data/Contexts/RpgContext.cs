using Microsoft.EntityFrameworkCore;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Contexts;

/// <summary>
/// Main database context for the RPG game.
/// </summary>
public class RpgContext : DbContext
{
    public RpgContext(DbContextOptions<RpgContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Users table.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Characters table.
    /// </summary>
    public DbSet<Character> Characters { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Items table.
    /// </summary>
    public DbSet<Item> Items { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Guilds table.
    /// </summary>
    public DbSet<Guild> Guilds { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Quests table.
    /// </summary>
    public DbSet<Quest> Quests { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Monsters table.
    /// </summary>
    public DbSet<Monster> Monsters { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CharacterItems table (inventory).
    /// </summary>
    public DbSet<CharacterItem> CharacterItems { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CharacterQuests table.
    /// </summary>
    public DbSet<CharacterQuest> CharacterQuests { get; set; } = null!;

    /// <summary>
    /// Gets or sets the PlayerStates table.
    /// Tracks current player location and status.
    /// </summary>
    public DbSet<PlayerState> PlayerStates { get; set; } = null!;

    /// <summary>
    /// Gets or sets the NightstormEvents table.
    /// Scheduled PvE combat events.
    /// </summary>
    public DbSet<NightstormEvent> NightstormEvents { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CombatInstances table.
    /// Active and completed combat sessions.
    /// </summary>
    public DbSet<CombatInstance> CombatInstances { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CombatParticipants table.
    /// Players and monsters in combat.
    /// </summary>
    public DbSet<CombatParticipant> CombatParticipants { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CombatLogEntries table.
    /// Combat action history.
    /// </summary>
    public DbSet<CombatLogEntry> CombatLogEntries { get; set; } = null!;

    /// <summary>
    /// Gets or sets the TravelLogs table.
    /// Player travel history.
    /// </summary>
    public DbSet<TravelLog> TravelLogs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ZoneTreasuries table.
    /// Gold accumulated in each zone.
    /// </summary>
    public DbSet<ZoneTreasury> ZoneTreasuries { get; set; } = null!;

    /// <summary>
    /// Gets or sets the TreasuryTransactions table.
    /// Treasury deposit and withdrawal history.
    /// </summary>
    public DbSet<TreasuryTransaction> TreasuryTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RpgContext).Assembly);

        // Global query filter for soft deletes (existing entities)
        modelBuilder.Entity<Character>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Item>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Guild>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Quest>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Monster>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CharacterItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CharacterQuest>().HasQueryFilter(e => !e.IsDeleted);

        // Global query filter for soft deletes (game engine entities)
        modelBuilder.Entity<PlayerState>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<NightstormEvent>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CombatInstance>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CombatParticipant>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CombatLogEntry>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TravelLog>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ZoneTreasury>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TreasuryTransaction>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
