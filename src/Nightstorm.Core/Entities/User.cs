namespace Nightstorm.Core.Entities;

/// <summary>
/// Represents a user account that can have multiple platform identities.
/// Platform-agnostic for Discord, Web, Mobile support.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the username (unique across all platforms).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address (optional, for web/mobile users).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password hash (for email/password authentication).
    /// Null for Discord-only users.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the Discord user ID (optional, for Discord users).
    /// </summary>
    public ulong? DiscordId { get; set; }

    /// <summary>
    /// Gets or sets the Discord username at time of registration.
    /// </summary>
    public string? DiscordUsername { get; set; }

    /// <summary>
    /// Gets or sets the Google ID (for Google OAuth users).
    /// </summary>
    public string? GoogleId { get; set; }

    /// <summary>
    /// Gets or sets the Apple ID (for Apple Sign-In users).
    /// </summary>
    public string? AppleId { get; set; }

    /// <summary>
    /// Gets or sets the last login timestamp.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets whether the account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to characters owned by this user.
    /// </summary>
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
