namespace Nightstorm.Core.Exceptions;

/// <summary>
/// Exception thrown when a distributed lock operation fails.
/// </summary>
public class DistributedLockException : Exception
{
    /// <summary>
    /// Gets the lock key that caused the exception.
    /// </summary>
    public string? LockKey { get; }

    /// <summary>
    /// Gets the reason for the lock failure.
    /// </summary>
    public LockFailureReason Reason { get; }

    public DistributedLockException(string message) : base(message)
    {
    }

    public DistributedLockException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public DistributedLockException(
        string message, 
        string lockKey, 
        LockFailureReason reason) 
        : base(message)
    {
        LockKey = lockKey;
        Reason = reason;
    }

    public DistributedLockException(
        string message, 
        string lockKey, 
        LockFailureReason reason,
        Exception innerException) 
        : base(message, innerException)
    {
        LockKey = lockKey;
        Reason = reason;
    }
}

/// <summary>
/// Reasons why a distributed lock operation might fail.
/// </summary>
public enum LockFailureReason
{
    /// <summary>
    /// Lock is already held by another process.
    /// </summary>
    AlreadyLocked,

    /// <summary>
    /// Timeout while trying to acquire lock.
    /// </summary>
    Timeout,

    /// <summary>
    /// Redis connection failed.
    /// </summary>
    ConnectionFailure,

    /// <summary>
    /// Lock expired before operation completed.
    /// </summary>
    LockExpired,

    /// <summary>
    /// Unknown error occurred.
    /// </summary>
    Unknown
}
