namespace Nightstorm.Core.Exceptions;

/// <summary>
/// Exception thrown when a concurrency conflict occurs during entity update.
/// This typically happens when two processes try to update the same entity simultaneously.
/// </summary>
public class ConcurrencyException : Exception
{
    /// <summary>
    /// Gets the entity type that caused the concurrency conflict.
    /// </summary>
    public Type? EntityType { get; }

    /// <summary>
    /// Gets the ID of the entity that caused the concurrency conflict.
    /// </summary>
    public Guid? EntityId { get; }

    /// <summary>
    /// Gets the number of retry attempts made before throwing this exception.
    /// </summary>
    public int RetryAttempts { get; }

    public ConcurrencyException(string message) : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public ConcurrencyException(
        string message, 
        Type entityType, 
        Guid entityId, 
        int retryAttempts,
        Exception innerException) 
        : base(message, innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
        RetryAttempts = retryAttempts;
    }
}
