using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;

namespace Nightstorm.Data.Services;

/// <summary>
/// Background service that verifies all required jobs are scheduled on startup.
/// Reschedules missing jobs if server restarted or jobs were deleted.
/// </summary>
public class QuartzJobRecoveryService : BackgroundService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<QuartzJobRecoveryService> _logger;

    public QuartzJobRecoveryService(
        ISchedulerFactory schedulerFactory,
        ILogger<QuartzJobRecoveryService> logger)
    {
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Wait for scheduler to initialize
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            _logger.LogInformation("Starting Quartz job recovery check...");

            var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);

            if (!scheduler.IsStarted)
            {
                _logger.LogWarning("Scheduler not started yet, waiting...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            // Get all existing jobs
            var jobGroups = await scheduler.GetJobGroupNames(stoppingToken);
            var existingJobs = new List<JobKey>();

            foreach (var group in jobGroups)
            {
                var jobKeys = await scheduler.GetJobKeys(
                    GroupMatcher<JobKey>.GroupEquals(group),
                    stoppingToken);
                existingJobs.AddRange(jobKeys);
            }

            _logger.LogInformation(
                "Found {JobCount} existing jobs in scheduler",
                existingJobs.Count);

            // TODO: Verify required jobs exist
            // This will be implemented when game engine jobs are created
            // For now, just log the existing jobs
            foreach (var jobKey in existingJobs)
            {
                _logger.LogDebug(
                    "Existing job: {JobName} in group {JobGroup}",
                    jobKey.Name,
                    jobKey.Group);
            }

            await VerifyRequiredJobsAsync(scheduler, existingJobs, stoppingToken);

            _logger.LogInformation("Quartz job recovery check complete");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Job recovery cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during job recovery");
        }
    }

    /// <summary>
    /// Verifies that all required jobs are scheduled.
    /// Reschedules missing jobs if necessary.
    /// </summary>
    private async Task VerifyRequiredJobsAsync(
        IScheduler scheduler,
        List<JobKey> existingJobs,
        CancellationToken cancellationToken)
    {
        // TODO: Implement verification logic
        // This will check for required game engine jobs:
        // - Nightstorm event schedulers for each zone (81 jobs)
        // - Combat turn processors
        // - Travel completion checkers
        // - Zone treasury update jobs

        // Example of what will be implemented:
        /*
        var requiredJobCount = 81; // One for each zone
        if (existingJobs.Count < requiredJobCount)
        {
            _logger.LogWarning(
                "Expected {RequiredCount} jobs but found {ActualCount}, rescheduling missing jobs",
                requiredJobCount,
                existingJobs.Count);
            
            // Reschedule missing zone jobs
            await RescheduleZoneJobsAsync(scheduler, existingJobs, cancellationToken);
        }
        */

        await Task.CompletedTask;
    }
}
