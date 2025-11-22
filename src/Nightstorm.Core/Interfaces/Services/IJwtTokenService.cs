namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="username">Username</param>
    /// <param name="platform">Platform identifier (discord/web/mobile)</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(Guid userId, string username, string platform);

    /// <summary>
    /// Validates a JWT token and returns user ID.
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    Guid? ValidateToken(string token);
}
