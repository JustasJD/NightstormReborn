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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RpgContext).Assembly);

        // Global query filter for soft deletes
        modelBuilder.Entity<Character>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Item>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Guild>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Quest>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Monster>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CharacterItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CharacterQuest>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
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
                    // Implement soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
