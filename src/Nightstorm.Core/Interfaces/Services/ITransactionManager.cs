namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for managing database transactions with automatic rollback and retry logic.
/// Ensures atomic operations across multiple repository calls.
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// Executes an action within a database transaction.
    /// Automatically commits on success, rolls back on failure.
    /// </summary>
    /// <typeparam name="T">Return type of the action</typeparam>
    /// <param name="action">Action to execute within transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the action</returns>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a database transaction (no return value).
    /// Automatically commits on success, rolls back on failure.
    /// </summary>
    /// <param name="action">Action to execute within transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExecuteInTransactionAsync(
        Func<Task> action, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction with automatic retry on transient failures.
    /// </summary>
    /// <typeparam name="T">Return type of the action</typeparam>
    /// <param name="action">Action to execute</param>
    /// <param name="maxRetries">Maximum number of retry attempts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the action</returns>
    Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> action,
        int maxRetries = 3,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current execution context is within a transaction.
    /// </summary>
    bool IsInTransaction { get; }
}
