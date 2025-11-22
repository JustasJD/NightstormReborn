namespace Nightstorm.API.DTOs.Auth;

/// <summary>
/// Request to register or authenticate a Discord user.
/// </summary>
public record RegisterDiscordRequest
{
    public required ulong DiscordId { get; init; }
    public required string DiscordUsername { get; init; }
}

/// <summary>
/// Request to register with email/password.
/// </summary>
public record RegisterEmailRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}

/// <summary>
/// Request to login with email/password.
/// </summary>
public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

/// <summary>
/// Authentication response with JWT token.
/// </summary>
public record AuthResponse
{
    public required string AccessToken { get; init; }
    public required string TokenType { get; init; } = "Bearer";
    public required int ExpiresIn { get; init; }
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
}

/// <summary>
/// User information response.
/// </summary>
public record UserInfoResponse
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public string? Email { get; init; }
    public ulong? DiscordId { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public required DateTime CreatedAt { get; init; }
}
