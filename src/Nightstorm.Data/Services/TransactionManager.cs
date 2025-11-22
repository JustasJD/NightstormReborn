using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Nightstorm.Core.Interfaces.Services;
using Nightstorm.Data.Contexts;

namespace Nightstorm.Data.Services;

/// <summary>
/// Manages database transactions with automatic rollback and retry logic.
/// Ensures atomic operations across multiple repository calls.
/// </summary>
public class TransactionManager : ITransactionManager
{
    private readonly RpgContext _context;
    private readonly ILogger<TransactionManager> _logger;
    private IDbContextTransaction? _currentTransaction;

    public TransactionManager(
        RpgContext context,
        ILogger<TransactionManager> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Checks if currently within a transaction.
    /// </summary>
    public bool IsInTransaction => _currentTransaction != null;

    /// <summary>
    /// Executes an action within a database transaction.
    /// Automatically commits on success, rolls back on failure.
    /// </summary>
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        // If already in transaction, just execute action
        if (IsInTransaction)
        {
            _logger.LogDebug("Already in transaction, executing action directly");
            return await action();
        }

        _logger.LogDebug("Starting new transaction");

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _currentTransaction = transaction;

        try
        {
            var result = await action();

            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogDebug("Transaction committed successfully");
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed, rolling back");
            
            await transaction.RollbackAsync(cancellationToken);
            
            throw;
        }
        finally
        {
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Executes an action within a database transaction (no return value).
    /// Automatically commits on success, rolls back on failure.
    /// </summary>
    public async Task ExecuteInTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await action();
            return true;
        }, cancellationToken);
    }

    /// <summary>
    /// Executes an action within a transaction with automatic retry on transient failures.
    /// </summary>
    public async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> action,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (maxRetries < 1)
            throw new ArgumentException("Max retries must be at least 1", nameof(maxRetries));

        int retryCount = 0;
        Exception? lastException = null;

        while (retryCount < maxRetries)
        {
            try
            {
                return await ExecuteInTransactionAsync(action, cancellationToken);
            }
            catch (DbUpdateException ex) when (IsTransientError(ex) && retryCount < maxRetries - 1)
            {
                retryCount++;
                lastException = ex;

                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 100);
                
                _logger.LogWarning(
                    ex,
                    "Transient error in transaction, retry {RetryCount} of {MaxRetries} after {Delay}ms",
                    retryCount,
                    maxRetries,
                    delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Non-transient error in transaction, not retrying");
                throw;
            }
        }

        _logger.LogError(
            lastException,
            "Transaction failed after {MaxRetries} retries",
            maxRetries);

        throw lastException!;
    }

    /// <summary>
    /// Determines if an exception is a transient database error that can be retried.
    /// </summary>
    private bool IsTransientError(Exception ex)
    {
        // Check for common transient error patterns
        var message = ex.Message.ToLowerInvariant();
        
        return message.Contains("timeout") ||
               message.Contains("deadlock") ||
               message.Contains("connection") ||
               message.Contains("network") ||
               ex is DbUpdateConcurrencyException;
    }
}
