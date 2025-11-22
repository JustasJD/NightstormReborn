namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for acquiring distributed locks across multiple processes.
/// Uses Redis for coordination to prevent race conditions in critical sections.
/// </summary>
public interface IDistributedLockService
{
    /// <summary>
    /// Acquires a distributed lock with the specified key.
    /// Returns null if lock cannot be acquired within timeout.
    /// </summary>
    /// <param name="key">Unique key for the lock (e.g., "combat:registration:eventId")</param>
    /// <param name="timeout">How long to hold the lock before automatic release</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Lock handle if acquired, null if timeout</returns>
    Task<IDistributedLock?> AcquireLockAsync(
        string key, 
        TimeSpan timeout, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to acquire a lock with retry attempts.
    /// </summary>
    /// <param name="key">Unique key for the lock</param>
    /// <param name="timeout">How long to hold the lock</param>
    /// <param name="retryAttempts">Number of retry attempts</param>
    /// <param name="retryDelay">Delay between retries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Lock handle if acquired, null if all retries failed</returns>
    Task<IDistributedLock?> AcquireLockWithRetryAsync(
        string key,
        TimeSpan timeout,
        int retryAttempts = 3,
        TimeSpan? retryDelay = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a held distributed lock.
/// Dispose to release the lock.
/// </summary>
public interface IDistributedLock : IAsyncDisposable
{
    /// <summary>
    /// Gets the lock key.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets when the lock was acquired.
    /// </summary>
    DateTime AcquiredAt { get; }

    /// <summary>
    /// Gets how long the lock will be held.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Gets whether the lock is still valid (not expired).
    /// </summary>
    bool IsValid { get; }
}
