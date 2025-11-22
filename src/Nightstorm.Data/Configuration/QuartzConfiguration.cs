using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Nightstorm.Data.Configuration;

/// <summary>
/// Extension methods for configuring Quartz.NET with PostgreSQL persistence.
/// Ensures scheduled jobs survive server restarts.
/// </summary>
public static class QuartzConfiguration
{
    /// <summary>
    /// Configures Quartz.NET with PostgreSQL persistence and clustering support.
    /// Jobs will be stored in database and automatically recovered on server restart.
    /// </summary>
    public static IServiceCollection AddQuartzWithPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string not found");

        services.AddQuartz(q =>
        {
            // Use unique scheduler instance ID
            q.SchedulerId = "NightstormScheduler";
            q.SchedulerName = "Nightstorm Game Engine Scheduler";

            // Use PostgreSQL for job storage
            q.UsePersistentStore(store =>
            {
                store.UsePostgres(postgres =>
                {
                    postgres.ConnectionString = connectionString;
                    postgres.TablePrefix = "qrtz_"; // Standard Quartz table prefix
                });

                store.UseJsonSerializer(); // Serialize job data as JSON
                store.UseClustering(); // Support multiple server instances
            });

            // Configure thread pool
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10; // Max 10 jobs running simultaneously
            });
        });

        // Add Quartz hosted service (starts scheduler on app startup)
        services.AddQuartzHostedService();

        return services;
    }
}
