using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Nightstorm.Data.Services;

/// <summary>
/// Represents a held distributed lock backed by Redis.
/// Automatically releases the lock when disposed.
/// </summary>
internal sealed class RedisDistributedLock : Nightstorm.Core.Interfaces.Services.IDistributedLock
{
    private readonly IDatabase _database;
    private readonly string _key;
    private readonly string _value;
    private readonly ILogger _logger;
    private bool _disposed;

    public string Key { get; }
    public DateTime AcquiredAt { get; }
    public TimeSpan Duration { get; }
    
    public bool IsValid => !_disposed && DateTime.UtcNow < AcquiredAt.Add(Duration);

    public RedisDistributedLock(
        IDatabase database,
        string key,
        string value,
        TimeSpan duration,
        ILogger logger)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _key = key ?? throw new ArgumentNullException(nameof(key));
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Key = key;
        AcquiredAt = DateTime.UtcNow;
        Duration = duration;

        _logger.LogDebug(
            "Distributed lock acquired: Key={LockKey}, Value={LockValue}, Duration={Duration}ms",
            key, value, duration.TotalMilliseconds);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        try
        {
            // Lua script ensures we only delete the lock if we still own it
            // This prevents accidentally deleting someone else's lock
            const string luaScript = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

            var result = await _database.ScriptEvaluateAsync(
                luaScript,
                new RedisKey[] { _key },
                new RedisValue[] { _value });

            if ((int)result == 1)
            {
                _logger.LogDebug(
                    "Distributed lock released successfully: Key={LockKey}",
                    _key);
            }
            else
            {
                _logger.LogWarning(
                    "Distributed lock was already released or expired: Key={LockKey}",
                    _key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to release distributed lock: Key={LockKey}",
                _key);
            // Don't throw - disposal should be safe
        }
        finally
        {
            _disposed = true;
        }
    }
}
