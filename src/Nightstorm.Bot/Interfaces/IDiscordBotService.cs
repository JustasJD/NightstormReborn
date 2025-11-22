using Discord.WebSocket;

namespace Nightstorm.Bot.Interfaces;

/// <summary>
/// Interface for Discord bot service operations.
/// </summary>
public interface IDiscordBotService
{
    /// <summary>
    /// Gets the Discord socket client instance.
    /// </summary>
    DiscordSocketClient Client { get; }

    /// <summary>
    /// Starts the Discord bot and connects to Discord gateway.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop the bot.</param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the Discord bot and disconnects from Discord gateway.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StopAsync(CancellationToken cancellationToken = default);
}
