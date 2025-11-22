using Nightstorm.Bot.Interfaces;

namespace Nightstorm.Bot;

/// <summary>
/// Background service that hosts the Discord bot.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDiscordBotService _discordBotService;

    public Worker(
        ILogger<Worker> logger,
        IDiscordBotService discordBotService)
    {
        _logger = logger;
        _discordBotService = discordBotService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Discord Bot Worker starting...");

        try
        {
            // Start the Discord bot
            await _discordBotService.StartAsync(stoppingToken);

            // Wait indefinitely until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Discord Bot Worker is stopping due to cancellation.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the Discord Bot Worker.");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Discord Bot Worker is stopping...");

        await _discordBotService.StopAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
