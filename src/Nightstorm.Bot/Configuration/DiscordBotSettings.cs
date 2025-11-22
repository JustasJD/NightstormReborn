namespace Nightstorm.Bot.Configuration;

/// <summary>
/// Discord bot configuration settings.
/// </summary>
public class DiscordBotSettings
{
    /// <summary>
    /// Discord bot token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Guild (server) ID where the bot operates.
    /// </summary>
    public ulong GuildId { get; set; }
    
    /// <summary>
    /// Channel IDs for various bot functions.
    /// </summary>
    public ChannelSettings Channels { get; set; } = new();
    
    /// <summary>
    /// Role IDs for user progression.
    /// </summary>
    public RoleSettings Roles { get; set; } = new();
    
    /// <summary>
    /// Character deletion settings.
    /// </summary>
    public CharacterDeletionSettings CharacterDeletion { get; set; } = new();
}

/// <summary>
/// Channel configuration.
/// </summary>
public class ChannelSettings
{
    /// <summary>
    /// Welcome channel ID.
    /// </summary>
    public ulong WelcomeChannelId { get; set; }
    
    /// <summary>
    /// Rules channel ID (where users verify).
    /// </summary>
    public ulong RulesChannelId { get; set; }
    
    /// <summary>
    /// Character creation channel ID.
    /// </summary>
    public ulong CharacterCreationChannelId { get; set; }
    
    /// <summary>
    /// Main game channel ID.
    /// </summary>
    public ulong GameChannelId { get; set; }
    
    /// <summary>
    /// Support/help channel ID.
    /// </summary>
    public ulong SupportChannelId { get; set; }
}

/// <summary>
/// Role configuration.
/// </summary>
public class RoleSettings
{
    /// <summary>
    /// Role ID given after reading rules.
    /// </summary>
    public ulong VerifiedRoleId { get; set; }
    
    /// <summary>
    /// Role ID given after creating a character.
    /// </summary>
    public ulong AdventurerRoleId { get; set; }
}

/// <summary>
/// Character deletion configuration.
/// </summary>
public class CharacterDeletionSettings
{
    /// <summary>
    /// Number of free deletions allowed per user.
    /// </summary>
    public int FreeDeletions { get; set; } = 1;
    
    /// <summary>
    /// Cost in gold for paid deletions.
    /// </summary>
    public int DeletionCostGold { get; set; } = 1000;
}
