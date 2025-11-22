using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Exceptions;
using Nightstorm.Core.Interfaces.Repositories;
using Nightstorm.Data.Contexts;

namespace Nightstorm.Data.Repositories;

/// <summary>
/// Generic repository implementation for common data operations.
/// Includes optimistic concurrency control with automatic retry logic.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly RpgContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T>> _logger;

    public Repository(RpgContext context, ILogger<Repository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// Updates an entity with automatic concurrency conflict retry.
    /// Implements optimistic concurrency control with exponential backoff.
    /// </summary>
    public virtual async Task<T> UpdateAsync(T entity, int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        int retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogDebug(
                    "Successfully updated {EntityType} with ID {EntityId} on attempt {Attempt}",
                    typeof(T).Name,
                    GetEntityId(entity),
                    retryCount + 1);
                
                return entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;
                
                _logger.LogWarning(
                    "Concurrency conflict on {EntityType} with ID {EntityId}. Attempt {Attempt} of {MaxAttempts}",
                    typeof(T).Name,
                    GetEntityId(entity),
                    retryCount,
                    maxRetries);
                
                if (retryCount >= maxRetries)
                {
                    _logger.LogError(
                        ex,
                        "Failed to update {EntityType} after {MaxRetries} retries due to concurrency conflicts",
                        typeof(T).Name,
                        maxRetries);
                    
                    throw new ConcurrencyException(
                        $"Failed to update {typeof(T).Name} after {maxRetries} retries due to concurrent modifications",
                        typeof(T),
                        GetEntityId(entity),
                        retryCount,
                        ex);
                }
                
                // Reload entity from database with fresh data
                var entry = ex.Entries.Single();
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                
                if (databaseValues == null)
                {
                    // Entity was deleted by another process
                    _logger.LogWarning(
                        "Entity {EntityType} with ID {EntityId} was deleted by another process",
                        typeof(T).Name,
                        GetEntityId(entity));
                    
                    throw new ConcurrencyException(
                        $"{typeof(T).Name} was deleted by another process",
                        typeof(T),
                        GetEntityId(entity),
                        retryCount,
                        ex);
                }
                
                // Update original values to current database values
                entry.OriginalValues.SetValues(databaseValues);
                
                // Exponential backoff before retry
                if (retryCount < maxRetries)
                {
                    var delay = TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 100);
                    _logger.LogDebug(
                        "Waiting {DelayMs}ms before retry attempt {Attempt}",
                        delay.TotalMilliseconds,
                        retryCount + 1);
                    
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
        
        // Should never reach here
        throw new InvalidOperationException(
            $"UpdateAsync loop exited unexpectedly for {typeof(T).Name}");
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the entity ID for logging purposes.
    /// Assumes entity inherits from BaseEntity.
    /// </summary>
    private Guid GetEntityId(T entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            return baseEntity.Id;
        }
        
        // Fallback: try to get Id property via reflection
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            return (Guid)(idProperty.GetValue(entity) ?? Guid.Empty);
        }
        
        return Guid.Empty;
    }
}
