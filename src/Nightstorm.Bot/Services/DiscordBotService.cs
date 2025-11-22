using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Nightstorm.Bot.Configuration;
using Nightstorm.Bot.Interfaces;

namespace Nightstorm.Bot.Services;

/// <summary>
/// Core Discord bot service that manages the Discord client lifecycle.
/// </summary>
public class DiscordBotService : IDiscordBotService
{
    private readonly ILogger<DiscordBotService> _logger;
    private readonly DiscordBotOptions _options;
    private readonly DiscordSocketClient _client;

    public DiscordSocketClient Client => _client;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        IOptions<DiscordBotOptions> options)
    {
        _logger = logger;
        _options = options.Value;

        // Configure Discord client
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                           GatewayIntents.GuildMessages |
                           GatewayIntents.MessageContent |
                           GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 100,
            LogLevel = _options.LogGatewayEvents ? LogSeverity.Debug : LogSeverity.Info
        };

        _client = new DiscordSocketClient(config);

        // Wire up logging
        _client.Log += LogAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Discord bot...");

        // Validate token
        if (string.IsNullOrWhiteSpace(_options.Token))
        {
            throw new InvalidOperationException("Discord bot token is not configured. Please set the DiscordBot:Token configuration value.");
        }

        // Login and start
        await _client.LoginAsync(TokenType.Bot, _options.Token);
        await _client.StartAsync();

        _logger.LogInformation("Discord bot started successfully.");
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping Discord bot...");

        await _client.StopAsync();
        await _client.LogoutAsync();

        _logger.LogInformation("Discord bot stopped successfully.");
    }

    private Task LogAsync(LogMessage log)
    {
        var logLevel = log.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };

        _logger.Log(logLevel, log.Exception, "[{Source}] {Message}", log.Source, log.Message);

        return Task.CompletedTask;
    }
}
