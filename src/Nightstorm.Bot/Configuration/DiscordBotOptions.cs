namespace Nightstorm.Bot.Configuration;

/// <summary>
/// Configuration options for the Discord bot.
/// </summary>
public class DiscordBotOptions
{
    public const string SectionName = "DiscordBot";

    /// <summary>
    /// The bot token from Discord Developer Portal.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The command prefix for text-based commands (if using prefix commands).
    /// </summary>
    public string CommandPrefix { get; set; } = "!";

    /// <summary>
    /// Whether to log gateway events.
    /// </summary>
    public bool LogGatewayEvents { get; set; } = false;

    /// <summary>
    /// The activity status message displayed by the bot.
    /// </summary>
    public string ActivityStatus { get; set; } = "Nightstorm RPG";

    /// <summary>
    /// The activity type (Playing, Watching, Listening, Streaming, Competing).
    /// </summary>
    public string ActivityType { get; set; } = "Playing";
}
