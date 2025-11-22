using Microsoft.Extensions.Logging;
using Nightstorm.Core.Exceptions;
using Nightstorm.Core.Interfaces.Services;
using StackExchange.Redis;

namespace Nightstorm.Data.Services;

/// <summary>
/// Redis-based distributed locking service using the RedLock algorithm.
/// Prevents race conditions in critical sections across multiple processes.
/// </summary>
public class RedisDistributedLockService : IDistributedLockService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisDistributedLockService> _logger;

    public RedisDistributedLockService(
        IConnectionMultiplexer redis,
        ILogger<RedisDistributedLockService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Attempts to acquire a distributed lock with the specified key.
    /// Returns null if the lock cannot be acquired within a reasonable time.
    /// </summary>
    public async Task<IDistributedLock?> AcquireLockAsync(
        string key,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Lock key cannot be null or empty", nameof(key));

        if (timeout <= TimeSpan.Zero)
            throw new ArgumentException("Timeout must be positive", nameof(timeout));

        var lockKey = $"lock:{key}";
        var lockValue = Guid.NewGuid().ToString("N"); // Unique value to identify this lock holder
        var database = _redis.GetDatabase();

        try
        {
            _logger.LogDebug(
                "Attempting to acquire lock: Key={LockKey}, Timeout={Timeout}ms",
                lockKey, timeout.TotalMilliseconds);

            // SET NX (set if not exists) with expiration
            // This is atomic - either sets the key and returns true, or returns false
            var acquired = await database.StringSetAsync(
                lockKey,
                lockValue,
                timeout,
                When.NotExists);

            if (!acquired)
            {
                _logger.LogWarning(
                    "Failed to acquire lock (already held by another process): Key={LockKey}",
                    lockKey);
                return null;
            }

            _logger.LogInformation(
                "Successfully acquired distributed lock: Key={LockKey}, Timeout={Timeout}ms",
                lockKey, timeout.TotalMilliseconds);

            return new RedisDistributedLock(database, lockKey, lockValue, timeout, _logger);
        }
        catch (RedisException ex)
        {
            _logger.LogError(
                ex,
                "Redis error while acquiring lock: Key={LockKey}",
                lockKey);

            throw new DistributedLockException(
                $"Failed to acquire lock '{lockKey}' due to Redis error",
                lockKey,
                LockFailureReason.ConnectionFailure,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while acquiring lock: Key={LockKey}",
                lockKey);

            throw new DistributedLockException(
                $"Failed to acquire lock '{lockKey}' due to unexpected error",
                lockKey,
                LockFailureReason.Unknown,
                ex);
        }
    }

    /// <summary>
    /// Attempts to acquire a lock with retry logic.
    /// Useful for handling transient failures or contention.
    /// </summary>
    public async Task<IDistributedLock?> AcquireLockWithRetryAsync(
        string key,
        TimeSpan timeout,
        int retryAttempts = 3,
        TimeSpan? retryDelay = null,
        CancellationToken cancellationToken = default)
    {
        if (retryAttempts < 1)
            throw new ArgumentException("Retry attempts must be at least 1", nameof(retryAttempts));

        var delay = retryDelay ?? TimeSpan.FromMilliseconds(100);

        for (int attempt = 1; attempt <= retryAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug(
                "Lock acquisition attempt {Attempt} of {MaxAttempts}: Key={LockKey}",
                attempt, retryAttempts, key);

            var lockHandle = await AcquireLockAsync(key, timeout, cancellationToken);

            if (lockHandle != null)
            {
                return lockHandle;
            }

            // If this wasn't the last attempt, wait before retrying
            if (attempt < retryAttempts)
            {
                _logger.LogDebug(
                    "Lock acquisition failed, waiting {Delay}ms before retry {NextAttempt}",
                    delay.TotalMilliseconds, attempt + 1);

                await Task.Delay(delay, cancellationToken);

                // Exponential backoff for subsequent retries
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
        }

        _logger.LogWarning(
            "Failed to acquire lock after {Attempts} attempts: Key={LockKey}",
            retryAttempts, key);

        return null;
    }
}
