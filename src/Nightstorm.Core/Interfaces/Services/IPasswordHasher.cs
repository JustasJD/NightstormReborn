namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for securely hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using PBKDF2 with SHA256.
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Base64 encoded hash (salt + hash)</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a stored hash.
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="passwordHash">Stored password hash</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string passwordHash);
}
