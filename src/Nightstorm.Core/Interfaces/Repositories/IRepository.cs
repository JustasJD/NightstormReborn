using System.Linq.Expressions;

namespace Nightstorm.Core.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for common data operations.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity, or null if not found.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter expression.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching entities.</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter expression.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity, or null if not found.</returns>
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Updates multiple entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);

    /// <summary>
    /// Removes multiple entities.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Checks if any entity matches the predicate.
    /// </summary>
    /// <param name="predicate">Filter expression.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any entity matches, false otherwise.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate.
    /// </summary>
    /// <param name="predicate">Filter expression.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Count of matching entities.</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
