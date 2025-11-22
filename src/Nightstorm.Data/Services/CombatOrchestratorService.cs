using Microsoft.Extensions.Logging;
using Nightstorm.Core.Interfaces.Services;

namespace Nightstorm.Data.Services;

/// <summary>
/// Orchestrates parallel combat turn processing with throttling.
/// Prevents sequential bottlenecks by processing multiple combats simultaneously.
/// </summary>
public class CombatOrchestratorService : ICombatOrchestratorService
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<CombatOrchestratorService> _logger;
    private int _activeCombatCount;

    /// <summary>
    /// Maximum number of combats that can be processed concurrently.
    /// Default: 10 (adjustable based on server resources)
    /// </summary>
    public int MaxConcurrency { get; }

    /// <summary>
    /// Current number of combats being processed.
    /// </summary>
    public int ActiveCombatCount => _activeCombatCount;

    public CombatOrchestratorService(
        ILogger<CombatOrchestratorService> logger,
        int maxConcurrency = 10)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        if (maxConcurrency < 1)
            throw new ArgumentException("Max concurrency must be at least 1", nameof(maxConcurrency));

        MaxConcurrency = maxConcurrency;
        _semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

        _logger.LogInformation(
            "CombatOrchestratorService initialized with max concurrency: {MaxConcurrency}",
            maxConcurrency);
    }

    /// <summary>
    /// Processes all active combats in parallel with throttling.
    /// </summary>
    public async Task<int> ProcessActiveCombatsAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Get active combat IDs from database/repository
        // For now, this is a placeholder that will be implemented when combat entities are created
        var activeCombatIds = await GetActiveCombatIdsAsync(cancellationToken);

        if (activeCombatIds.Count == 0)
        {
            _logger.LogDebug("No active combats to process");
            return 0;
        }

        _logger.LogInformation(
            "Processing {CombatCount} active combats with max concurrency {MaxConcurrency}",
            activeCombatIds.Count,
            MaxConcurrency);

        var startTime = DateTime.UtcNow;

        try
        {
            // Process all combats in parallel with throttling
            var tasks = activeCombatIds.Select(combatId =>
                ProcessCombatWithThrottlingAsync(combatId, cancellationToken));

            await Task.WhenAll(tasks);

            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogInformation(
                "Processed {CombatCount} combats in {Duration}ms (avg {AvgDuration}ms per combat)",
                activeCombatIds.Count,
                duration.TotalMilliseconds,
                duration.TotalMilliseconds / activeCombatIds.Count);

            return activeCombatIds.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing active combats. Processed {CompletedCount} of {TotalCount}",
                activeCombatIds.Count - _activeCombatCount,
                activeCombatIds.Count);
            
            throw;
        }
    }

    /// <summary>
    /// Processes a single combat turn (placeholder for actual implementation).
    /// </summary>
    public async Task ProcessCombatTurnAsync(Guid combatId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Processing combat turn: {CombatId}", combatId);

        // TODO: Implement actual combat turn processing
        // This will be implemented when combat entities and combat service are created
        // For now, simulate processing time
        await Task.Delay(100, cancellationToken); // Simulate database operations

        _logger.LogDebug("Combat turn processed: {CombatId}", combatId);
    }

    /// <summary>
    /// Processes a combat turn with semaphore throttling.
    /// Ensures max concurrent operations are not exceeded.
    /// </summary>
    private async Task ProcessCombatWithThrottlingAsync(
        Guid combatId,
        CancellationToken cancellationToken)
    {
        // Wait for semaphore slot (throttle concurrent operations)
        await _semaphore.WaitAsync(cancellationToken);

        Interlocked.Increment(ref _activeCombatCount);

        try
        {
            _logger.LogDebug(
                "Combat {CombatId} acquired processing slot ({ActiveCount}/{MaxConcurrency})",
                combatId,
                _activeCombatCount,
                MaxConcurrency);

            await ProcessCombatTurnAsync(combatId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing combat {CombatId}",
                combatId);
            
            // Don't rethrow - continue processing other combats
        }
        finally
        {
            Interlocked.Decrement(ref _activeCombatCount);
            _semaphore.Release();

            _logger.LogDebug(
                "Combat {CombatId} released processing slot ({ActiveCount}/{MaxConcurrency})",
                combatId,
                _activeCombatCount,
                MaxConcurrency);
        }
    }

    /// <summary>
    /// Gets list of active combat IDs from database.
    /// Placeholder - will be implemented when combat entities are created.
    /// </summary>
    private async Task<List<Guid>> GetActiveCombatIdsAsync(CancellationToken cancellationToken)
    {
        // TODO: Query database for active combats
        // SELECT Id FROM CombatInstances WHERE Status = InProgress
        
        // For now, return empty list
        await Task.CompletedTask;
        return new List<Guid>();
    }
}
