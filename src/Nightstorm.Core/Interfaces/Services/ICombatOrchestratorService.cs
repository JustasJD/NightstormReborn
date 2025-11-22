namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for orchestrating parallel combat turn processing.
/// Prevents sequential bottlenecks when multiple combats are active simultaneously.
/// </summary>
public interface ICombatOrchestratorService
{
    /// <summary>
    /// Processes all active combat turns in parallel with throttling.
    /// Ensures server resources are not overwhelmed while maximizing throughput.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of combats processed</returns>
    Task<int> ProcessActiveCombatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a single combat turn.
    /// Called internally by parallel processing orchestrator.
    /// </summary>
    /// <param name="combatId">The combat instance ID to process</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ProcessCombatTurnAsync(Guid combatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current number of active combats being processed.
    /// </summary>
    int ActiveCombatCount { get; }

    /// <summary>
    /// Gets the maximum number of concurrent combat processes allowed.
    /// </summary>
    int MaxConcurrency { get; }
}
